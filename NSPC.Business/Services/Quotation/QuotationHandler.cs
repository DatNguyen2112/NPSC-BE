using AutoMapper;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.Quotation;
using Serilog;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using NSPC.Data.Data.Entity.VatTu;
using NSPC.Data.Entity;
using static NSPC.Common.Helper;
using NSPC.Business.Services.InventoryNote;
using MongoDB.Driver.Linq;

namespace NSPC.Business.Services.Quotation
{
    public class QuotationHandler : IQuotationHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IKhachHangHandler _customerHandler;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        private readonly List<string> _listPropertyBaoGia;
        public QuotationHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IKhachHangHandler customerHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _customerHandler = customerHandler;
            _mapper = mapper;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
        }

        public async Task<Response<QuotationViewModel>> Create(QuotationCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                List<sm_Product> allQuotationsProducts = new List<sm_Product>();

                if (model.QuotationItem == null || !model.QuotationItem.Any())
                    return Helper.CreateBadRequestResponse<QuotationViewModel>("Báo giá cần ít nhất 1 sản phẩm/vật tư.");

                if (model.QuotationItem != null && model.QuotationItem.Count > 0)
                {
                    // Fill tat ca products
                    var allProductIds = model.QuotationItem.Select(x => x.ProductId).ToList();
                    allQuotationsProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Khong duoc duplicate dong trong Product Item
                    // Neu so luong distinct product id khac so luong product item => co dong trung
                    if (model.QuotationItem.Count !=
                        model.QuotationItem.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<QuotationViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.QuotationItem.Any(x => x.Quantity < 0))
                        return Helper.CreateBadRequestResponse<QuotationViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn 0.");

                    if (model.QuotationItem.Any(x => x.UnitPrice < 0))
                        return Helper.CreateBadRequestResponse<QuotationViewModel>
                                    ("Danh sách sản phẩm không được có đơn giá nhỏ hơn 0.");

                    foreach (var item in model.QuotationItem)
                    {
                        var product = allQuotationsProducts
                                            .Where(x => x.Id == item.ProductId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<QuotationViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");

                        if (product.IsOrder == false)
                            return Helper.CreateBadRequestResponse<QuotationViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không cho phép bán");
                    }
                }

                #region Fill Item's Line No
                if (model.QuotationItem != null && model.QuotationItem.Count > 0)
                {
                    model.QuotationItem = model.QuotationItem.OrderBy(x => x.LineNumber).ToList();
                    for (int i = 0; i < model.QuotationItem.Count; i++)
                        model.QuotationItem[i].LineNumber = i + 1;
                }
                #endregion

                // Mapping và tạo entity
                var entity = _mapper.Map<sm_Quotation>(model);
                entity.Code = await GenerateNewQuotationCode(PrefixConstants.QUOTATION_PREFIX);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.SubTotalAmount = model.QuotationItem.Sum(x => x.AfterLineDiscountGoodsAmount);
                entity.PaymentMethodName = CodeTypeCollection.Instance.FetchCode(entity.PaymentMethodCode, "vn", currentUser.TenantId).Title;

                #region Tính toán Quotation Items

                // Fill product & tính discount từng line
                foreach (var item in entity.QuotationItem)
                {
                    var product = allQuotationsProducts
                        .FirstOrDefault(x => x.Id == item.ProductId);

                    if (product != null)
                    {
                        item.Code = product.Code;
                        item.Name = product.Name;
                        item.Unit = product.Unit;
                    }

                    // GoodsAmount: Thành tiền trước chiết khấu
                    item.GoodsAmount = item.Quantity * item.UnitPrice.GetValueOrDefault(0M);

                    // Line Discount (tính trên đơn giá)
                    switch (item.UnitPriceDiscountType)
                    {
                        // Chế độ tính discount bằng %
                        case QuotationConstants.UnitPriceDiscountType.Percent:
                            item.UnitPriceDiscountAmount = item.UnitPrice.GetValueOrDefault() * item.UnitPriceDiscountPercent.GetValueOrDefault(0M) / 100;
                            break;

                        // Chế độ tính discount bằng value
                        case QuotationConstants.UnitPriceDiscountType.Value:
                            if (item.UnitPrice.GetValueOrDefault() != 0)
                                item.UnitPriceDiscountPercent = item.UnitPriceDiscountAmount * 100 / item.UnitPrice.GetValueOrDefault();
                            else
                                item.UnitPriceDiscountPercent = 0;
                            break;
                    }

                    // DiscountedGoodsAmount: giá trị hàng hóa sau chiết khấu dòng
                    item.AfterLineDiscountGoodsAmount = item.GoodsAmount - item.UnitPriceDiscountAmount * item.Quantity;
                }

                /*
                 * Phân bổ ngược lại DiscountAmount của Order vào từng dòng
                 * từ đó tính ra SalesOrderDiscountAmount
                 */

                // B1. Tính tổng AfterLineDiscountGoodsAmount của toàn bộ dòng
                var totalAfterDiscountGoodsAmount = entity.QuotationItem
                    .Sum(x => x.AfterLineDiscountGoodsAmount);

                // Bước 2. Tính ra DiscountAmount của đơn hàng
                switch (entity.DiscountType)
                {
                    // Chế độ tính discount bằng %
                    case QuotationConstants.DiscountType.Percent:
                        entity.DiscountAmount = totalAfterDiscountGoodsAmount * entity.DiscountPercent / 100;
                        break;

                    // Chế độ tính discount bằng value
                    case QuotationConstants.DiscountType.Value:
                        if (totalAfterDiscountGoodsAmount != 0)
                            entity.DiscountPercent = entity.DiscountAmount * 100 / totalAfterDiscountGoodsAmount;
                        else
                            entity.DiscountPercent = 0;
                        break;
                }

                // Bước 3. Phân bổ lại từng dòng ra Order Discount bằng AfterLineDiscountGoodsAmount

                foreach (var item in entity.QuotationItem)
                {
                    if (totalAfterDiscountGoodsAmount != 0)
                        item.OrderDiscountAmount = entity.DiscountAmount
                                                        * item.AfterLineDiscountGoodsAmount
                                                        / totalAfterDiscountGoodsAmount;
                    else
                        item.OrderDiscountAmount = 0;

                    // Todo: item.VATPercent = product.VATPercent;
                    // Thêm phần checkbox có tính thuế cho sản phẩm và lựa chọn mức thuế
                    var product = allQuotationsProducts
                                             .Where(x => x.Id == item.ProductId)
                                             .FirstOrDefault();

                    item.IsProductVATApplied = product.IsVATApplied;
                    item.LineVATCode = product.ExportVATCode;
                    item.LineVATPercent = product.IsVATApplied ? product.ExportVATPercent : 0M;
                    item.LineVATableAmount = item.AfterLineDiscountGoodsAmount - item.OrderDiscountAmount;
                    item.LineVATAmount = item.LineVATableAmount * item.LineVATPercent / 100;
                    item.LineAmount = item.LineVATableAmount + item.LineVATAmount;
                }
                #endregion


                #region Amount

                // SubTotal = Tổng tiền sau khi chiết khấu dòng của từng Line
                entity.SubTotalAmount = entity.QuotationItem.Sum(x => x.AfterLineDiscountGoodsAmount);

                // Số tiền VAT
                entity.TotalVatAmount = entity.QuotationItem.Sum(x => x.LineVATAmount);

                // Tổng phải trả
                entity.TotalAmount = entity.QuotationItem.Sum(x => x.AfterLineDiscountGoodsAmount) - entity.DiscountAmount + entity.TotalVatAmount + entity.ShippingCostAmount + entity.OtherCostAmount;

                #endregion

                _dbContext.sm_Quotation.Add(entity);

                var customer = await _dbContext.sm_Customer.FirstOrDefaultAsync(x => x.Id == model.CustomerId.Value);

                if (model.CustomerId.HasValue)
                {
                    if (customer == null)
                        return Helper.CreateBadRequestResponse<QuotationViewModel>("Khách hàng không tồn tại");
                    entity.CustomerId = customer.Id;
                    entity.CustomerCode = customer.Code;
                    entity.CustomerName = customer.Name;
                    entity.CustomerTaxCode = customer.TaxCode;

                    entity.CustomerAddress = string.Join(", ",
                        new[]
                        {
                            customer.Address,
                            customer.WardName,
                            customer.DistrictName,
                            customer.ProvinceName
                        }.Where(field => !string.IsNullOrWhiteSpace(field)));

                    entity.CustomerPhoneNumber = customer.PhoneNumber;
                    _dbContext.sm_Quotation.Add(entity);
                    await _dbContext.SaveChangesAsync();
                    // Cập nhật TotalQuotationCount bằng cách đếm từ bảng Quotation
                    customer.TotalQuotationCount = _dbContext.sm_Quotation
                        .AsNoTracking()
                        .Count(x => x.CustomerId == model.CustomerId.Value);
                }
                else
                {
                    var customerModel = new KhachHangCreateUpdateModel
                    {
                        Code = await GenerateNewCustomerCode(PrefixConstants.CUSTOMER_PREFIX),
                        Name = model.CustomerName,
                        TaxCode = model.CustomerTaxCode,
                        Address = model.CustomerAddress,
                        PhoneNumber = model.CustomerPhoneNumber,
                    };
                    _dbContext.sm_Quotation.Add(entity);
                    await _dbContext.SaveChangesAsync();
                    // Cập nhật TotalQuotationCount bằng cách đếm từ bảng Quotation
                    customer.TotalQuotationCount = _dbContext.sm_Quotation
                        .AsNoTracking()
                        .Count(x => x.CustomerId == model.CustomerId.Value);

                    var response = await _customerHandler.Create(customerModel, currentUser);

                    entity.CustomerCode = await GenerateNewCustomerCode(PrefixConstants.CUSTOMER_PREFIX);
                    entity.CustomerId = response.Data.Id;
                }

                //Assign project
                if (model.ProjectId.HasValue)
                {
                    var project = await _dbContext.mk_DuAn.FirstOrDefaultAsync(x => x.Id == model.ProjectId.Value);
                    if (project == null)
                        return Helper.CreateBadRequestResponse<QuotationViewModel>("Dự án không tồn tại");
                    entity.ProjectCode = project.MaDuAn;
                    entity.ProjectName = project.TenDuAn;
                }

                await _dbContext.SaveChangesAsync();

                var result = await GetById(entity.Id);

                if (result.IsSuccess)
                    result.Message = "Thêm mới thành công";

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<QuotationViewModel>(ex);
            }
        }

        public async Task<Response<QuotationViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Quotation.AsNoTracking().Include(x =>x.sm_Customer).Include(x => x.QuotationItem.OrderBy(x => x.LineNumber)).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<QuotationViewModel>("Mã báo giá không tồn tại trong hệ thống.");

                var result = _mapper.Map<QuotationViewModel>(entity);

                return new Response<QuotationViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<QuotationViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<QuotationViewModel>>> GetPage(QuotationQueryModel query)
        {
            try
            {

                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_Quotation.AsNoTracking().Include(x => x.QuotationItem.OrderBy(x => x.LineNumber)).Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<QuotationViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<QuotationViewModel>>(ex);
            }
        }

        public async Task<Response<QuotationViewModel>> Update(Guid id, QuotationCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Quotation.Include(x => x.QuotationItem).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<QuotationViewModel>(string.Format("Báo giá không tồn tại trong hệ thống!"));

                var oldCustomerId = entity.CustomerId;

                List<sm_Product> allQuotationProducts = new List<sm_Product>();

                #region Validate model

                // Validate Customer
                if (!await _dbContext.sm_Customer.AnyAsync(x => x.Id == model.CustomerId))
                {
                    return Helper.CreateNotFoundResponse<QuotationViewModel>
                        ($"Customer Id: {model.CustomerId} not found.");
                }

                // Validate Project
                if (model.ProjectId.HasValue && !await _dbContext.mk_DuAn.AnyAsync(x => x.Id == model.ProjectId))
                {
                    return Helper.CreateNotFoundResponse<QuotationViewModel>
                        ($"Project Id: {model.ProjectId} not found.");
                }

                // Validate Product Item Model
                // 1.Product ton tai trong he thong
                // 2.Gia nhap vao khong duoc< 0
                // 3.So luong khong duoc < 0
                // 4.Product phai duoc cho phep ban
                // 6.Khong duoc duplicate dong trong Product Item
                if (model.QuotationItem != null && model.QuotationItem.Count > 0)
                {
                    // Fill tat ca products
                    var allProductIds = model.QuotationItem.Select(x => x.ProductId).ToList();
                    allQuotationProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Khong duoc duplicate dong trong Product Item
                    // Neu so luong distinct product id khac so luong product item => co dong trung
                    if (model.QuotationItem.Count !=
                        model.QuotationItem.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<QuotationViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.QuotationItem.Any(x => x.Quantity < 0))
                        return Helper.CreateBadRequestResponse<QuotationViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn 0.");

                    if (model.QuotationItem.Any(x => x.UnitPrice < 0))
                        return Helper.CreateBadRequestResponse<QuotationViewModel>
                                    ("Danh sách sản phẩm không được có đơn giá nhỏ hơn 0.");

                    foreach (var item in model.QuotationItem)
                    {
                        var product = allQuotationProducts
                                            .Where(x => x.Id == item.ProductId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<QuotationViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");

                        if (product.IsOrder == false)
                            return Helper.CreateBadRequestResponse<QuotationViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không cho phép bán");
                    }
                }
                #endregion

                #region Fill Item's Line Number

                if (model.QuotationItem != null && model.QuotationItem.Count > 0)
                {
                    model.QuotationItem = model.QuotationItem.OrderBy(x => x.LineNumber).ToList();
                    for (int i = 0; i < model.QuotationItem.Count; i++)
                        model.QuotationItem[i].LineNumber = i + 1;
                }
                #endregion

                #region Điền thông tin Khách hàng hoặc tạo Khách hàng nếu chưa có
                //Update customer
                var customer = await _dbContext.sm_Customer.FirstOrDefaultAsync(x => x.Id == model.CustomerId.Value);
                if (model.CustomerId.HasValue)
                {
                    var oldCustomer = await _dbContext.sm_Customer.FirstOrDefaultAsync(x => x.Id == oldCustomerId);
                    if (customer == null)
                        return Helper.CreateBadRequestResponse<QuotationViewModel>("Khách hàng không tồn tại");
                    entity.CustomerId = customer.Id;
                    entity.CustomerCode = customer.Code;
                    entity.CustomerName = customer.Name;
                    entity.CustomerTaxCode = customer.TaxCode;
                    entity.CustomerPhoneNumber = customer.PhoneNumber;

                    entity.CustomerAddress = string.Join(", ",
                        new[]
                        {
                            customer.Address,
                            customer.WardName,
                            customer.DistrictName,
                            customer.ProvinceName
                        }.Where(field => !string.IsNullOrWhiteSpace(field)));

                    // Tính lại TotalQuotationCount của khách hàng cũ
                    oldCustomer.TotalQuotationCount -= 1;
                    await _dbContext.SaveChangesAsync();
                    // Cập nhật TotalQuotationCount bằng cách đếm từ bảng Quotation
                    customer.TotalQuotationCount = _dbContext.sm_Quotation
                        .AsNoTracking()
                        .Count(x => x.CustomerId == model.CustomerId.Value);
                }
                else
                {
                    var customerModel = new KhachHangCreateUpdateModel
                    {
                        Code = await GenerateNewCustomerCode(PrefixConstants.CUSTOMER_PREFIX),
                        Name = model.CustomerName,
                        TaxCode = model.CustomerTaxCode,
                        Address = model.CustomerAddress,
                        PhoneNumber = model.CustomerPhoneNumber,
                    };
                    // Cập nhật TotalQuotationCount bằng cách đếm từ bảng Quotation
                    customer.TotalQuotationCount = _dbContext.sm_Quotation
                        .AsNoTracking()
                        .Count(x => x.CustomerId == model.CustomerId.Value);

                    var response = await _customerHandler.Create(customerModel, currentUser);

                    entity.CustomerCode = await GenerateNewCustomerCode(PrefixConstants.CUSTOMER_PREFIX);
                    entity.CustomerId = response.Data.Id;
                }
                #endregion

                #region Điền thông tin dự án
                //Update project
                if (model.ProjectId.HasValue)
                {
                    var project = await _dbContext.mk_DuAn.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.ProjectId.Value);
                    if (project == null)
                        return Helper.CreateBadRequestResponse<QuotationViewModel>("Dự án không tồn tại");
                    entity.ProjectId = project.Id;
                    entity.ProjectCode = project.MaDuAn;
                    entity.ProjectName = project.TenDuAn;
                }
                #endregion

                // Cập nhật các thông tin
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = $"{currentUser.FullName} ({currentUser.UserName})";
                entity.LastModifiedOnDate = DateTime.Now;

                entity.DueDate = model.DueDate;
                entity.TypeCode = model.TypeCode;
                entity.PaymentMethodCode = model.PaymentMethodCode;
                entity.Status = model.Status;
                entity.Note = model.Note;
                entity.SubTotalAmount = 0M;
                entity.DiscountType = model.DiscountType;
                entity.DiscountAmount = model.DiscountAmount;
                entity.DiscountPercent = model.DiscountPercent;
                entity.ShippingCostAmount = model.ShippingCostAmount;
                entity.OtherCostAmount = model.OtherCostAmount;
                entity.TotalAmount = 0M;

                // Remove Old Quotation Item -> Re-add
                _dbContext.RemoveRange(entity.QuotationItem);
                entity.QuotationItem = new List<sm_QuotationItem>();

                #region Điền thông tin Product, GoodsAmount, Line Discount

                // Fill Product Data, GoodsAmount, DiscountAmount
                foreach (var modelItem in model.QuotationItem)
                {
                    // Readd new item
                    var item = _mapper.Map<sm_QuotationItem>(modelItem);
                    entity.QuotationItem.Add(item);

                    // Assign SalesOrderId
                    item.QuotationId = entity.Id;

                    // Fill product data
                    var product = allQuotationProducts
                                    .Where(x => x.Id == modelItem.ProductId)
                                    .FirstOrDefault();

                    item.Code = product.Code;
                    item.Name = product.Name;
                    item.Unit = product.Unit;

                    // GoodsAmount: giá trị hàng hóa
                    item.GoodsAmount = item.Quantity * item.UnitPrice.GetValueOrDefault(0M);

                    // Line Discount (tính trên đơn giá)
                    switch (item.UnitPriceDiscountType)
                    {
                        // Chế độ tính discount bằng %
                        case QuotationConstants.UnitPriceDiscountType.Percent:
                            item.UnitPriceDiscountAmount = item.UnitPrice.GetValueOrDefault() * item.UnitPriceDiscountPercent.GetValueOrDefault(0M) / 100;
                            break;

                        // Chế độ tính discount bằng value
                        case QuotationConstants.UnitPriceDiscountType.Value:
                            if (item.UnitPrice.GetValueOrDefault() != 0)
                                item.UnitPriceDiscountPercent = item.UnitPriceDiscountAmount * 100 / item.UnitPrice.GetValueOrDefault();
                            else
                                item.UnitPriceDiscountPercent = 0;
                            break;
                    }

                    // DiscountedGoodsAmount: giá trị hàng hóa sau chiết khấu dòng
                    item.AfterLineDiscountGoodsAmount = item.GoodsAmount - item.UnitPriceDiscountAmount * item.Quantity;
                }

                /*
                    * Phân bổ ngược lại DiscountAmount của Order vào từng dòng
                    * từ đó tính ra SalesOrderDiscountAmount
                    */

                // B1. Tính tổng AfterLineDiscountGoodsAmount của toàn bộ dòng
                var totalAfterDiscountGoodsAmount = entity.QuotationItem
                .Sum(x => x.AfterLineDiscountGoodsAmount);

                // B2. Tính ra OrderDiscount
                switch (entity.DiscountType)
                {
                    // Chế độ tính discount bằng %
                    case SalesOrderConstants.DiscountType.Percent:
                        entity.DiscountAmount = totalAfterDiscountGoodsAmount * entity.DiscountPercent / 100;
                        break;

                    // Chế độ tính discount bằng value
                    case SalesOrderConstants.DiscountType.Value:
                        if (totalAfterDiscountGoodsAmount != 0)
                            entity.DiscountPercent = entity.DiscountAmount * 100 / totalAfterDiscountGoodsAmount;
                        else
                            entity.DiscountPercent = 0;
                        break;
                }

                // B2. Phân bổ lại giá trị cho từng Line
                foreach (var item in entity.QuotationItem)
                {
                    if (totalAfterDiscountGoodsAmount != 0)
                        item.OrderDiscountAmount = entity.DiscountAmount
                                                        * item.AfterLineDiscountGoodsAmount
                                                        / totalAfterDiscountGoodsAmount;
                    else
                        item.OrderDiscountAmount = 0;

                    // Todo: item.VATPercent = product.VATPercent;
                    // Thêm phần checkbox có tính thuế cho sản phẩm và lựa chọn mức thuế
                    // Fill product data
                    var product = allQuotationProducts
                                    .Where(x => x.Id == item.ProductId)
                                    .FirstOrDefault();

                    item.IsProductVATApplied = product.IsVATApplied;
                    item.LineVATCode = product.ExportVATCode;
                    item.LineVATPercent = product.IsVATApplied ? product.ExportVATPercent : 0M;
                    item.LineVATableAmount = item.AfterLineDiscountGoodsAmount - item.OrderDiscountAmount;
                    item.LineVATAmount = item.LineVATableAmount * item.LineVATPercent / 100;
                    item.LineAmount = item.LineVATableAmount + item.LineVATAmount;
                }

                // B3. Thêm lại item sau khi tính toán
                foreach (var item in entity.QuotationItem)
                {
                    _dbContext.Sm_QuotationItems.Add(item);
                }

                #endregion

                #region Amount

                // SubTotal = Tổng tiền sau khi chiết khấu dòng của từng Line
                entity.SubTotalAmount = entity.QuotationItem.Sum(x => x.AfterLineDiscountGoodsAmount);

                //// Total = Tổng tiền người dùng phải trả từng Line + Phí giao hàng + Chi phí khác
                //entity.Total = entity.QuotationItem.Sum(x => x.LineAmount) + entity.ShippingCostAmount + entity.OtherCostAmount;

                // Số tiền VAT
                entity.TotalVatAmount = entity.QuotationItem.Sum(x => x.LineVATAmount);

                // Tổng phải trả
                entity.TotalAmount = entity.QuotationItem.Sum(x => x.AfterLineDiscountGoodsAmount) - entity.DiscountAmount + entity.TotalVatAmount + entity.ShippingCostAmount + entity.OtherCostAmount;

                #endregion

                _dbContext.sm_Quotation.Update(entity);

                await _dbContext.SaveChangesAsync();

                var result = await GetById(entity.Id);

                if (result.IsSuccess)
                    result.Message = "Chỉnh sửa thành công";

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<QuotationViewModel>(ex);
            }
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Quotation.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<QuotationViewModel>(string.Format("Báo giá không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse("Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse(ex);
            }
        }

        

        private Expression<Func<sm_Quotation, bool>> BuildQuery(QuotationQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_Quotation>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate = predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.Code.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.CustomerName.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.CustomerPhoneNumber.ToLower().Contains(query.FullTextSearch.ToLower())
                );

            if (!string.IsNullOrEmpty(query.TypeCode))
                predicate = predicate.And(s => s.TypeCode == query.TypeCode);

            if (query.FromDate.HasValue)
            {
                var time = new DateTime(query.FromDate.Value.ToLocalTime().Year,
                    query.FromDate.Value.ToLocalTime().Month, query.FromDate.Value.ToLocalTime().Day, 0, 0, 1);
                predicate = predicate.And(s => s.CreatedOnDate >= time);
            }
            if (query.ToDate.HasValue)
            {
                var time = new DateTime(query.ToDate.Value.ToLocalTime().Year,
                    query.ToDate.Value.ToLocalTime().Month, query.ToDate.Value.ToLocalTime().Day, 23, 59, 59);
                predicate = predicate.And(s => s.CreatedOnDate <= time);
            }

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date.AddDays(1).AddTicks(-1));
            }

            if (!currentUser.ListRights.Contains("QUOTATION." + RightActionConstants.VIEWALL))
                predicate.And(s => s.CreatedByUserId == currentUser.UserId);

            return predicate;
        }

        /// <summary>
        /// Generate new Quotation code
        /// </summary>
        /// <param name="defaultPrefix"></param>
        /// <returns></returns>
        public async Task<string> GenerateNewQuotationCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_Quotation.AsNoTracking().Where(x => x.Code.Contains(code)).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

                if (result != null)
                {
                    var currentNum = result.Code.Substring(result.Code.Length - 3, 3);
                    var currentNumInt = int.Parse(currentNum) + 1;
                    var stringResult = "";
                    if (currentNumInt < 10)
                    {
                        stringResult = "00" + currentNumInt;
                    }
                    else if (currentNumInt >= 10 && currentNumInt < 100)
                    {
                        stringResult = "0" + currentNumInt;
                    }
                    else
                    {
                        stringResult = currentNumInt.ToString();
                    }

                    return code + stringResult;
                }
                else
                {
                    return code + "001";
                }
            }
            catch (Exception ex)
            {
                Log.Error("", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Generate new Customer code
        /// </summary>
        /// <param name="defaultPrefix"></param>
        /// <returns></returns>
        public async Task<string> GenerateNewCustomerCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_Customer.AsNoTracking().Where(x => x.Code.Contains(code)).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

                if (result != null)
                {
                    var currentNum = result.Code.Substring(result.Code.Length - 3, 3);
                    var currentNumInt = int.Parse(currentNum) + 1;
                    var stringResult = "";
                    if (currentNumInt < 10)
                    {
                        stringResult = "00" + currentNumInt;
                    }
                    else if (currentNumInt >= 10 && currentNumInt < 100)
                    {
                        stringResult = "0" + currentNumInt;
                    }
                    else
                    {
                        stringResult = currentNumInt.ToString();
                    }

                    return code + stringResult;
                }
                else
                {
                    return code + "001";
                }
            }
            catch (Exception ex)
            {
                Log.Error("", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Xuất danh sách báo giá ra file excel theo query
        /// </summary>
        /// <param name="query">Các tham số lọc và phân trang</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<string>> ExportExcelListPage(QuotationQueryModel query)
        {
            try
            {
                // Tạo predicate để lọc dựa trên các tham số trong query
                var predicate = BuildQuery(query);

                // Lấy danh sách phiếu nhập hoặc phiếu xuất từ cơ sở dữ liệu dựa trên lọc và phân trang
                var quotations = await _dbContext.sm_Quotation.AsNoTracking()
                    .Include(x => x.QuotationItem)
                    .Where(predicate)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .GetPageAsync(query);

                // Kiểm tra nếu không có dữ liệu trả về trong trang hiện tại
                if (quotations == null || quotations.Content == null || quotations.Content.Count == 0)
                    return Helper.CreateNotFoundResponse<string>("Không có báo giá nào tồn tại trong hệ thống.");

                // Đặt tên file và đường dẫn template dựa trên loại phiếu
                var fileName = $"danh sách báo giá_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}.xlsx";
                var filePath = Path.Combine(_staticsFolder, fileName);

                // Xác định đường dẫn template dựa trên loại phiếu
                var templatePath = Path.Combine(_staticsFolder, "excel-template/QuotationTemplate.xlsx");

                if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                    return Helper.CreateBadRequestResponse<string>("Không tìm thấy file template");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Mở template Excel và điền dữ liệu vào file
                using (var package = new ExcelPackage(new FileInfo(templatePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Giả sử dữ liệu nằm trong worksheet đầu tiên

                    // Đặt độ rộng cố định cho cột A (cột STT) là 8
                    worksheet.Column(1).Width = 8;

                    // Điền dữ liệu vào bảng Excel (giả sử bắt đầu từ hàng thứ 4)
                    int startRow = 4;
                    int index = 1;

                    foreach (var note in quotations.Content)
                    {
                        worksheet.Cells[startRow, 1].Value = index; // STT
                        worksheet.Cells[startRow, 2].Value = note.CreatedOnDate.ToString("dd/MM/yyyy"); // Ngày tạo
                        worksheet.Cells[startRow, 3].Value = note.LastModifiedOnDate?.ToString("dd/MM/yyyy"); // Ngày cập nhật
                        worksheet.Cells[startRow, 4].Value = note.Code; // Mã báo giá
                        worksheet.Cells[startRow, 5].Value = note.TypeCode == QuotationConstants.TypeCode.Quotation_Material ? QuotationConstants.TypeName.Quotation_Material : QuotationConstants.TypeName.Quotation_Product; // Loại báo giá

                        string statusName;
                        switch (note.Status)
                        {
                            case QuotationConstants.StatusCode.DRAFT:
                                statusName = QuotationConstants.StatusName.DRAFT;
                                break;
                            case QuotationConstants.StatusCode.PENDING_APPROVAL:
                                statusName = QuotationConstants.StatusName.PENDING_APPROVAL;
                                break;
                            case QuotationConstants.StatusCode.INTERNAL_APPROVAL:
                                statusName = QuotationConstants.StatusName.INTERNAL_APPROVAL;
                                break;
                            case QuotationConstants.StatusCode.CUSTOMER_APPROVED:
                                statusName = QuotationConstants.StatusName.CUSTOMER_APPROVED;
                                break;
                            case QuotationConstants.StatusCode.CANCELLED:
                                statusName = QuotationConstants.StatusName.CANCELLED;
                                break;
                            default:
                                statusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }

                        worksheet.Cells[startRow, 6].Value = statusName;  // Trạng thái
                        worksheet.Cells[startRow, 7].Value = note.TotalAmount; // Giá trị
                        worksheet.Cells[startRow, 8].Value = note.ProjectName; // Dự án
                        worksheet.Cells[startRow, 9].Value = note.CreatedByUserName; // Người tạo
                        worksheet.Cells[startRow, 10].Value = note.LastModifiedByUserName; // Người cập nhật cuối
                        worksheet.Cells[startRow, 11].Value = note.CustomerCode; // Mã khách hàng
                        worksheet.Cells[startRow, 12].Value = note.CustomerName; // Tên khách hàng
                        worksheet.Cells[startRow, 13].Value = note.CustomerAddress; // Địa chỉ
                        worksheet.Cells[startRow, 14].Value = note.CustomerPhoneNumber; // Số điện thoại
                        worksheet.Cells[startRow, 15].Value = note.PaymentMethodName; // Phương thức dự kiến
                        worksheet.Cells[startRow, 16].Value = note.DueDate.ToString("dd/MM/yyyy"); // Ngày hết hạn
                        worksheet.Cells[startRow, 17].Value = note.Note; // Ghi chú

                        startRow++;
                        index++;
                    }

                    int lastDataRow = startRow - 1;

                    // Thêm đường viền cho các ô đã điền dữ liệu
                    using (var range = worksheet.Cells[4, 1, lastDataRow, 17]) // điều chỉnh cột cuối (18) tùy theo số lượng cột bạn có
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    // Xóa các dòng thừa sau khi điền dữ liệu
                    int totalRows = worksheet.Dimension.End.Row;
                    if (totalRows > lastDataRow)
                    {
                        worksheet.DeleteRow(lastDataRow + 1, totalRows - lastDataRow);
                    }

                    // Tự động điều chỉnh kích thước các cột từ cột thứ hai đến cột cuối cùng
                    worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].AutoFitColumns();

                    // Lưu file Excel đã điền dữ liệu
                    await package.SaveAsAsync(new FileInfo(filePath));
                }

                return Helper.CreateSuccessResponse<string>(filePath, "Xuất file thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<string>(ex);
            }
        }
    }
}
