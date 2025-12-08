using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using NSPC.Common;
using NSPC.Data.Data;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using NSPC.Business.Services.StockTransaction;
using NSPC.Business.Services.VatTu;
using NSPC.Data.Data.Entity.Quotation;
using NSPC.Data.Data.Entity.VatTu;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Data.Entity;
using static NSPC.Common.Helper;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Business.Services.InventoryNote;
using static NSPC.Common.OrderReturnConstants;
using System.Data.Entity.Core.Metadata.Edm;
using MongoDB.Driver.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using NSPC.Data.Data.Entity.StockTransaction;
using NSPC.Business.Services.ConstructionActitvityLog;

namespace NSPC.Business.Services
{
    // Service Api Chức năng bán hàng
    public class SalesOrderHandler : ISalesOrderHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        // private readonly string _staticsFolder;
        private readonly ICashbookTransactionHandler _cashbookTransactionHandler;
        private readonly IStockTransactionHandler _stockTransactionHandler;
        // private readonly IVatTuHandler _vatTuHandler;
        private readonly IDebtTransactionHandler _debtTransactionHandler;
        private readonly IInventoryNoteHandler _inventoryNoteHandler;
        private readonly string _staticsFolder;
        private readonly IConstructionActivityLogHandler _constructionActivityLogHandler;
        public SalesOrderHandler(SMDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ICashbookTransactionHandler cashbookTransactionHandler,
            IStockTransactionHandler stockTransactionHandler,
            // IVatTuHandler vatTuHandler,
            IDebtTransactionHandler debtTransactionHandler, 
            IInventoryNoteHandler inventoryNoteHandler,
            IConstructionActivityLogHandler constructionActivityLogHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            // _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
            _cashbookTransactionHandler = cashbookTransactionHandler;
            _stockTransactionHandler = stockTransactionHandler;
            // _vatTuHandler = vatTuHandler;
            _debtTransactionHandler = debtTransactionHandler;
            _inventoryNoteHandler = inventoryNoteHandler;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
            _constructionActivityLogHandler = constructionActivityLogHandler;
        }
        public async Task<Response<SalesOrderViewModel>> Create(SalesOrderCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                // Check mã phiếu đã tồn tại với các phiếu khác hay chưa
                if (_dbContext.sm_SalesOrder.Any(x => x.OrderCode == model.OrderCode))
                    return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                        (string.Format("Mã phiếu {0} đã tồn tại!", model.OrderCode));
                
                sm_Quotation quotation = null;
                List<sm_Product> allSalesOrderProducts = new List<sm_Product>();

                #region Validate

                // Validate Quotation
                if (model.QuotationId.HasValue)
                {
                    quotation = await _dbContext.sm_Quotation
                                    .AsNoTracking()
                                    .Include(x => x.QuotationItem)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Id == model.QuotationId.Value);
                    if (quotation == null)
                    {
                        return Helper.CreateNotFoundResponse<SalesOrderViewModel>($"Quotation Id: {model.QuotationId} not found.");
                    }
                }

                // Validate Customer
                if (!await _dbContext.sm_Customer.AnyAsync(x => x.Id == model.CustomerId))
                {
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>
                        ($"Customer Id: {model.CustomerId} not found.");
                }

                //Validate Product Item Model
                //1.Product ton tai trong he thong
                //2.Gia nhap vao khong duoc< 0
                //3.So luong khong duoc < 0
                //4.Product phai duoc cho phep ban
                //5.Ton kho phai du so luong
                //6.Khong duoc duplicate dong trong Product Item

                if (model.SalesOrderItems != null && model.SalesOrderItems.Count > 0)
                {
                    // Fill tat ca products
                    var allProductIds = model.SalesOrderItems.Select(x => x.ProductId).ToList();
                    allSalesOrderProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Khong duoc duplicate dong trong Product Item
                    // Neu so luong distinct product id khac so luong product item => co dong trung
                    if (model.SalesOrderItems.Count !=
                        model.SalesOrderItems.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.SalesOrderItems.Any(x => x.Quantity < 0))
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn 0.");

                    if (model.SalesOrderItems.Any(x => x.UnitPrice < 0))
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ("Danh sách sản phẩm không được có đơn giá nhỏ hơn 0.");

                    foreach (var item in model.SalesOrderItems)
                    {
                        var product = allSalesOrderProducts
                                            .Where(x => x.Id == item.ProductId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");

                        if (product.InitialStockQuantity < item.Quantity)
                            return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không đủ tồn kho");

                        if (product.IsOrder == false)
                            return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không cho phép bán");
                    }

                    #region Check số lượng có thể bán và tồn trong kho của sản phẩm
                    foreach (var item in model.SalesOrderItems)
                    {
                        var allStockTransactions =
                            await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId, model.WareCode);
                        var productInventoryEntity =
                            await _dbContext.sm_ProductInventory.FirstOrDefaultAsync(x =>
                                x.ProductId == item.ProductId && x.WarehouseCode == model.WareCode);
                    
                        if (allStockTransactions.Data != null && allStockTransactions.Data.Count > 0)
                        {
                            if (productInventoryEntity != null)
                            {
                                if (productInventoryEntity.SellableQuantity < item.Quantity)
                                    return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                        ($"Sản phẩm {allStockTransactions.Data[0].ProductName} có số lượng bán ra vượt quá số lượng có thể bán trong kho");
                            }
                            
                            if (allStockTransactions.Data[0].StockTransactionQuantity < item.Quantity)
                                return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ($"Sản phẩm {allStockTransactions.Data[0].ProductName} không đủ tồn trong kho");
                        }
                        else
                        {
                            var nameProduct = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Name;
                            return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                ($"Sản phẩm {nameProduct} có số lượng bán ra vượt quá số lượng có thể bán trong kho");
                        }
                    }
                    #endregion
                }

                #endregion

                #region Fill Item's Line No
                if (model.SalesOrderItems != null && model.SalesOrderItems.Count > 0)
                {
                    model.SalesOrderItems = model.SalesOrderItems.OrderBy(x => x.LineNo).ToList();
                    for (int i = 0; i < model.SalesOrderItems.Count; i++)
                        model.SalesOrderItems[i].LineNo = i + 1;
                }
                #endregion

                // Mapping và tạo entity
                var entity = _mapper.Map<sm_SalesOrder>(model);
                entity.Id = Guid.NewGuid();

                if (model.OrderCode == null)
                {
                    entity.OrderCode = await GetNewCode(SalesOrderConstants.OrderCodePrefix);
                }
                
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.StatusCode = SalesOrderConstants.StatusCode.FINALIZED;
                entity.ExportStatusCode = SalesOrderConstants.ExportStatusCode.CHUA_XUAT; 
                entity.PaidAmount = 0M;
                entity.Total = 0M;
                entity.SubTotal = 0M;
                entity.RemainingAmount = 0M;
                entity.WareCode = model.WareCode;
                entity.WareName = _dbContext.sm_CodeType.FirstOrDefault(x => x.Code == model.WareCode)?.Title;

                #region Tính toán Order Items

