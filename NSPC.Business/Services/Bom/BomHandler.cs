using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.ChucVu;
using NSPC.Business.Services.Quotation;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.Bom;
using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data.Entity.NguyenVatLieu;
using NSPC.Data.Data.Entity.Quotation;
using NSPC.Data.Data.Entity.VatTu;
using Serilog;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.Bom
{
    public class BomHandler : IBomHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public BomHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<BomViewModel>> Create(BomCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                List<sm_Product> allQuotationsProducts = new List<sm_Product>();

                if (model.Materials == null || !model.Materials.Any())
                    return Helper.CreateBadRequestResponse<BomViewModel>("Bom cần ít nhất 1 nguyên vật liệu");

                if (model.Materials != null && model.Materials.Count > 0)
                {
                    // Fill tat ca products
                    var allProductIds = model.Materials.Select(x => x.MaterialId).ToList();
                    allQuotationsProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Khong duoc duplicate dong trong Product Item
                    // Neu so luong distinct product id khac so luong product item => co dong trung
                    if (model.Materials.Count !=
                        model.Materials.Select(x => x.MaterialId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<BomViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.Materials.Any(x => x.Quantity < 0))
                        return Helper.CreateBadRequestResponse<BomViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn 0.");

                    if (model.Materials.Any(x => x.UnitPrice < 0))
                        return Helper.CreateBadRequestResponse<BomViewModel>
                                    ("Danh sách sản phẩm không được có đơn giá nhỏ hơn 0.");

                    foreach (var item in model.Materials)
                    {
                        var product = allQuotationsProducts
                                            .Where(x => x.Id == item.MaterialId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<BomViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.MaterialId}");

                        if (product.IsOrder == false)
                            return Helper.CreateBadRequestResponse<BomViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không cho phép bán");
                    }
                }

                #region Fill Item's Line No
                if (model.Materials != null && model.Materials.Count > 0)
                {
                    model.Materials = model.Materials.OrderBy(x => x.LineNumber).ToList();
                    for (int i = 0; i < model.Materials.Count; i++)
                        model.Materials[i].LineNumber = i + 1;
                }
                #endregion

                #region Fill Item's Id
                if (model.ListOtherCost != null && model.ListOtherCost.Count > 0)
                {
                    for (int i = 0; i < model.ListOtherCost.Count; i++)
                    {
                        model.ListOtherCost[i].Id = Guid.NewGuid();
                    }
                }
                #endregion

                // Mapping và tạo entity
                var entity = _mapper.Map<sm_Bom>(model);
                //entity.Code = await GenerateNewQuotationCode(PrefixConstants.QUOTATION_PREFIX);
                entity.Id = Guid.NewGuid();

                if (model.Code == null) {
                    entity.Code = await GenerateNewBomCode(BomConstants.Code.PREFIX);
                }

                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;

                #region Tính toán nguyên vật liệu
                // Fill product & tính discount từng line
                foreach (var item in entity.Materials)
                {
                    var product = allQuotationsProducts
                        .FirstOrDefault(x => x.Id == item.MaterialId);

                    if (product != null)
                    {
                        item.MaterialCode = product.Code;
                        item.MaterialName = product.Name;
                        item.MaterialUnit = product.Unit;
                    }
                    // GoodsAmount: Thành tiền trước chiết khấu
                    item.LineAmount = item.Quantity * item.UnitPrice;
                    item.LineVatPercent = product.IsVATApplied ? product.ExportVATPercent : 0M;
                    item.LineVatAmount = item.LineAmount * item.LineVatPercent / 100;
                }
                #endregion

                #region Amount & Total Quantity
                entity.TotalQuantity = entity.Materials.Sum(x => x.Quantity);
                // SubTotal = Tổng tiền của từng line
                entity.SubTotalAmount = entity.Materials.Sum(x => x.LineAmount);
                // Số tiền VAT
                entity.TotalVatAmount = entity.Materials.Sum(x => x.LineVatAmount);
                entity.TotalOtherExpenses = entity.ListOtherCost.Sum(x => x.FeeCost ?? 0);
                entity.TotalAmount = entity.SubTotalAmount + entity.TotalVatAmount + entity.TotalOtherExpenses;
                #endregion

                _dbContext.sm_Bom.Add(entity);

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
                return Helper.CreateExceptionResponse<BomViewModel>(ex);
            }
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Bom.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<BomViewModel>(string.Format("Bom không tồn tại trong hệ thống!"));

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

        public async Task<Response<BomViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Bom.AsNoTracking().Include(x => x.Product).Include(x => x.Materials.OrderBy(x => x.LineNumber)).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<BomViewModel>("Bom không tồn tại trong hệ thống.");

                var result = _mapper.Map<BomViewModel>(entity);

                return new Response<BomViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<BomViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<BomViewModel>>> GetPage(BomQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_Bom.AsNoTracking().Include(x => x.Product).Include(x => x.Materials.OrderBy(x => x.LineNumber)).Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<BomViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<BomViewModel>>(ex);
            }
        }

        private Expression<Func<sm_Bom, bool>> BuildQuery(BomQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_Bom>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate = predicate.And(s => s.Product.Code.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.Product.Name.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date.AddDays(1).AddTicks(-1));
            }

            return predicate;
        }

        public async Task<Response<BomViewModel>> Update(Guid id, BomCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Bom.Include(x => x.Materials).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<BomViewModel>(string.Format("Bom không tồn tại trong hệ thống!"));

                List<sm_Product> allBomMaterials = new List<sm_Product>();

                #region Validate model

                // Validate product
                if (!await _dbContext.sm_Product.AnyAsync(x => x.Id == model.ProductId))
                    return Helper.CreateBadRequestResponse<BomViewModel>("Sản phẩm không tồn tại trong hệ thống.");

                // Validate Materials Item Model
                // 1.Materials ton tai trong he thong
                // 2.Gia nhap vao khong duoc< 0
                // 3.So luong khong duoc < 0
                // 4.Product phai duoc cho phep ban
                // 6.Khong duoc duplicate dong trong Product Item
                if (model.Materials != null && model.Materials.Count > 0)
                {
                    // Fill tat ca products
                    var allMaterialIds = model.Materials.Select(x => x.MaterialId).ToList();
                    allBomMaterials = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allMaterialIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Khong duoc duplicate dong trong Materials Item
                    // Neu so luong distinct product id khac so luong product item => co dong trung
                    if (model.Materials.Count !=
                        model.Materials.Select(x => x.MaterialId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<BomViewModel>
                                    ("Danh sách nguyên vật liệu không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.Materials.Any(x => x.Quantity < 0))
                        return Helper.CreateBadRequestResponse<BomViewModel>
                                    ("Danh sách nguyên vật liệu không được có số lượng nhỏ hơn 0.");

                    if (model.Materials.Any(x => x.UnitPrice < 0))
                        return Helper.CreateBadRequestResponse<BomViewModel>
                                    ("Danh sách nguyên vật liệu không được có đơn giá nhỏ hơn 0.");

                    foreach (var item in model.Materials)
                    {
                        var product = allBomMaterials
                                            .Where(x => x.Id == item.MaterialId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<BomViewModel>
                                    ($"Danh sách nguyên vật liệu có nguyên vật liệu không tồn tại, id: {item.MaterialId}");

                        if (product.IsOrder == false)
                            return Helper.CreateBadRequestResponse<BomViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không cho phép bán");
                    }
                }
                #endregion

                #region Fill Item's Line Number

                if (model.Materials != null && model.Materials.Count > 0)
                {
                    model.Materials = model.Materials.OrderBy(x => x.LineNumber).ToList();
                    for (int i = 0; i < model.Materials.Count; i++)
                        model.Materials[i].LineNumber = i + 1;
                }
                #endregion

                #region Fill Item's Id
                if (model.ListOtherCost != null && model.ListOtherCost.Count > 0)
                {
                    for (int i = 0; i < model.ListOtherCost.Count; i++)
                    {
                        model.ListOtherCost[i].Id = Guid.NewGuid();
                    }
                }
                #endregion

                // Cập nhật các thông tin
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = $"{currentUser.FullName} ({currentUser.UserName})";
                entity.LastModifiedOnDate = DateTime.Now;

                entity.ProductId = model.ProductId;
                entity.ListOtherCost = model.ListOtherCost;
                entity.Note = model.Note;

                // Remove Old Quotation Item -> Re-add
                _dbContext.RemoveRange(entity.Materials);
                entity.Materials = new List<sm_Materials>();

                foreach (var modelItem in model.Materials)
                {
                    // Readd new item
                    var item = _mapper.Map<sm_Materials>(modelItem);
                    entity.Materials.Add(item);

                    item.BomId = entity.Id;

                    var product = allBomMaterials
                        .Where(x => x.Id == modelItem.MaterialId)
                        .FirstOrDefault();

                    item.MaterialCode = product.Code;
                    item.MaterialName = product.Name;
                    item.MaterialUnit = product.Unit;
                }

                #region Tính toán nguyên vật liệu
                // Fill product & tính discount từng line
                foreach (var item in entity.Materials)
                {
                    var product = allBomMaterials
                        .Where(x => x.Id == item.MaterialId)
                        .FirstOrDefault();

                    item.LineAmount = item.Quantity * item.UnitPrice;
                    item.LineVatPercent = product.IsVATApplied ? product.ExportVATPercent : 0M;
                    item.LineVatAmount = item.LineAmount * item.LineVatPercent / 100;
                }

                // B3. Thêm lại item sau khi tính toán
                foreach (var item in entity.Materials)
                {
                    _dbContext.sm_Materials.Add(item);
                }
                #endregion

                #region Amount & Total Quantity
                entity.TotalQuantity = entity.Materials.Sum(x => x.Quantity);
                // SubTotal = Tổng tiền của từng line
                entity.SubTotalAmount = entity.Materials.Sum(x => x.LineAmount);
                // Số tiền VAT
                entity.TotalVatAmount = entity.Materials.Sum(x => x.LineVatAmount);
                entity.TotalOtherExpenses = entity.ListOtherCost.Sum(x => x.FeeCost ?? 0);
                entity.TotalAmount = entity.SubTotalAmount + entity.TotalVatAmount + entity.TotalOtherExpenses;
                #endregion

                _dbContext.sm_Bom.Update(entity);

                await _dbContext.SaveChangesAsync();

                var result = await GetById(entity.Id);

                if (result.IsSuccess)
                    result.Message = "Chỉnh sửa thành công";

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<BomViewModel>(ex);
            }
        }

        /// <summary>
        /// Generate new Bom code
        /// </summary>
        /// <param name="defaultPrefix"></param>
        /// <returns></returns>
        public async Task<string> GenerateNewBomCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_Bom.AsNoTracking().Where(x => x.Code.Contains(code)).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

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
    }
}