                // Fill product & tính discount từng line
                foreach (var item in entity.SalesOrderItems)
                {
                    var product = allSalesOrderProducts
                        .FirstOrDefault(x => x.Id == item.ProductId);

                    if (product != null)
                    {
                        item.ProductCode = product.Code;
                        item.ProductName = product.Name;
                        item.Unit = product.Unit;
                    }
                    
                    // GoodsAmount: Thành tiền trước chiết khấu
                    item.GoodsAmount = item.Quantity * item.UnitPrice.GetValueOrDefault(0M);

                    // Line Discount (tính trên đơn giá)
                    switch (item.UnitPriceDiscountType)
                    {
                        // Chế độ tính discount bằng %
                        case SalesOrderConstants.DiscountType.Percent:
                            item.UnitPriceDiscountAmount = item.UnitPrice.GetValueOrDefault() * item.UnitPriceDiscountPercent.GetValueOrDefault(0M) / 100;
                            break;

                        // Chế độ tính discount bằng value
                        case SalesOrderConstants.DiscountType.Value:
                            if (item.UnitPrice.GetValueOrDefault() != 0)
                                item.UnitPriceDiscountPercent = item.UnitPriceDiscountAmount * 100 / item.UnitPrice.GetValueOrDefault();
                            else
                                item.UnitPriceDiscountPercent = 0;
                            break;
                    }

                    // DiscountedGoodsAmount: giá trị hàng hóa sau chiết khấu dòng
                    item.AfterLineDiscountGoodsAmount = item.GoodsAmount - item.UnitPriceDiscountAmount * item.Quantity;


                    // Fill QuotationUnitPrice from Quotation
                    //if (quotation != null && quotation.QuotationItem != null)
                    //{
                    //    item.QuotationUnitPrice = quotation.QuotationItem.
                    //                        FirstOrDefault(x => x.QuotationId == model.QuotationId).UnitPrice;
                    //}

                }

                /*
                 * Phân bổ ngược lại DiscountAmount của Order vào từng dòng
                 * từ đó tính ra SalesOrderDiscountAmount
                 */

                // B1. Tính tổng AfterLineDiscountGoodsAmount của toàn bộ dòng
                var totalAfterDiscountGoodsAmount = entity.SalesOrderItems
                    .Sum(x => x.AfterLineDiscountGoodsAmount);

                // Bước 2. Tính ra DiscountAmount của đơn hàng
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

                // Bước 3. Phân bổ lại từng dòng ra Order Discount bằng AfterLineDiscountGoodsAmount

                foreach (var item in entity.SalesOrderItems)
                {
                    if (totalAfterDiscountGoodsAmount != 0)
                        item.OrderDiscountAmount = entity.DiscountAmount
                                                        * item.AfterLineDiscountGoodsAmount
                                                        / totalAfterDiscountGoodsAmount;
                    else
                        item.OrderDiscountAmount = 0;

                    // Todo: item.VATPercent = product.VATPercent;
                    // Thêm phần checkbox có tính thuế cho sản phẩm và lựa chọn mức thuế
                    var product = allSalesOrderProducts
                                             .Where(x => x.Id == item.ProductId)
                                             .FirstOrDefault();

                    item.IsProductVATApplied = product.IsVATApplied;
                    item.VATCode = product.ExportVATCode;
                    item.VATPercent = product.IsVATApplied ? product.ExportVATPercent : 0M;
                    item.VATableAmount = item.AfterLineDiscountGoodsAmount - item.OrderDiscountAmount;
                    item.VATAmount = item.VATableAmount * item.VATPercent / 100;
                    item.LineAmount = item.VATableAmount + item.VATAmount;
                }
                #endregion

                #region Payment & Amount
                /* 
                 * Xu ly ListPayment
                 */
                entity.ListPayment = model.ListPayment;
                if (entity.ListPayment != null && entity.ListPayment.Count > 0)
                {
                    // Fill CreatedDate
                    entity.ListPayment.ForEach(x => x.CreateDate = entity.CreatedOnDate);

                    // Calculate Total Paid Amount
                    entity.PaidAmount = model.ListPayment.Sum(x => x.LinePaidAmount);
                }

                // SubTotal = Tổng tiền sau khi chiết khấu dòng của từng Line
                entity.SubTotal = entity.SalesOrderItems.Sum(x => x.AfterLineDiscountGoodsAmount);
                
                // Số tiền VAT
                entity.VATAmount = entity.SalesOrderItems.Sum(x => x.VATAmount);

                // Total = Tổng tiền người dùng phải trả từng Line + Phí giao hàng + Chi phí khác + Tổng VAT của các sản phẩm
                entity.Total = entity.SalesOrderItems.Sum(x => x.LineAmount) + entity.DeliveryFee + entity.OtherCostAmount;
                
                // TotalQuantity = Tổng SL Sản phẩm
                entity.TotalQuantity = entity.SalesOrderItems.Sum(x => x.Quantity);

                // Tiền cần thanh toán = Tổng tiền - Tiền đã trả
                entity.RemainingAmount = entity.Total - entity.PaidAmount;
                #endregion

                #region Validate Payment
                // Validate List Payment
                // Rules:
                // Số tiền không đươc lớn hơn tiền cần thanh toán và không được âm
                if (model.ListPayment != null && model.ListPayment.Count > 0)
                {
                    if (model.ListPayment.Any(x => x.LinePaidAmount < 0))
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                            ("Danh sách thanh toán không được có số tiền nhỏ hơn 0.");
                    
                    if (model.ListPayment.Any(x => x.LinePaidAmount == 0 || x.LinePaidAmount == null))
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                            ("Vui lòng nhập vào số tiền thanh toán");
                    
                    if (model.ListPayment.Any(x => x.LinePaidAmount > entity.Total))
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                            ("Danh sách thanh toán không được có số tiền lớn hơn số tiền cần thanh toán.");
                    
                    if (model.ListPayment.Sum(x => x.LinePaidAmount) > entity.Total)
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                            ("Số tiền thanh toán không được lớn hơn tổng tiền");

                    // Fill customer from model
                    model.ListPayment.ForEach(x => x.CustomerId = model.CustomerId);
                }
                #endregion
                
                #region Trạng thái đơn và trạng thái thanh toán
                // Đã thanh toán
                if (entity.RemainingAmount == 0)
                    entity.PaymentStatusCode = PaymentStatusConstants.DA_THANH_TOAN;
                else if (entity.PaidAmount != 0)
                    entity.PaymentStatusCode = PaymentStatusConstants.THANH_TOAN_MOT_NUA;
                else if (entity.PaidAmount == 0)
                    entity.PaymentStatusCode = PaymentStatusConstants.CHUA_THANH_TOAN;

                // Hoàn thành = Thanh toán hết và Đã xuất kho
                if (entity.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN &&
                    entity.ExportStatusCode == OriginalDocumentTypeConstants.XUAT_KHO)
                {
                    entity.StatusCode = SalesOrderConstants.StatusCode.COMPLETED;
                }

                #endregion

                #region Fill Quotation Code by QuotationId
                var quotationEntity =
                    await _dbContext.sm_Quotation.FirstOrDefaultAsync(x => x.Id == entity.QuotationId);

                if (quotationEntity != null)
                {
                    entity.QuotationCode = quotationEntity.Code;
                }
                #endregion
                
                #region Validate khi báo giá hết hạn
                if (quotationEntity != null)
                {
                    if (entity.CreatedOnDate > quotationEntity.DueDate)
                    {
                        return CreateBadRequestResponse<SalesOrderViewModel>($"Báo giá {quotationEntity.Code} đã hết hạn, " +
                                                                             $"vui lòng gia hạn hoặc chọn báo giá khác!");
                    }
                }
                #endregion

                _dbContext.sm_SalesOrder.Add(entity);

                var createResult = await _dbContext.SaveChangesAsync();

                /*
                 * Thêm mới các phiếu thu tự động vào Sổ quỹ
                 * 
                 */
                #region Thêm Phiếu thu vào Sổ quỹ
                if (createResult > 0 && _cashbookTransactionHandler != null)
                {
                    #region Thêm phiếu thu vào sổ quỹ
                    // Điền tên customer vào sổ quỹ
                    var customer = await _dbContext.sm_Customer.AsNoTracking()
                        .Where(x => x.Id == entity.CustomerId)
                        .Select(x => new KhachHangViewModel { Id = x.Id, Name = x.Name, Code = x.Code })
                        .FirstOrDefaultAsync();

                    if (entity.ListPayment != null && entity.ListPayment.Count() != 0)
                    {
                        foreach (var item in entity.ListPayment)
                        {
                            await _cashbookTransactionHandler.Create(new CashbookTransactionCreateUpdateModel()
                            {
                                EntityId = customer.Id,
                                EntityCode = customer.Code,
                                OriginalDocumentId = entity.Id,
                                ConstructionId = entity.ConstructionId,
                                ContractId = entity.ContractId,
                                OriginalDocumentType = OriginDocumentCashbookTransactionConstants.SALES_ORDER,
                                OriginalDocumentCode = entity.OrderCode,
                                ProjectId = entity.ProjectId,
                                TransactionTypeCode = CashbookTransactionConstants.ReceiptVoucherType,
                                PurposeCode = CashbookTransactionConstants.AUTO_RECEIPT_PURPOSE,
                                PaymentMethodCode = item.PaymentMethod,
                                Amount = item.LinePaidAmount ?? 0,
                                EntityName = customer?.Name,
                                ReceiptDate = entity.CreatedOnDate,
                                EntityTypeCode = EntityTypeCodeConstants.CUSTOMER,
                                EntityTypeName = EntityTypeNameConstants.CUSTOMER,
                                Description = $"Phiếu thu tự động tạo khi khách hàng thanh toán cho đơn mua hàng {entity.OrderCode}",
                                IsDebt = true
                            }, currentUser);
                        }
                    }
                    #endregion
                    
                    #region Log lại hoạt động thêm mới phiếu xuất kho vào bảng sm_ConstructionActivityLog
                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã tạo đơn bán hàng",
                        CodeLinkDescription = $"{entity.OrderCode} - {customer.Name}",
                        OrderId = entity.Id,
                        ConstructionId = entity.ConstructionId,
                    }, currentUser);
                    #endregion
                    
                    
                    #region Cập nhật tổng chi tiêu và Tổng đơn hàng của khách hàng
                    var customerEntity = await _dbContext.sm_Customer.Where(x => x.Id == entity.CustomerId).FirstOrDefaultAsync();
                
                    var salesOrdersHasDone = _dbContext.sm_SalesOrder
                        .Where(x => x.CustomerId == entity.CustomerId && x.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN).ToList();
                
                    var salesOrdersHasHalf =  _dbContext.sm_SalesOrder
                        .Where(x => x.CustomerId == entity.CustomerId && x.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA).ToList();
                
                    if (customerEntity != null)
                    {
                        customerEntity.OrderCount = salesOrdersHasDone.Count + salesOrdersHasHalf.Count;
                        customerEntity.ExpenseAmount = salesOrdersHasDone.Sum(x => x.PaidAmount ?? 0) + salesOrdersHasHalf.Sum(x => x.PaidAmount ?? 0);
                        await _dbContext.SaveChangesAsync();
                    }
                    #endregion

                    #region Cập nhật lại số lượng có thể bán
                    foreach (var item in entity.SalesOrderItems)
                    {
                        // var productItem = await _dbContext.sm_Stock_Transaction
                        //     .Where(x => x.ProductId == item.ProductId && x.WareCode == entity.WareCode)
                        //     .OrderByDescending(x => x.CreatedOnDate)
                        //     .FirstOrDefaultAsync();
                        // var allStockTransactions =
                        //     await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId, entity.WareCode);
                        var productEntity =
                            await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                        var productInventoryEntity = await _dbContext.sm_ProductInventory
                            .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseCode == entity.WareCode);

                        if (productInventoryEntity != null)
                        {
                            productInventoryEntity.SellableQuantity -= item.Quantity;
                            productEntity.SellableQuantity -= item.Quantity;
                            await _dbContext.SaveChangesAsync(); 
                        }
                    }
                    #endregion
                }
                #endregion
                var result = await GetById(entity.Id);
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<SalesOrderViewModel>(result.Data, "Tạo đơn bán hàng thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<SalesOrderViewModel>(ex);
            }
        }

        /// <summary>
        /// Cập nhật đơn hàng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response<SalesOrderViewModel>> Update(Guid id, SalesOrderCreateUpdateModel model, RequestUser currentUser)
        {
            /*
             * RULE
             * Chỉ được cập nhật khi chưa có thanh toán, nếu đơn đã thanh toán rồi thì 
             * dùng API charge-payment để xử lý tiếp
             * Chỉ được cập nhật khi chưa có nhập hàng
             */

            try
            {
                // Select và validate entity
                var entity = await _dbContext.sm_SalesOrder
                                    .Include(x => x.SalesOrderItems)
                                    .ThenInclude(x => x.sm_Product)
                                    .Include(x => x.sm_Customer)
                                    .Include(x => x.mk_DuAn)
                                    .Include(x => x.sm_Construction)
                                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>
                        ($"Đơn hàng không tồn tại trong hệ thống, id: {id}");


                sm_Quotation quotation = null;
                List<sm_Product> allSalesOrderProducts = new List<sm_Product>();

                #region Validate model

                // Validate Quotation
                // Khi update được chọn Quotation khác nên cần validate lại
                if (model.QuotationId.HasValue)
                {
                    quotation = await _dbContext.sm_Quotation
                                    .AsNoTracking()
                                    .Include(x => x.QuotationItem)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Id == model.QuotationId.Value);
                    if (quotation == null)
                    {
                        return Helper.CreateNotFoundResponse<SalesOrderViewModel>($"Quotation Id: {model.QuotationId} not found.");
                    }
                }

                // Validate Customer
                if (!await _dbContext.sm_Customer.AnyAsync(x => x.Id == model.CustomerId))
                {
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>
                        ($"Customer Id: {model.CustomerId} not found.");
                }
                
                
                // Validate Proruct Item Model
                // 1.Product ton tai trong he thong
                // 2.Gia nhap vao khong duoc< 0
                // 3.So luong khong duoc < 0
                // 4.Product phai duoc cho phep ban
                // 5.Ton kho phai du so luong
                // 6.Khong duoc duplicate dong trong Product Item
                if (model.SalesOrderItems != null && model.SalesOrderItems.Count > 0)
                {
                    // Fill tat ca products
                    var allProductIds = model.SalesOrderItems.Select(x => x.ProductId).ToList();
                    allSalesOrderProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Khong duoc duplicate dong trong Product Item
                    // Neu so luong distinct product id khac so luong product item => co dong trung
                    if (model.SalesOrderItems.Count !=
                        model.SalesOrderItems.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.SalesOrderItems.Any(x => x.Quantity < 0))
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn 0.");

                    if (model.SalesOrderItems.Any(x => x.UnitPrice < 0))
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ("Danh sách sản phẩm không được có đơn giá nhỏ hơn 0.");

                    foreach (var item in model.SalesOrderItems)
                    {
                        var product = allSalesOrderProducts
                                            .Where(x => x.Id == item.ProductId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");

                        if (product.InitialStockQuantity < item.Quantity)
                            return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không đủ tồn kho");

                        if (product.IsOrder == false)
                            return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không cho phép bán");
                    }
                    
                    #region Check số lượng có thể bán và tồn trong kho của sản phẩm
                    foreach (var item in model.SalesOrderItems)
                    {
                        var allStockTransactions =
                            await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId, model.WareCode);
                        var productInventoryEntity =
                            await _dbContext.sm_ProductInventory.FirstOrDefaultAsync(x =>
                                x.ProductId == item.ProductId && x.WarehouseCode == model.WareCode);
                    
                        if (allStockTransactions.Data != null && allStockTransactions.Data.Count > 0)
                        {
                            if (productInventoryEntity != null)
                            {
                                if (productInventoryEntity.SellableQuantity < item.Quantity)
                                    return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                        ($"Sản phẩm {allStockTransactions.Data[0].ProductName} có số lượng bán ra vượt quá số lượng có thể bán trong kho");
                            }
                            
                            if (allStockTransactions.Data[0].StockTransactionQuantity < item.Quantity)
                                return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ($"Sản phẩm {allStockTransactions.Data[0].ProductName} không đủ tồn trong kho");
                        }
                        else
                        {
                            var nameProduct = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Name;
                            return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                ($"Sản phẩm {nameProduct} có số lượng bán ra vượt quá số lượng có thể bán trong kho");
                        }
                    }
                    #endregion
                }
                #endregion

                #region Validate Payment & Receipt Status (trạng thái thanh toán & nhập hàng)
                // Nếu đơn đã có thanh toán thì không cho cập nhật nữa
                if (entity.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA ||
                    entity.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN)
                    return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ("Đơn hàng đã có thanh toán, không thể cập nhật.");

                // Nếu đơn đã được nhập kho thì không cho cập nhật
                if (entity.ExportStatusCode == RecieveStatus.DA_NHAP)
                    return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ("Đơn hàng đã được nhập kho, không thể cập nhật.");

                #endregion

                #region Fill Item's Line No

                if (model.SalesOrderItems != null && model.SalesOrderItems.Count > 0)
                {
                    model.SalesOrderItems = model.SalesOrderItems.OrderBy(x => x.LineNo).ToList();
                    for (int i = 0; i < model.SalesOrderItems.Count; i++)
                        model.SalesOrderItems[i].LineNo = i + 1;
                }
                #endregion

                // Cập nhật các thông tin
                // Không cập nhật StatusCode, PaymentStatusCode
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = $"{currentUser.FullName} ({currentUser.UserName})";
                entity.LastModifiedOnDate = DateTime.Now;

                if (model.OrderCode == null)
                {
                    entity.OrderCode = entity.OrderCode;
                }
                else
                {
                    entity.OrderCode = model.OrderCode;
                }
                
                entity.QuotationId = model.QuotationId;
                entity.PaidAmount = 0M;
                entity.Total = 0M;
                entity.SubTotal = 0M;
                entity.RemainingAmount = 0M;
                entity.DiscountPercent = model.DiscountPercent;
                entity.DiscountAmount = model.DiscountAmount;
                entity.DiscountType = model.DiscountType;
                entity.DeliveryFee = model.DeliveryFee;
                entity.WareCode = model.WareCode;
                entity.WareName = _dbContext.sm_CodeType.FirstOrDefault(x => x.Code == model.WareCode)?.Title;
                entity.Tags = model.Tags;
                entity.Note = model.Note;
                entity.CustomerId = model.CustomerId;
                entity.DeliveryFee = model.DeliveryFee;
                entity.OtherCostAmount = model.OtherCostAmount;
                entity.PaymentMethodCode = model.PaymentMethodCode;
                entity.ProjectId = model.ProjectId;

                #region Fill Quotation Code by QuotationId
                var quotationEntity =
                    await _dbContext.sm_Quotation.FirstOrDefaultAsync(x => x.Id == entity.QuotationId);

                if (quotationEntity != null)
                {
                    entity.QuotationCode = quotationEntity.Code;
                }
                #endregion 

                // Remove Old Sales Order Item -> Re-add
                _dbContext.RemoveRange(entity.SalesOrderItems);
                entity.SalesOrderItems = new List<sm_SalesOrderItem>();

                #region Điền thông tin Product, GoodsAmount, Line Discount

                // Fill Product Data, GoodsAmount, DiscountAmount
                foreach (var modelItem in model.SalesOrderItems)
                {
                    // Readd new item
                    var item = _mapper.Map<sm_SalesOrderItem>(modelItem);
                    entity.SalesOrderItems.Add(item);

                    // Assign SalesOrderId
                    item.SalesOrderId = entity.Id;

                    // Fill product data
                    var product = allSalesOrderProducts
                                    .Where(x => x.Id == modelItem.ProductId)
                                    .FirstOrDefault();

                    item.ProductCode = product.Code;
                    item.ProductName = product.Name;
                    item.Unit = product.Unit;

                    // GoodsAmount: giá trị hàng hóa
                    item.GoodsAmount = item.Quantity * item.UnitPrice.GetValueOrDefault(0M);

                    // Line Discount (tính trên đơn giá)
                    switch (item.UnitPriceDiscountType)
                    {
                        // Chế độ tính discount bằng %
                        case SalesOrderConstants.DiscountType.Percent:
                            item.UnitPriceDiscountAmount = item.UnitPrice.GetValueOrDefault() * item.UnitPriceDiscountPercent.GetValueOrDefault(0M) / 100;
                            break;

                        // Chế độ tính discount bằng value
                        case SalesOrderConstants.DiscountType.Value:
                            if (item.UnitPrice.GetValueOrDefault() != 0)
                                item.UnitPriceDiscountPercent = item.UnitPriceDiscountAmount * 100 / item.UnitPrice.GetValueOrDefault();
                            else
                                item.UnitPriceDiscountPercent = 0;
                            break;
                    }

                    // DiscountedGoodsAmount: giá trị hàng hóa sau chiết khấu dòng
                    item.AfterLineDiscountGoodsAmount = item.GoodsAmount - item.UnitPriceDiscountAmount * item.Quantity;


                    // Fill QuotationUnitPrice from Quotation for each iteration
                    //if (quotation != null && quotation.QuotationItem != null)
                    //{
                    //    item.QuotationUnitPrice = quotation.QuotationItem.
                    //                        FirstOrDefault(x => x.QuotationId == model.QuotationId).UnitPrice;
                    //}
                }

                /*
                    * Phân bổ ngược lại DiscountAmount của Order vào từng dòng
                    * từ đó tính ra SalesOrderDiscountAmount
                    */

                // B1. Tính tổng AfterLineDiscountGoodsAmount của toàn bộ dòng
                var totalAfterDiscountGoodsAmount = entity.SalesOrderItems
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
                foreach (var item in entity.SalesOrderItems)
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
                    var product = allSalesOrderProducts
                                    .Where(x => x.Id == item.ProductId)
                                    .FirstOrDefault();

                    item.IsProductVATApplied = product.IsVATApplied;
                    item.VATCode = product.ExportVATCode;
                    item.VATPercent = product.IsVATApplied ? product.ExportVATPercent : 0M;
                    item.VATableAmount = item.AfterLineDiscountGoodsAmount - item.OrderDiscountAmount;
                    item.VATAmount = item.VATableAmount * item.VATPercent / 100;
                    item.LineAmount = item.VATableAmount + item.VATAmount;
                }

                // B3. Thêm lại item sau khi tính toán
                foreach (var item in entity.SalesOrderItems)
                {
                    _dbContext.sm_SalesOrderItem.Add(item);
                }

                #endregion

                #region Payment, Total, SubTotal, VATAmount, Paid, Remaining Amount

                /* 
                 * Xu ly ListPayment
                 */
                entity.ListPayment = model.ListPayment;
                if (entity.ListPayment != null && entity.ListPayment.Count > 0)
                {
                    // Fill CreatedDate
                    entity.ListPayment.ForEach(x => x.CreateDate = entity.CreatedOnDate);

                    // Calculate Total Paid Amount
                    entity.PaidAmount = model.ListPayment.Sum(x => x.LinePaidAmount);
                }

                // SubTotal = Tổng tiền sau khi chiết khấu dòng của từng Line
                entity.SubTotal = entity.SalesOrderItems.Sum(x => x.AfterLineDiscountGoodsAmount);

                // Total = Tổng tiền người dùng phải trả từng Line + Phí giao hàng + Chi phí khác
                entity.Total = entity.SalesOrderItems.Sum(x => x.LineAmount) + entity.DeliveryFee + entity.OtherCostAmount;
                
                // TotalQuantity = Tổng SL Sản phẩm
                entity.TotalQuantity = entity.SalesOrderItems.Sum(x => x.Quantity);

                // Số tiền VAT
                entity.VATAmount = entity.SalesOrderItems.Sum(x => x.VATAmount);

                // Tiền cần thanh toán = Tổng tiền - Tiền đã trả
                entity.RemainingAmount = entity.Total - entity.PaidAmount;

                // Tổng tiền giảm giá cho đơn
                entity.TotalDiscountAmount = entity.DiscountAmount +
                    entity.SalesOrderItems.Sum(x => x.GoodsAmount - x.AfterLineDiscountGoodsAmount);
                #endregion
                
                #region Validate Payment
                // Validate List Payment
                // Rules:
                // Số tiền không đươc lớn hơn tiền cần thanh toán và không được âm
                if (model.ListPayment != null && model.ListPayment.Count > 0)
                {
                    if (model.ListPayment.Any(x => x.LinePaidAmount < 0))
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>("Danh sách thanh toán không được có số tiền nhỏ hơn 0.");
                            
                    if (model.ListPayment.Any(x => x.LinePaidAmount == 0 || x.LinePaidAmount == null))
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>("Vui lòng nhập số tiền thanh toán");
                    
                    if (model.ListPayment.Any(x => x.LinePaidAmount > entity.Total))
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                            ("Danh sách thanh toán không được có số tiền lớn hơn số tiền cần thanh toán.");
                    
                    if (model.ListPayment.Sum(x => x.LinePaidAmount) > entity.Total)
                        return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                            ("Số tiền thanh toán không được lớn hơn tổng tiền");

                    // Fill customer from model
                    model.ListPayment.ForEach(x => x.CustomerId = model.CustomerId);
                }
                #endregion

                #region Trạng thái đơn và trạng thái thanh toán
                // Đã thanh toán
                if (entity.RemainingAmount == 0)
                    entity.PaymentStatusCode = PaymentStatusConstants.DA_THANH_TOAN;
                else if (entity.PaidAmount != 0)
                    entity.PaymentStatusCode = PaymentStatusConstants.THANH_TOAN_MOT_NUA;
                else if (entity.PaidAmount == 0)
                    entity.PaymentStatusCode = PaymentStatusConstants.CHUA_THANH_TOAN;

                // Hoàn thành = Thanh toán hết và Đã xuất kho
                if (entity.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN &&
                    entity.ExportStatusCode == OriginalDocumentTypeConstants.XUAT_KHO)
                {
                    entity.StatusCode = SalesOrderConstants.StatusCode.COMPLETED;
                }

                #endregion
                
                #region Validate khi báo giá hết hạn
                if (quotationEntity != null)
                {
                    if (entity.CreatedOnDate > quotationEntity.DueDate)
                    {
                        return CreateBadRequestResponse<SalesOrderViewModel>($"Báo giá {quotationEntity.Code} đã hết hạn, " +
                                                                             $"vui lòng gia hạn hoặc chọn báo giá khác!");
                    }
                }
                #endregion

                _dbContext.sm_SalesOrder.Update(entity);

                var createResult = await _dbContext.SaveChangesAsync();

                #region Cashbook Transaction - Sổ quỹ
                if (createResult > 0)
                {
                    // Điền tên customer vào sổ quỹ
                    var customer = await _dbContext.sm_Customer.AsNoTracking()
                                        .Where(x => x.Id == entity.CustomerId)
                                        .Select(x => new KhachHangViewModel { Id = x.Id, Name = x.Name, Code = x.Code })
                                        .FirstOrDefaultAsync();

                    if (entity.ListPayment != null && entity.ListPayment.Count() != 0)
                    {
                        foreach (var item in entity.ListPayment)
                        {
                            await _cashbookTransactionHandler.Create(new CashbookTransactionCreateUpdateModel()
                            {
                                EntityId = customer.Id,
                                EntityCode = customer.Code,
                                OriginalDocumentId = entity.Id,
                                ConstructionId = entity.ConstructionId,
                                OriginalDocumentType = OriginDocumentCashbookTransactionConstants.SALES_ORDER,
                                OriginalDocumentCode = entity.OrderCode,
                                TransactionTypeCode = CashbookTransactionConstants.ReceiptVoucherType,
                                PurposeCode = CashbookTransactionConstants.AUTO_RECEIPT_PURPOSE,
                                PaymentMethodCode = item.PaymentMethod,
                                Amount = item.LinePaidAmount ?? 0,
                                EntityName = customer?.Name,
                                ReceiptDate = entity.CreatedOnDate,
                                EntityTypeCode = EntityTypeCodeConstants.CUSTOMER,
                                EntityTypeName = EntityTypeNameConstants.CUSTOMER,
                                Description = $"Phiếu thu tự động tạo khi khách hàng thanh toán cho đơn mua hàng {entity.OrderCode}",
                                IsDebt = true
                            }, currentUser);
                        }
                    }
                }
                #endregion

                var result = await GetById(entity.Id);
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<SalesOrderViewModel>(result.Data, "Sửa đơn bán hàng thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<SalesOrderViewModel>(ex);
            }
        }


        public async Task<Response<SalesOrderViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_SalesOrder
                                    .Include(x => x.SalesOrderItems)
                                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>(string.Format("Đơn bán hàng không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<SalesOrderViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<SalesOrderViewModel>(ex);
            }
        }

        public async Task<Response<string>> ExportExcelList()
        {
            try
            {
                var allSalesOrders = await _dbContext.sm_SalesOrder.AsNoTracking()
                    .Include(x => x.SalesOrderItems)
                    .Include(c => c.sm_Customer)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .ToListAsync();

                if (allSalesOrders == null || allSalesOrders.Count == 0)
                {
                    return Helper.CreateNotFoundResponse<string>("Không có đơn nào tồn tại trong hệ thống.");
                }

                var fileName = $"SalesOrder_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}.xlsx";
                var filePath = Path.Combine(_staticsFolder, fileName);
                var templatePath = Path.Combine(_staticsFolder + "excel-template/SalesOrderTemplate.xlsx");

                if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                {
                    return Helper.CreateBadRequestResponse<string>("Không tìm thấy file template");
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(templatePath)))
                {
                    var ws = package.Workbook.Worksheets[0];
                    ws.Column(1).Width = 8;
                    int startRow = 4;
                    int index = 1;
                    foreach (var salesOrder in allSalesOrders)
                    {
                        ws.Cells[startRow, 1].Value = index;
                        ws.Cells[startRow, 2].Value = salesOrder.CreatedOnDate.ToString("dd/MM/yyyy");
                        ws.Cells[startRow, 3].Value = salesOrder.LastModifiedOnDate?.ToString("dd/MM/yyyy");
                        ws.Cells[startRow, 4].Value = salesOrder.OrderCode;
                        
                        // Kiểm tra trạng thái và gán tên trạng thái
                        string paymentStatusName;
                        switch (salesOrder.PaymentStatusCode)
                        {
                            case PaymentStatusConstants.DA_THANH_TOAN:
                                paymentStatusName = "Đã thanh toán";
                                break;
                            case PaymentStatusConstants.THANH_TOAN_MOT_NUA:
                                paymentStatusName = "Thanh toán một nửa";
                                break;
                            case PaymentStatusConstants.CHUA_THANH_TOAN:
                                paymentStatusName = "Chưa thanh toán";
                                break;
                            default:
                                paymentStatusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }
                        
                        // Kiểm tra trạng thái giao dịch và gán tên trạng thái
                        string statusName;
                        switch (salesOrder.StatusCode)
                        {
                            case SalesOrderConstants.StatusCode.FINALIZED:
                                statusName = "Đang giao dịch";
                                break;
                            case SalesOrderConstants.StatusCode.COMPLETED:
                                statusName = "Đã hoàn thành";
                                break;
                            case SalesOrderConstants.StatusCode.CANCELLED:
                                statusName = "Đã hủy";
                                break;
                            default:
                                statusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }
                        
                        // Kiểm tra trạng thái giao dịch và gán tên trạng thái
                        string exportStatusName;
                        switch (salesOrder.ExportStatusCode)
                        {
                            case SalesOrderConstants.ExportStatusCode.CHUA_XUAT:
                                exportStatusName = "Chưa xuất";
                                break;
                            case OriginalDocumentTypeConstants.XUAT_KHO:
                                exportStatusName = "Đã xuất";
                                break;
                            default:
                                exportStatusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }
                        
                        ws.Cells[startRow, 5].Value = statusName;
                        ws.Cells[startRow, 6].Value = exportStatusName;
                        ws.Cells[startRow, 7].Value = paymentStatusName;
                        ws.Cells[startRow, 8].Value = salesOrder.Total;
                        ws.Cells[startRow, 9].Value = CodeTypeCollection.Instance
                            .FetchCode(salesOrder.WareCode, LanguageConstants.Default, salesOrder.TenantId)?.Title ?? null;
                        ws.Cells[startRow, 10].Value = CodeTypeCollection.Instance
                            .FetchCode(salesOrder.PaymentMethodCode, LanguageConstants.Default, salesOrder.TenantId)?.Title ?? null;
                        ws.Cells[startRow, 11].Value = salesOrder.CreatedByUserName;
                        ws.Cells[startRow, 12].Value = salesOrder.LastModifiedByUserName;
                        ws.Cells[startRow, 13].Value = salesOrder.sm_Customer?.Code;
                        ws.Cells[startRow, 14].Value = salesOrder.sm_Customer?.Name;
                        ws.Cells[startRow, 12].Value = $"{salesOrder.sm_Customer?.Address}" + $"{(string.IsNullOrEmpty(salesOrder.sm_Customer?.WardName) ? "" : $", {salesOrder.sm_Customer?.WardName}")}" +
                                                              $"{(string.IsNullOrEmpty(salesOrder.sm_Customer?.DistrictName) ? "" : $", {salesOrder.sm_Customer?.DistrictName}")}" +
                                                              $"{(string.IsNullOrEmpty(salesOrder.sm_Customer?.ProvinceName) ? "" : $", {salesOrder.sm_Customer?.ProvinceName}")}";
                        ws.Cells[startRow, 16].Value = salesOrder.sm_Customer?.PhoneNumber;
                        ws.Cells[startRow, 17].Value = salesOrder.QuotationCode;
                        ws.Cells[startRow, 18].Value = salesOrder.Note;

                        startRow++;
                        index++;
                    }
                    int lastDataRow = startRow - 1;
                    using (var range = ws.Cells[4, 1, lastDataRow, 18]) 
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    
                    int totalRow = ws.Dimension.End.Row;
                    if (totalRow > lastDataRow)
                    {
                        ws.DeleteRow(lastDataRow + 1, totalRow - lastDataRow);
                    }
                    
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    await package.SaveAsAsync(new FileInfo(filePath));
                }

                return Helper.CreateSuccessResponse<string>(filePath, "Xuất file thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<string>(ex);
            }
        }

        public async Task<Response<string>> ExportExcelListCurrentPage(SalesOrderQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var salesOrders = await _dbContext.sm_SalesOrder.AsNoTracking()
                    .Include(c => c.SalesOrderItems)
                    .Include(c => c.sm_Customer)
                    .Where(predicate)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .GetPageAsync(query);

                if (salesOrders == null || salesOrders.Content == null || salesOrders.Content.Count == 0)
                {
                    return Helper.CreateNotFoundResponse<string>("Không có phiếu nào tồn tại trong hệ thống.");
                }
                
                var fileName = $"SalesOrder_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}.xlsx";
                var filePath = Path.Combine(_staticsFolder, fileName);
                var templatePath = Path.Combine(_staticsFolder, "excel-template/SalesOrderTemplate.xlsx");

                if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                {
                    return Helper.CreateBadRequestResponse<string>("Không tìm thấy file template");
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
              using (var package = new ExcelPackage(new FileInfo(templatePath)))
                {
                    var ws = package.Workbook.Worksheets[0];
                    ws.Column(1).Width = 8;
                    int startRow = 4;
                    int index = 1;
                    foreach (var salesOrder in salesOrders.Content)
                    {
                        ws.Cells[startRow, 1].Value = index;
                        ws.Cells[startRow, 2].Value = salesOrder.CreatedOnDate.ToString("dd/MM/yyyy");
                        ws.Cells[startRow, 3].Value = salesOrder.LastModifiedOnDate?.ToString("dd/MM/yyyy");
                        ws.Cells[startRow, 4].Value = salesOrder.OrderCode;
                        
                        // Kiểm tra trạng thái và gán tên trạng thái
                        string paymentStatusName;
                        switch (salesOrder.PaymentStatusCode)
                        {
                            case PaymentStatusConstants.DA_THANH_TOAN:
                                paymentStatusName = "Đã thanh toán";
                                break;
                            case PaymentStatusConstants.THANH_TOAN_MOT_NUA:
                                paymentStatusName = "Thanh toán một nửa";
                                break;
                            case PaymentStatusConstants.CHUA_THANH_TOAN:
                                paymentStatusName = "Chưa thanh toán";
                                break;
                            default:
                                paymentStatusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }
                        
                        // Kiểm tra trạng thái giao dịch và gán tên trạng thái
                        string statusName;
                        switch (salesOrder.StatusCode)
                        {
                            case SalesOrderConstants.StatusCode.FINALIZED:
                                statusName = "Đang giao dịch";
                                break;
                            case SalesOrderConstants.StatusCode.COMPLETED:
                                statusName = "Đã hoàn thành";
                                break;
                            case SalesOrderConstants.StatusCode.CANCELLED:
                                statusName = "Đã hủy";
                                break;
                            default:
                                statusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }
                        
                        // Kiểm tra trạng thái giao dịch và gán tên trạng thái
                        string exportStatusName;
                        switch (salesOrder.ExportStatusCode)
                        {
                            case SalesOrderConstants.ExportStatusCode.CHUA_XUAT:
                                exportStatusName = "Chưa xuất";
                                break;
                            case OriginalDocumentTypeConstants.XUAT_KHO:
                                exportStatusName = "Đã xuất";
                                break;
                            default:
                                exportStatusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }
                        
                        ws.Cells[startRow, 5].Value = statusName;
                        ws.Cells[startRow, 6].Value = exportStatusName;
                        ws.Cells[startRow, 7].Value = paymentStatusName;
                        ws.Cells[startRow, 8].Value = salesOrder.Total;
                        ws.Cells[startRow, 9].Value = CodeTypeCollection.Instance
                            .FetchCode(salesOrder.WareCode, LanguageConstants.Default, salesOrder.TenantId)?.Title ?? null;
                        ws.Cells[startRow, 10].Value = CodeTypeCollection.Instance
                            .FetchCode(salesOrder.PaymentMethodCode, LanguageConstants.Default, salesOrder.TenantId)?.Title ?? null;
                        ws.Cells[startRow, 11].Value = salesOrder.CreatedByUserName;
                        ws.Cells[startRow, 12].Value = salesOrder.LastModifiedByUserName;
                        ws.Cells[startRow, 13].Value = salesOrder.sm_Customer?.Code;
                        ws.Cells[startRow, 14].Value = salesOrder.sm_Customer?.Name;
                        ws.Cells[startRow, 12].Value = $"{salesOrder.sm_Customer?.Address}" + $"{(string.IsNullOrEmpty(salesOrder.sm_Customer?.WardName) ? "" : $", {salesOrder.sm_Customer?.WardName}")}" +
                                                       $"{(string.IsNullOrEmpty(salesOrder.sm_Customer?.DistrictName) ? "" : $", {salesOrder.sm_Customer?.DistrictName}")}" +
                                                       $"{(string.IsNullOrEmpty(salesOrder.sm_Customer?.ProvinceName) ? "" : $", {salesOrder.sm_Customer?.ProvinceName}")}";
                        ws.Cells[startRow, 16].Value = salesOrder.sm_Customer?.PhoneNumber;
                        ws.Cells[startRow, 17].Value = salesOrder.QuotationCode;
                        ws.Cells[startRow, 18].Value = salesOrder.Note;

                        startRow++;
                        index++;
                    }
                    int lastDataRow = startRow - 1;
                    
                    using (var range = ws.Cells[4, 1, lastDataRow, 16]) 
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    int totalRow = ws.Dimension.End.Row;
                    if (totalRow > lastDataRow)
                    {
                        ws.DeleteRow(lastDataRow + 1, totalRow - lastDataRow);
                    }
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
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

        public async Task<Response<SalesOrderViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_SalesOrder
                                    .AsNoTracking()
                                    .Include(x => x.SalesOrderItems)
                                    .ThenInclude(x => x.sm_Product)
                                    .Include(x => x.sm_Customer)
                                    .Include(x => x.mk_DuAn)
                                    .Include(x => x.sm_Construction)
                                    .Include(x => x.sm_Contract)
                                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>("Đơn hàng không tồn tại trong hệ thống.");

                var result = _mapper.Map<SalesOrderViewModel>(entity);

                return new Response<SalesOrderViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<SalesOrderViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<SalesOrderViewModel>>> GetPage(SalesOrderQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_SalesOrder
                                    .AsNoTracking()
                                    .Include(x => x.SalesOrderItems)
                                    .ThenInclude(x => x.sm_Product)
                                    .Include(x => x.sm_Customer)
                                    .Include(x => x.mk_DuAn)
                                    .Include(x => x.sm_Construction)
                                    .Include(x => x.sm_Contract)
                                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<SalesOrderViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<SalesOrderViewModel>>(ex);
            }
        }

        public async Task<Response<SalesOrderSummaryModel>> GetSalesOrderSummary(
            SalesOrderQueryModel query)
        {
            try
            {
                var predicate = BuildQueryTransaction(query);
                var result = new SalesOrderSummaryModel();
                
                // Lấy ra danh sách chờ xuất kho
                var exportStatusCodeList = _dbContext.sm_SalesOrder.AsNoTracking()
                    .Where(predicate.And(x => x.ExportStatusCode == SalesOrderConstants.ExportStatusCode.CHUA_XUAT && x.StatusCode != SalesOrderConstants.StatusCode.CANCELLED));
                
                // Lấy ra danh sách chờ thanh toán (Chưa thanh toán, Thanh toán một nửa)
                var paymentStatusCodePendingList = _dbContext.sm_SalesOrder.AsNoTracking().Where(predicate.And(s =>
                    s.PaymentStatusCode == PaymentStatusConstants.CHUA_THANH_TOAN &&
                    s.StatusCode != SalesOrderConstants.StatusCode.CANCELLED));
                var paymentStatusCodeNotDoneList = _dbContext.sm_SalesOrder.AsNoTracking().Where(predicate.And(s =>
                    s.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA &&
                    s.StatusCode != SalesOrderConstants.StatusCode.CANCELLED));
                
                result.PendingExportQuantity = exportStatusCodeList.Count();
                result.PendingExportAmount = exportStatusCodeList.Sum(x => x.Total);
                result.PendingPaymentQuantity =
                    paymentStatusCodePendingList.Count() + paymentStatusCodeNotDoneList.Count();
                result.PendingPaymentAmount = paymentStatusCodePendingList.Sum(x => x.RemainingAmount) +
                                              paymentStatusCodeNotDoneList.Sum(x => x.RemainingAmount);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<SalesOrderSummaryModel>(ex);
            }
        }

        private Expression<Func<sm_SalesOrder, bool>> BuildQueryTransaction(SalesOrderQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_SalesOrder>(true);
            return predicate;
        }

        private Expression<Func<sm_SalesOrder, bool>> BuildQuery(SalesOrderQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            
            var predicate = PredicateBuilder.New<sm_SalesOrder>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.OrderCode.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.sm_Customer.Name.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.sm_Customer.PhoneNumber.Contains(query.FullTextSearch)
                || s.PaymentStatusCode.Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.StatusCode))
                predicate.And(s => s.StatusCode == query.StatusCode);
            
            if (!string.IsNullOrEmpty(query.ExportStatusCode))
                predicate.And(s => s.ExportStatusCode == query.ExportStatusCode && s.StatusCode != SalesOrderConstants.StatusCode.CANCELLED);

            if (query.PaymentStatusCodes != null && query.PaymentStatusCodes.Count() > 0)
                predicate.And(s => query.PaymentStatusCodes.Contains(s.PaymentStatusCode) && s.StatusCode != SalesOrderConstants.StatusCode.CANCELLED);

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date < query.DateRange[1].Value.AddDays(1));
            }

            // User view owned orders
            if (query.CreatedByUserId.HasValue)
            {
                predicate.And(s => s.CreatedByUserId == query.CreatedByUserId);
            }

            if (query.CustomerId.HasValue)
            {
                predicate = predicate.And(x => x.CustomerId == query.CustomerId);
            }
            
            if (!currentUser.ListRights.Contains("SALESORDER." + RightActionConstants.VIEWALL))
                predicate.And(s => s.CreatedByUserId == currentUser.UserId);
            
            return predicate;
        }


        private async Task<string> GetNewCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_SalesOrder.AsNoTracking().Where(x => x.OrderCode.Contains(code)).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

                if (result != null)
                {
                    var currentNum = result.OrderCode.Substring(result.OrderCode.Length - 3, 3);
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

        public async Task<Response<SalesOrderViewModel>> RejectOrder(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_SalesOrder.Include(x => x.SalesOrderItems).FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                entity.StatusCode = SalesOrderConstants.StatusCode.CANCELLED;
                entity.LastModifiedOnDate = DateTime.Now;

                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    #region Cập nhật lại số lượng có thể bán
                    foreach (var item in entity.SalesOrderItems)
                    {
                        var productEntity =
                            await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                        var productInventoryEntity = await _dbContext.sm_ProductInventory.FirstOrDefaultAsync(x =>
                            x.ProductId == item.ProductId && x.WarehouseCode == entity.WareCode);
                        
                        if (productInventoryEntity != null)
                        {
                            productInventoryEntity.SellableQuantity += item.Quantity;

                            if (productEntity != null)
                            {
                                productEntity.SellableQuantity += item.Quantity;
                            }
                                
                            await _dbContext.SaveChangesAsync();
                        }
                        
                    }
                    #endregion
                }
                return Helper.CreateSuccessResponse(_mapper.Map<SalesOrderViewModel>(entity), "Hủy đơn bán hàng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<SalesOrderViewModel>(ex);
            }
        }

        public async Task<Response<SalesOrderViewModel>> ChargePaymentOrder(Guid id, SalesOrderCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_SalesOrder
                    .Include(x => x.sm_Construction)
                    .Include(x => x.sm_Contract)
                    .Include(x => x.SalesOrderItems)
                    .ThenInclude(x => x.sm_Product)
                    .Include(x => x.sm_Customer)
                    .Include(x => x.mk_DuAn)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));
                
                #region Validated ChargePayment

                if (model.ListPayment != null && model.ListPayment.Count > 0)
                {
                    foreach (var item in model.ListPayment)
                    {
                        if (item.LinePaidAmount < 0)
                        {
                            return Helper.CreateBadRequestResponse<SalesOrderViewModel>(string.Format("Tiền thanh toán không được âm"));
                        }
                        
                        if (item.LinePaidAmount == 0 || item.LinePaidAmount == null)
                        {
                            return Helper.CreateBadRequestResponse<SalesOrderViewModel>(string.Format("Vui lòng nhập số tiền thanh toán"));
                        }
                    
                        if (item.LinePaidAmount > entity.RemainingAmount)
                        {
                            return Helper.CreateBadRequestResponse<SalesOrderViewModel>(string.Format("Tiền thanh toán không được lớn hơn tổng tiền"));
                        }
                    }
                }
                #endregion

                if (model.ListPayment.Count != 0)
                {
                    // Điền tên customer vào sổ quỹ
                    var customer = await _dbContext.sm_Customer.AsNoTracking()
                                        .Where(x => x.Id == entity.CustomerId)
                                        .Select(x => new KhachHangViewModel { Id = x.Id, Name = x.Name, Code = x.Code, OrderCount = x.OrderCount, ExpenseAmount = x.ExpenseAmount})
                                        .FirstOrDefaultAsync();

                    foreach (var item in model.ListPayment)
                    {
                        await _cashbookTransactionHandler.Create(new CashbookTransactionCreateUpdateModel()
                        {
                            EntityId = customer.Id,
                            EntityCode = customer.Code,
                            OriginalDocumentId = entity.Id,
                            ConstructionId = entity.ConstructionId,
                            ContractId = entity.ContractId,
                            OriginalDocumentType = OriginDocumentCashbookTransactionConstants.SALES_ORDER,
                            OriginalDocumentCode = entity.OrderCode,
                            ProjectId = entity.ProjectId,
                            TransactionTypeCode = CashbookTransactionConstants.ReceiptVoucherType,
                            PurposeCode = CashbookTransactionConstants.AUTO_RECEIPT_PURPOSE,
                            PaymentMethodCode = item.PaymentMethod,
                            Amount = item.LinePaidAmount ?? 0,
                            EntityName = customer.Name,
                            ReceiptDate = DateTime.Now,
                            EntityTypeCode = EntityTypeCodeConstants.CUSTOMER,
                            EntityTypeName = EntityTypeNameConstants.CUSTOMER,                            
                            IsDebt = true,
                            Description = $"Phiếu thu tự động tạo khi khách hàng thanh toán cho đơn hàng {entity.OrderCode}",
                        }, currentUser);
                        
                    }

                    foreach (var item in entity.ListPayment)
                    {
                        model.ListPayment.Add(item);
                        
                        #region Cập nhật tổng chi tiêu và Tổng đơn hàng của khách hàng
                        var customerEntity = await _dbContext.sm_Customer.Where(x => x.Id == entity.CustomerId).FirstOrDefaultAsync();
                
                        var salesOrdersHasDone = _dbContext.sm_SalesOrder
                            .Where(x => x.CustomerId == entity.CustomerId && x.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN).ToList();
                
                        var salesOrdersHasHalf =  _dbContext.sm_SalesOrder
                            .Where(x => x.CustomerId == entity.CustomerId && x.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA).ToList();
                
                        if (customerEntity != null)
                        {
                            customerEntity.OrderCount = salesOrdersHasDone.Count + salesOrdersHasHalf.Count;
                            customerEntity.ExpenseAmount += item.LinePaidAmount ?? 0;
                            await _dbContext.SaveChangesAsync();
                        }
                        #endregion
                    }
                }

                entity.ListPayment = model.ListPayment;

                // Tính toán số tiền đã trả

                entity.PaidAmount = model.ListPayment.Sum(x => x.LinePaidAmount);
                entity.RemainingAmount = entity.Total - entity.PaidAmount;
                entity.LastModifiedOnDate = DateTime.Now;


                if (entity.RemainingAmount == 0)
                {
                    entity.PaymentStatusCode = PaymentStatusConstants.DA_THANH_TOAN;

                    if (entity.ExportStatusCode == OriginalDocumentTypeConstants.XUAT_KHO)
                    {
                        entity.StatusCode = SalesOrderConstants.StatusCode.COMPLETED;
                    }
                }
                else if (entity.PaidAmount != 0)
                {
                    entity.PaymentStatusCode = PaymentStatusConstants.THANH_TOAN_MOT_NUA;
                    entity.StatusCode = SalesOrderConstants.StatusCode.FINALIZED;
                }
                else if (entity.PaidAmount == 0)
                {
                    entity.PaymentStatusCode = PaymentStatusConstants.CHUA_THANH_TOAN;
                    entity.StatusCode = SalesOrderConstants.StatusCode.FINALIZED;
                }

                var createdResult = await _dbContext.SaveChangesAsync();

                if (createdResult > 0)
                {
                    if (entity.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN ||
                        entity.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA)
                    {
                        #region Cập nhật tổng chi tiêu và Tổng đơn hàng của khách hàng
                        var customerEntity = await _dbContext.sm_Customer.Where(x => x.Id == entity.CustomerId).FirstOrDefaultAsync();
                
                        var salesOrdersHasDone = _dbContext.sm_SalesOrder
                            .Where(x => x.CustomerId == entity.CustomerId && x.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN).ToList();
                
                        var salesOrdersHasHalf =  _dbContext.sm_SalesOrder
                            .Where(x => x.CustomerId == entity.CustomerId && x.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA).ToList();
                
                        if (customerEntity != null)
                        {
                            customerEntity.OrderCount = salesOrdersHasDone.Count + salesOrdersHasHalf.Count;
                            customerEntity.ExpenseAmount = salesOrdersHasDone.Sum(x => x.PaidAmount ?? 0) + salesOrdersHasHalf.Sum(x => x.PaidAmount ?? 0);
                            await _dbContext.SaveChangesAsync();
                        }
                        #endregion
                    }
                    
                }
                var result = await GetById(entity.Id);
                
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<SalesOrderViewModel>(result.Data, "Thanh toán thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<SalesOrderViewModel>(ex);
            }
        }

        public async Task<Response<SalesOrderViewModel>> ExportOrder(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_SalesOrder
                    .Include(x => x.SalesOrderItems)
                    .ThenInclude(x => x.sm_Product)
                    .Include(x => x.sm_Customer)
                    .Include(x => x.mk_DuAn)
                    .Include(x => x.sm_Construction)
                    .FirstOrDefaultAsync(x => x.Id == id);

                var vatTu = _dbContext.sm_Product;

                if (entity == null)
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                entity.ExportStatusCode = OriginalDocumentTypeConstants.XUAT_KHO;
                entity.LastModifiedOnDate = DateTime.Now;

                if (entity.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN && entity.ExportStatusCode == OriginalDocumentTypeConstants.XUAT_KHO)
                {
                    entity.StatusCode = SalesOrderConstants.StatusCode.COMPLETED;
                }

                if (entity.ExportStatusCode == OriginalDocumentTypeConstants.XUAT_KHO)
                {
                    if (entity.SalesOrderItems.Count != 0)
                    {
                        foreach (var item in entity.SalesOrderItems)
                        {
                            var newProduct = new StockTransactionCreateModel
                            {
                                ExportInventoryQuantity = item.Quantity,
                                ReceiptInventoryQuantity = 0,
                                SellableQuantity = 0,
                                InitialStockQuantity = vatTu.FirstOrDefault(x => x.Id == item.ProductId)?.InitialStockQuantity - item.Quantity,
                                ProductCode = vatTu.FirstOrDefault(x => x.Id == item.ProductId)?.Code,
                                ProductName = vatTu.FirstOrDefault(x => x.Id == item.ProductId)?.Name,
                                ProductId = item.ProductId,
                                WareCode = entity.WareCode,
                                OriginalDocumentType = OriginalDocumentTypeConstants.XUAT_KHO,
                                OriginalDocumentId = entity.Id,
                                //ClosingInventory = item.Quantity,
                                OriginalDocumentCode = entity.OrderCode,
                                //OpeningInventoryQuantity = vatTu.FirstOrDefault(x => x.Id == item.ProductId)?.InitialStockQuantity,
                                UnitPrice = item.UnitPrice,
                                Unit = item.Unit,
                                CreatedByUserId = entity.CreatedByUserId,
                                Action = ActionConstants.EXPORT_ORDER,
                            };

                            await _stockTransactionHandler.Create(newProduct);

                            // Update InitialStockQuantity

                            var product = await _dbContext.sm_Product.Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();

                            if (product == null)
                            {
                                return Helper.CreateBadRequestResponse<SalesOrderViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                            } else
                            {
                                product.InitialStockQuantity -= item.Quantity;
                                // product.SellableQuantity -= item.Quantity;
                                _dbContext.sm_Product.Update(product);
                            }
                        }
                    }
                }

                var result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                {
                    if (entity.ExportStatusCode == OriginalDocumentTypeConstants.XUAT_KHO)
                    {
                        // Create Inventory Note when export order
                        await _inventoryNoteHandler.CreateExportSalesOrder(entity.Id, currentUser);

                        // Create Debt Transaction when export order
                        await _debtTransactionHandler.CreateDebtSaleOrder(entity.Id, currentUser);
                    }
                }
                
                return Helper.CreateSuccessResponse(_mapper.Map<SalesOrderViewModel>(entity), "Xuất hàng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<SalesOrderViewModel>(ex);
            }
        }
    }
}
