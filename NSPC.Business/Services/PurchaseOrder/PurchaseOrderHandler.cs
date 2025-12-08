using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Business.Services.InventoryNote;
using NSPC.Business.Services.NhaCungCap;
using NSPC.Business.Services.StockTransaction;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.Quotation;
using NSPC.Data.Data.Entity.VatTu;
using NSPC.Data.Entity;
using Serilog;
using System.Linq.Expressions;
using NSPC.Data;
using static NSPC.Common.Helper;
using NSPC.Business.Services.ConstructionActitvityLog;

namespace NSPC.Business.Services
{
    // Service Api Chức năng nhập hàng
    public class PurchaseOrderHandler : IPurchaseOrderHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        private readonly ICashbookTransactionHandler _cashbookTransactionHandler;
        private readonly IStockTransactionHandler _stockTransactionHandler;
        private readonly IDebtTransactionHandler _debtTransactionHandler;
        private readonly IInventoryNoteHandler _inventoryNoteHandler;
        private readonly IProductInventoryHandler _productInventoryHandler;
        private readonly IConstructionActivityLogHandler _constructionActivityLogHandler;
        public PurchaseOrderHandler(SMDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ICashbookTransactionHandler cashbookTransactionHandler,
            IStockTransactionHandler stockTransactionHandler,
            IDebtTransactionHandler debtTransactionHandler,
            IInventoryNoteHandler inventoryNoteHandler,
            IProductInventoryHandler productInventoryHandler,
            IConstructionActivityLogHandler constructionActivityLogHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
            _stockTransactionHandler = stockTransactionHandler;
            _cashbookTransactionHandler = cashbookTransactionHandler;
            _debtTransactionHandler = debtTransactionHandler;
            _inventoryNoteHandler = inventoryNoteHandler;
            _productInventoryHandler = productInventoryHandler;
            _constructionActivityLogHandler = constructionActivityLogHandler;
        }

        /// <summary>
        /// Tạo đơn nhập hàng
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<PurchaseOrderViewModel>> Create(PurchaseOrderCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                // Check mã phiếu đã tồn tại với các phiếu khác hay chưa
                if (_dbContext.sm_PurchaseOrder.Any(x => x.OrderCode == model.OrderCode))
                    return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                        (string.Format("Mã phiếu {0} đã tồn tại!", model.OrderCode));
                
                List<sm_Product> allPurchaseOrderProducts = new List<sm_Product>();

                #region Validate

                // Validate Supplier
                if (!await _dbContext.sm_Supplier.AnyAsync(x => x.Id == model.SupplierId))
                {
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>
                        ($"Supplier Id: {model.SupplierId} not found.");
                }
                
                // Validate MaterialRequestId
                if (model.MaterialRequestId != null)
                {
                    if (!await _dbContext.sm_MaterialRequest.AnyAsync(x => x.Id == model.MaterialRequestId))
                    {
                        return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>
                            ($"Id ${model.MaterialRequestId} không tồn tại");
                    }
                }
                
                //Validate Product Item Model
                //1.Product ton tai trong he thong
                //2.Gia nhap vao khong duoc< 0
                //3.So luong khong duoc < 0
                //4.Product phai duoc cho phep ban
                //5.Ton kho phai du so luong
                //6.Khong duoc duplicate dong trong Product Item

                if (model.Items != null && model.Items.Count > 0)
                {
                    // Fill tat ca products
                    var allProductIds = model.Items.Select(x => x.ProductId).ToList();
                    allPurchaseOrderProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Khong duoc duplicate dong trong Product Item
                    // Neu so luong distinct product id khac so luong product item => co dong trung
                    if (model.Items.Count !=
                        model.Items.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.Items.Any(x => x.Quantity < 0))
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn 0.");

                    if (model.Items.Any(x => x.UnitPrice < 0))
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                                    ("Danh sách sản phẩm không được có đơn giá nhỏ hơn 0.");

                    foreach (var item in model.Items)
                    {
                        var product = allPurchaseOrderProducts
                                            .Where(x => x.Id == item.ProductId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                    }
                }
                #endregion

                #region Fill Item's Line No

                if (model.Items != null && model.Items.Count > 0)
                {
                    model.Items = model.Items.OrderBy(x => x.LineNo).ToList();
                    for (int i = 0; i < model.Items.Count; i++)
                        model.Items[i].LineNo = i + 1;
                }
                #endregion

                // Mapping và tạo entity
                var entity = _mapper.Map<sm_PurchaseOrder>(model);
                entity.Id = Guid.NewGuid();

                if (model.OrderCode == null)
                {
                    entity.OrderCode = await GetNewCode(PurchaseOrderConstants.OrderCodePrefix);
                }
                
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.StatusCode = PurchaseOrderConstants.StatusCode.FINALIZED;
                entity.ImportStatusCode = PurchaseOrderConstants.CHUA_NHAP;
                entity.PaidAmount = 0M;
                entity.Total = 0M;
                entity.SubTotal = 0M;
                entity.RemainingAmount = 0M;
                entity.WareCode = model.WareCode;

                #region Tính toán Order Items

                // Fill product & tính discount từng line
                foreach (var item in entity.Items)
                {
                    var product = allPurchaseOrderProducts.FirstOrDefault(x => x.Id == item.ProductId);

                    if (product != null)
                    {
                        item.ProductCode = product.Code;
                        item.ProductName = product.Name;
                        item.Unit = product.Unit;
                    }

                    // GoodsAmount: Thành tiền trước chiết khấu
                    item.GoodsAmount = item.Quantity * item.UnitPrice;

                    // Line Discount (tính trên đơn giá)
                    switch (item.UnitPriceDiscountType)
                    {
                        // Chế độ tính discount bằng %
                        case PurchaseOrderConstants.DiscountType.Percent:
                            item.UnitPriceDiscountAmount = item.UnitPrice * item.UnitPriceDiscountPercent / 100;
                            break;

                        // Chế độ tính discount bằng value
                        case PurchaseOrderConstants.DiscountType.Value:
                            if (item.UnitPrice != 0)
                                item.UnitPriceDiscountPercent = item.UnitPriceDiscountAmount * 100 / item.UnitPrice;
                            else
                                item.UnitPriceDiscountPercent = 0;
                            break;
                    }

                    // DiscountedGoodsAmount: giá trị hàng hóa sau chiết khấu dòng
                    item.AfterLineDiscountGoodsAmount = item.GoodsAmount - item.UnitPriceDiscountAmount * item.Quantity;
                }



                /*
                 * Phân bổ ngược lại DiscountAmount của Order vào từng dòng
                 * từ đó tính ra PurchaseOrderDiscountAmount
                 */

                // B1. Tính tổng AfterLineDiscountGoodsAmount của toàn bộ dòng
                var totalAfterDiscountGoodsAmount = entity.Items
                    .Sum(x => x.AfterLineDiscountGoodsAmount);

                // Bước 2. Tính ra DiscountAmount của đơn hàng
                switch (entity.DiscountType)
                {
                    // Chế độ tính discount bằng %
                    case PurchaseOrderConstants.DiscountType.Percent:
                        entity.DiscountAmount = totalAfterDiscountGoodsAmount * entity.DiscountPercent / 100;
                        break;

                    // Chế độ tính discount bằng value
                    case PurchaseOrderConstants.DiscountType.Value:
                        if (totalAfterDiscountGoodsAmount != 0)
                            entity.DiscountPercent = entity.DiscountAmount * 100 / totalAfterDiscountGoodsAmount;
                        else
                            entity.DiscountPercent = 0;
                        break;
                }

                // Bước 3. Phân bổ lại từng dòng ra Order Discount bằng AfterLineDiscountGoodsAmount

                foreach (var item in entity.Items)
                {
                    if (totalAfterDiscountGoodsAmount != 0)
                        item.OrderDiscountAmount = entity.DiscountAmount
                                                        * item.AfterLineDiscountGoodsAmount
                                                        / totalAfterDiscountGoodsAmount;
                    else
                        item.OrderDiscountAmount = 0;

                    // Todo: item.VATPercent = product.VATPercent;
                    // Thêm phần checkbox có tính thuế cho sản phẩm và lựa chọn mức thuế
                    var product = allPurchaseOrderProducts.FirstOrDefault(x => x.Id == item.ProductId);

                    if (product != null)
                    {
                        item.IsProductVATApplied = product.IsVATApplied;
                        item.VATCode = product.ImportVATCode;
                        item.VATPercent = product.IsVATApplied ? product.ImportVATPercent : 0M;
                        item.VATableAmount = item.AfterLineDiscountGoodsAmount - item.OrderDiscountAmount;
                        item.VATAmount = item.VATableAmount * item.VATPercent / 100;
                        item.LineAmount = item.VATableAmount + item.VATAmount;
                    }
                }
                #endregion

                #region Payment & Amount

                /* 
                 * Xử lý List Payment tính ra số tiền
                 */
                entity.ListPayment = model.ListPayment;
                if (entity.ListPayment != null && entity.ListPayment.Count > 0)
                {
                    // Fill CreatedDate
                    entity.ListPayment.ForEach(x => x.CreateDate = entity.CreatedOnDate);

                    if (model.ListPayment != null)
                    {
                        // Calculate Total Paid Amount
                        entity.PaidAmount = model.ListPayment.Sum(x => x.LinePaidAmount);
                    }
                }

                // TotalOtherCost, tính ra số tiền khác
                entity.ListOtherCost = model.ListOtherCost;
                entity.TotalOtherCost = entity.ListOtherCost.Sum(x => x.FeeCost);

                // SubTotal = Tổng tiền sau khi chiết khấu dòng của từng Line
                entity.SubTotal = entity.Items.Sum(x => x.AfterLineDiscountGoodsAmount);

                // Total = Tổng tiền người dùng phải trả từng Line + Các chi phí khác
                entity.Total = entity.Items.Sum(x => x.LineAmount) + entity.TotalOtherCost;

                // TotalQuantity = Tổng SL Sản phẩm
                entity.TotalQuantity = entity.Items.Sum(x => x.Quantity);

                // Số tiền VAT
                entity.VATAmount = entity.Items.Sum(x => x.VATAmount);

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
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                            ("Danh sách thanh toán không được có số tiền nhỏ hơn 0.");

                    if (model.ListPayment.Any(x => x.LinePaidAmount == 0 || x.LinePaidAmount == null))
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                            ("Vui lòng nhập số tiền thanh toán");

                    if (model.ListPayment.Any(x => x.LinePaidAmount > entity.Total))
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                            ("Danh sách thanh toán không được có số tiền lớn hơn số tiền cần thanh toán.");
                    
                    if (model.ListPayment.Sum(x => x.LinePaidAmount) > entity.Total)
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                            ("Số tiền thanh toán không được lớn hơn tổng tiền");

                    // Fill supplier from model
                    model.ListPayment.ForEach(x => x.SupplierId = model.SupplierId);
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

                // Hoàn thành = Thanh toán hết và Đã nhập kho
                if (entity.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN &&
                    entity.ImportStatusCode == PurchaseOrderConstants.NHAP_HANG)
                {
                    entity.StatusCode = PurchaseOrderConstants.StatusCode.COMPLETED;
                }

                #endregion

                _dbContext.sm_PurchaseOrder.Add(entity);

                var createResult = await _dbContext.SaveChangesAsync();

                #region Cashbook Transaction - Sổ quỹ

                /*
                 * Tạo phiếu chi cho nhà cung cấp tự động từ thông tin thanh toán
                 */

                if (createResult > 0)
                {
                    // Điền tên supplier vào sổ quỹ
                    var supplier = await _dbContext.sm_Supplier.AsNoTracking()
                                        .Where(x => x.Id == entity.SupplierId)
                                        .Select(x => new NhaCungCapViewModel { Id = x.Id, Name = x.Name, Code = x.Code })
                                        .FirstOrDefaultAsync();

                    if (entity.ListPayment != null && entity.ListPayment.Count() != 0)
                    {
                        foreach (var item in entity.ListPayment)
                        {
                            await _cashbookTransactionHandler.Create(new CashbookTransactionCreateUpdateModel()
                            {
                                EntityCode = supplier.Code,
                                EntityId = supplier.Id,
                                OriginalDocumentId = entity.Id,
                                ConstructionId = entity.ConstructionId,
                                ContractId = entity.ContractId,
                                OriginalDocumentType = OriginDocumentCashbookTransactionConstants.PURCHASE_ORDER,
                                OriginalDocumentCode = entity.OrderCode,
                                ProjectId = entity.ProjectId,
                                TransactionTypeCode = CashbookTransactionConstants.PaymentVoucherType,
                                PurposeCode = PurchaseOrderStatusConstants.AUTO_RECEIPT,
                                PaymentMethodCode = item.PaymentMethod,
                                Amount = item.LinePaidAmount ?? 0,
                                EntityName = supplier?.Name,
                                ReceiptDate = entity.CreatedOnDate,
                                EntityTypeCode = EntityTypeCodeConstants.SUPPLIER,
                                EntityTypeName = EntityTypeNameConstants.SUPPLIER,
                                Description = $"Phiếu chi tự động tạo khi thanh toán cho nhà cung cấp theo đơn nhập hàng {entity.OrderCode}",
                                IsDebt = true
                            }, currentUser);
                        }
                    }

                    #region Cập nhật tổng chi tiêu và Tổng đơn hàng của nhà cung cấp
                    var supplierEntity = await _dbContext.sm_Supplier.Where(x => x.Id == entity.SupplierId).FirstOrDefaultAsync();

                    var purchaseOrdersHasDone = _dbContext.sm_PurchaseOrder
                        .Where(x => x.SupplierId == entity.SupplierId && x.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN).ToList();

                    var purchaseOrdersHasHalf = _dbContext.sm_PurchaseOrder
                        .Where(x => x.SupplierId == entity.SupplierId && x.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA).ToList();

                    if (supplierEntity != null)
                    {
                        supplierEntity.OrderCount = purchaseOrdersHasDone.Count + purchaseOrdersHasHalf.Count;
                        supplierEntity.ExpenseAmount = purchaseOrdersHasDone.Sum(x => x.PaidAmount ?? 0) + purchaseOrdersHasHalf.Sum(x => x.PaidAmount ?? 0);
                        await _dbContext.SaveChangesAsync();
                    }
                    #endregion 
                    
                    #region Log lại hoạt động thêm mới đơn mua hàng vào bảng sm_ConstructionActivityLog
                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã tạo đơn nhập hàng",
                        CodeLinkDescription = $"{entity.OrderCode} - {supplier.Name}",
                        OrderId = entity.Id,
                        ConstructionId = entity.ConstructionId,
                    }, currentUser);
                    #endregion
                }
                #endregion

                var result = await GetById(entity.Id);
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<PurchaseOrderViewModel>(result.Data, "Tạo đơn nhập hàng thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<PurchaseOrderViewModel>(ex);
            }
        }

        /// <summary>
        /// Cập nhật đơn hàng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response<PurchaseOrderViewModel>> Update(Guid id, PurchaseOrderCreateUpdateModel model, RequestUser currentUser)
        {
            /*
             * RULE
             * Chỉ được cập nhật khi chưa có thanh toán, nếu đơn đã thanh toán rồi thì 
             * dùng API charge-payment để xử lý tiếp
             * Chỉ được cập nhật khi chưa có nhập hàng
             */

            try
            {
                // Check mã phiếu đã tồn tại với các phiếu khác hay chưa
                if (_dbContext.sm_PurchaseOrder.Any(x => x.OrderCode == model.OrderCode))
                    return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                        (string.Format("Mã phiếu {0} đã tồn tại!", model.OrderCode));
                
                var entity = await _dbContext.sm_PurchaseOrder
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Sm_Product)
                    .Include(x => x.sm_Supplier)
                    .Include(x => x.mk_DuAn)
                    .Include(x => x.sm_Construction)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>
                        ($"Đơn hàng không tồn tại trong hệ thống, id: {id}");


                sm_Quotation quotation = null;
                List<sm_Product> allPurchaseOrderProducts = new List<sm_Product>();

                #region Validate model

                // Validate supplier
                if (!await _dbContext.sm_Supplier.AnyAsync(x => x.Id == model.SupplierId))
                {
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>
                        ($"supplier Id: {model.SupplierId} not found.");
                }
                
                // Validate MaterialRequestId
                if (model.MaterialRequestId != null)
                {
                    if (!await _dbContext.sm_MaterialRequest.AnyAsync(x => x.Id == model.MaterialRequestId))
                    {
                        return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>
                            ($"Id ${model.MaterialRequestId} không tồn tại");
                    }
                }

                // Validate Proruct Item Model
                // 1.Product ton tai trong he thong
                // 2.Gia nhap vao khong duoc< 0
                // 3.So luong khong duoc < 0
                // 4.Product phai duoc cho phep ban
                // 5.Ton kho phai du so luong
                // 6.Khong duoc duplicate dong trong Product Item
                if (model.Items != null && model.Items.Count > 0)
                {
                    // Fill tat ca products
                    var allProductIds = model.Items.Select(x => x.ProductId).ToList();
                    allPurchaseOrderProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Khong duoc duplicate dong trong Product Item
                    // Neu so luong distinct product id khac so luong product item => co dong trung
                    if (model.Items.Count !=
                        model.Items.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.Items.Any(x => x.Quantity < 0))
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn 0.");

                    if (model.Items.Any(x => x.UnitPrice < 0))
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                                    ("Danh sách sản phẩm không được có đơn giá nhỏ hơn 0.");

                    foreach (var item in model.Items)
                    {
                        var product = allPurchaseOrderProducts.FirstOrDefault(x => x.Id == item.ProductId);

                        if (product == null)
                            return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                    }
                }

                #endregion

                #region Fill Item's Line No
                if (model.Items != null && model.Items.Count > 0)
                {
                    model.Items = model.Items.OrderBy(x => x.LineNo).ToList();
                    for (int i = 0; i < model.Items.Count; i++)
                        model.Items[i].LineNo = i + 1;
                }
                #endregion

                #region Validate Payment & Receipt Status (trạng thái thanh toán & nhập hàng)

                // Nếu đơn đã có thanh toán thì không cho cập nhật nữa
                if (entity.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA ||
                    entity.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN)
                    return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                                    ("Đơn hàng đã có thanh toán, không thể cập nhật.");

                // Nếu đơn đã được nhập kho thì không cho cập nhật
                if (entity.ImportStatusCode == RecieveStatus.DA_NHAP)
                    return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                                    ("Đơn hàng đã được nhập kho, không thể cập nhật.");

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
                
                entity.PaidAmount = 0M;
                entity.Total = 0M;
                entity.SubTotal = 0M;
                entity.RemainingAmount = 0M;
                entity.DiscountPercent = model.DiscountPercent;
                entity.DiscountAmount = model.DiscountAmount;
                entity.DiscountType = model.DiscountType;
                entity.WareCode = model.WareCode;
                entity.Note = model.Note;
                entity.SupplierId = model.SupplierId;
                entity.ProjectId = model.ProjectId;

                // Remove Old Sales Order Item -> Re-add
                _dbContext.RemoveRange(entity.Items);
                entity.Items = new List<sm_PurchaseOrderItem>();

                #region Điền thông tin Product, GoodsAmount, Line Discount

                // Fill Product Data, GoodsAmount, DiscountAmount
                if (model.Items != null && model.Items.Count > 0)
                {
                    foreach (var modelItem in model.Items)
                    {
                        var item = _mapper.Map<sm_PurchaseOrderItem>(modelItem);
                        entity.Items.Add(item);

                        // Assign PurchaseOrderId
                        item.PurchaseOrderId = entity.Id;

                        // Fill product data
                        var product = allPurchaseOrderProducts.FirstOrDefault(x => x.Id == modelItem.ProductId);

                        if (product != null)
                        {
                            item.ProductCode = product.Code;
                            item.ProductName = product.Name;
                            item.Unit = product.Unit;
                        }

                        // GoodsAmount: giá trị hàng hóa
                        item.GoodsAmount = item.Quantity * item.UnitPrice;

                        // Line Discount (tính trên đơn giá)
                        switch (item.UnitPriceDiscountType)
                        {
                            // Chế độ tính discount bằng %
                            case PurchaseOrderConstants.DiscountType.Percent:
                                item.UnitPriceDiscountAmount = item.UnitPrice * item.UnitPriceDiscountPercent / 100;
                                break;

                            // Chế độ tính discount bằng value
                            case PurchaseOrderConstants.DiscountType.Value:
                                if (item.UnitPrice != 0)
                                    item.UnitPriceDiscountPercent = item.UnitPriceDiscountAmount * 100 / item.UnitPrice;
                                else
                                    item.UnitPriceDiscountPercent = 0;
                                break;
                        }

                        // DiscountedGoodsAmount: giá trị hàng hóa sau chiết khấu dòng
                        item.AfterLineDiscountGoodsAmount = item.GoodsAmount - item.UnitPriceDiscountAmount * item.Quantity;
                    }
                }

                /*
                 * Phân bổ ngược lại DiscountAmount của Order vào từng dòng
                 * từ đó tính ra PurchaseOrderDiscountAmount
                 */

                // B1. Tính tổng AfterLineDiscountGoodsAmount của toàn bộ dòng
                var totalAfterDiscountGoodsAmount = entity.Items
                    .Sum(x => x.AfterLineDiscountGoodsAmount);

                // B2. Tính ra OrderDiscount
                switch (entity.DiscountType)
                {
                    // Chế độ tính discount bằng %
                    case PurchaseOrderConstants.DiscountType.Percent:
                        entity.DiscountAmount = totalAfterDiscountGoodsAmount * entity.DiscountPercent / 100;
                        break;

                    // Chế độ tính discount bằng value
                    case PurchaseOrderConstants.DiscountType.Value:
                        if (totalAfterDiscountGoodsAmount != 0)
                            entity.DiscountPercent = entity.DiscountAmount * 100 / totalAfterDiscountGoodsAmount;
                        else
                            entity.DiscountPercent = 0;
                        break;
                }

                // B2. Phân bổ lại giá trị cho từng Line
                foreach (var item in entity.Items)
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
                    var product = allPurchaseOrderProducts
                        .FirstOrDefault(x => x.Id == item.ProductId);

                    if (product != null)
                    {
                        item.IsProductVATApplied = product.IsVATApplied;
                        item.VATCode = product.ImportVATCode;
                        item.VATPercent = product.IsVATApplied ? product.ImportVATPercent : 0M;
                        item.VATableAmount = item.AfterLineDiscountGoodsAmount - item.OrderDiscountAmount;
                        item.VATAmount = item.VATableAmount * item.VATPercent / 100;
                        item.LineAmount = item.VATableAmount + item.VATAmount;
                    }
                }

                // B3. Thêm lại item sau khi tính toán
                foreach (var item in entity.Items)
                {
                    _dbContext.sm_PurchaseOrderItem.Add(item);
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
                    if (model.ListPayment != null && model.ListPayment.Count > 0)
                    {
                        entity.PaidAmount = model.ListPayment.Sum(x => x.LinePaidAmount);
                    }
                }

                // TotalOtherCost, tính ra số tiền khác
                entity.ListOtherCost = model.ListOtherCost;
                entity.TotalOtherCost = entity.ListOtherCost.Sum(x => x.FeeCost);

                // SubTotal = Tổng tiền sau khi chiết khấu dòng của từng Line
                entity.SubTotal = entity.Items.Sum(x => x.AfterLineDiscountGoodsAmount);

                // Total = Tổng tiền người dùng phải trả từng Line + Phí giao hàng
                entity.Total = entity.Items.Sum(x => x.LineAmount) + entity.TotalOtherCost;

                // TotalQuantity = Tổng SL Sản phẩm
                entity.TotalQuantity = entity.Items.Sum(x => x.Quantity);

                // Số tiền VAT
                entity.VATAmount = entity.Items.Sum(x => x.VATAmount);

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
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                            ("Danh sách thanh toán không được có số tiền nhỏ hơn 0.");

                    if (model.ListPayment.Any(x => x.LinePaidAmount == 0 || x.LinePaidAmount == null))
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                            ("Vui lòng nhập số tiền thanh toán");

                    if (model.ListPayment.Any(x => x.LinePaidAmount > entity.Total))
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                            ("Danh sách thanh toán không được có số tiền lớn hơn số tiền cần thanh toán.");
                    
                    if (model.ListPayment.Sum(x => x.LinePaidAmount) > entity.Total)
                        return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                            ("Số tiền thanh toán không được lớn hơn tổng tiền");

                    // Fill supplier from model
                    model.ListPayment.ForEach(x => x.SupplierId = model.SupplierId);
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
                    entity.ImportStatusCode == PurchaseOrderConstants.NHAP_HANG)
                {
                    entity.StatusCode = PurchaseOrderConstants.StatusCode.COMPLETED;
                }

                #endregion

                _dbContext.sm_PurchaseOrder.Update(entity);

                var createResult = await _dbContext.SaveChangesAsync();

                #region Cashbook Transaction - Sổ quỹ
                if (createResult > 0)
                {
                    // Điền tên supplier vào sổ quỹ
                    var supplier = await _dbContext.sm_Supplier.AsNoTracking()
                                        .Where(x => x.Id == entity.SupplierId)
                                        .Select(x => new NhaCungCapViewModel { Id = x.Id, Name = x.Name, Code = x.Code })
                                        .FirstOrDefaultAsync();

                    if (entity.ListPayment != null && entity.ListPayment.Count() != 0)
                    {
                        foreach (var item in entity.ListPayment)
                        {
                            await _cashbookTransactionHandler.Create(new CashbookTransactionCreateUpdateModel()
                            {
                                EntityId = supplier.Id,
                                EntityCode = supplier.Code,
                                OriginalDocumentId = entity.Id,
                                ConstructionId = entity.ConstructionId,
                                OriginalDocumentType = OriginDocumentCashbookTransactionConstants.PURCHASE_ORDER,
                                OriginalDocumentCode = entity.OrderCode,
                                TransactionTypeCode = CashbookTransactionConstants.PaymentVoucherType,
                                PurposeCode = PurchaseOrderStatusConstants.AUTO_RECEIPT,
                                PaymentMethodCode = item.PaymentMethod,
                                Amount = item.LinePaidAmount ?? 0,
                                EntityName = supplier?.Name,
                                ReceiptDate = entity.CreatedOnDate,
                                EntityTypeCode = EntityTypeCodeConstants.SUPPLIER,
                                EntityTypeName = EntityTypeNameConstants.SUPPLIER,
                                Description = $"Phiếu chi tự động tạo khi thanh toán cho đơn nhập hàng {entity.OrderCode}",
                                IsDebt = true
                            }, currentUser);
                        }
                    }

                    #region Cập nhật tổng chi tiêu và Tổng đơn hàng của nhà cung cấp
                    var supplierEntity = await _dbContext.sm_Supplier.Where(x => x.Id == entity.SupplierId).FirstOrDefaultAsync();

                    var purchaseOrdersHasDone = _dbContext.sm_PurchaseOrder
                        .Where(x => x.SupplierId == entity.SupplierId && x.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN).ToList();

                    var purchaseOrdersHasHalf = _dbContext.sm_PurchaseOrder
                        .Where(x => x.SupplierId == entity.SupplierId && x.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA).ToList();

                    if (supplierEntity != null)
                    {
                        supplierEntity.OrderCount = purchaseOrdersHasDone.Count + purchaseOrdersHasHalf.Count;
                        supplierEntity.ExpenseAmount = purchaseOrdersHasDone.Sum(x => x.PaidAmount ?? 0) + purchaseOrdersHasHalf.Sum(x => x.PaidAmount ?? 0);
                        await _dbContext.SaveChangesAsync();
                    }
                    #endregion
                }
                #endregion

                var result = await GetById(entity.Id);
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<PurchaseOrderViewModel>(result.Data, "Sửa đơn nhập hàng thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<PurchaseOrderViewModel>(ex);
            }
        }

        public async Task<Response<PurchaseOrderViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_PurchaseOrder.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<PurchaseOrderViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<PurchaseOrderViewModel>(ex);
            }

        }

        public async Task<Response<PurchaseOrderViewModel>> GetById(Guid id)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = await _dbContext.sm_PurchaseOrder.AsNoTracking()
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Sm_Product)
                    .Include(x => x.sm_Supplier)
                    .Include(x => x.mk_DuAn)
                    .Include(x => x.sm_Construction)
                    .Include(x => x.sm_Contract)
                    .FirstOrDefaultAsync(x => x.Id == id);
                //var entity = await _dbContext.mk_QuanLyPhieu.AsNoTracking().Include(x => x.VatTuItem).ThenInclude(x => x.Sm_Product).Include(x => x.mk_DuAn).Include(x => x.sm_Supplier).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>("Mã phiếu không tồn tại trong hệ thống.");

                var result = _mapper.Map<PurchaseOrderViewModel>(entity);

                if (result.StatusCode == TrangThaiPhieuNhapXuatConstants.CHO_DUYET)
                {
                    if (currentUser.IsAdmin)
                    {
                        result.AllowedActions = new[] { "UPDATE", "APPROVE", "REJECT", "DELETE" };
                    }
                    else
                    {
                        result.AllowedActions = new[] { "UPDATE", "DELETE" };
                    }
                }
                else if (result.StatusCode == TrangThaiPhieuNhapXuatConstants.DA_TU_CHOI)
                {
                    result.AllowedActions = new[] { "UPDATE", "DELETE" };
                }
                else if (result.ImportStatusCode == RecieveStatus.CHUA_NHAP)
                {
                    result.AllowedActions = new[] { "UPDATE", "DELETE" };
                }
                else
                {
                    result.AllowedActions = Array.Empty<string>();
                }


                return new Response<PurchaseOrderViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<PurchaseOrderViewModel>(ex);
            }

        }

        public async Task<Response<Pagination<PurchaseOrderViewModel>>> GetPage(PurchaseOrderQueryModel query)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_PurchaseOrder.AsNoTracking()
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Sm_Product)
                    .Include(x => x.sm_Supplier)
                    .Include(x => x.mk_DuAn)
                    .Include(x => x.sm_Construction)
                    .Include(x => x.sm_Construction)
                    .Include(x => x.sm_Contract)
                    .Where(predicate);
                //var queryResult = _dbContext.mk_QuanLyPhieu.AsNoTracking().Include(x => x.VatTuItem).ThenInclude(x => x.Sm_Product).Include(x => x.mk_DuAn).Include(x => x.sm_Supplier).Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<PurchaseOrderViewModel>>(data);

                foreach (var item in result.Content)
                {
                    if (item.StatusCode == TrangThaiPhieuNhapXuatConstants.CHO_DUYET)
                    {
                        if (currentUser.IsAdmin)
                        {
                            item.AllowedActions = new[] { "UPDATE", "APPROVE", "REJECT", "DELETE" };
                        }
                        else
                        {
                            item.AllowedActions = new[] { "UPDATE", "DELETE" };
                        }
                    }
                    else if (item.StatusCode == TrangThaiPhieuNhapXuatConstants.DA_TU_CHOI)
                    {
                        item.AllowedActions = new[] { "UPDATE", "DELETE" };
                    }
                    else
                    {
                        item.AllowedActions = Array.Empty<string>();
                    }
                }

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<PurchaseOrderViewModel>>(ex);
            }

        }

        private Expression<Func<sm_PurchaseOrder, bool>> BuildQuery(PurchaseOrderQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_PurchaseOrder>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.OrderCode.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.WareCode))
                predicate.And(s => s.WareCode.ToLower().Contains(query.WareCode.ToLower()));
            if (!string.IsNullOrEmpty(query.StatusCode))
                predicate.And(s => s.StatusCode.ToLower().Contains(query.StatusCode.ToLower()));
            if (!string.IsNullOrEmpty(query.ImportStatusCode))
                predicate.And(s => s.ImportStatusCode.ToLower().Contains(query.ImportStatusCode.ToLower()));

            if (query.ConstructionId.HasValue)
            {
                predicate.And(x => x.ConstructionId == query.ConstructionId);
            }

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

            if (query.SupplierId.HasValue)
            {
                predicate = predicate.And(x => x.SupplierId == query.SupplierId);
            }

            if (!currentUser.ListRights.Contains("PURCHASEORDER." + RightActionConstants.VIEWALL))
                predicate.And(s => s.CreatedByUserId == currentUser.UserId);

            return predicate;
        }

        private async Task<string> GetNewCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_PurchaseOrder.AsNoTracking().Where(x => x.OrderCode.Contains(code)).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

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
                Log.Error("123", ex);
                return string.Empty;
            }
        }

        public Task<Response<string>> ExportExcel(Guid idPhieu)
        {
            throw new NotImplementedException();
        }

        public Task<Response> ImportApply(List<PurchaseOrderCreateUpdateModel> model, string typePhieu)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<PurchaseOrderViewModel>> Reciept(Guid id, RequestUser currentUser)
        {
            try
            {
                List<jsonb_HistoryProcess> allHistoryProcess = new List<jsonb_HistoryProcess>();
                
                var entity = await _dbContext.sm_PurchaseOrder
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Sm_Product)
                    .Include(x => x.sm_Supplier)
                    .Include(x => x.mk_DuAn)
                    .FirstOrDefaultAsync(x => x.Id == id);

                var vatTu = _dbContext.sm_Product;

                if (entity == null)
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                entity.ImportStatusCode = PurchaseOrderConstants.NHAP_HANG;
                entity.LastModifiedOnDate = DateTime.Now;

                if (entity.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN && entity.ImportStatusCode == PurchaseOrderConstants.NHAP_HANG)
                {
                    entity.StatusCode = PurchaseOrderConstants.StatusCode.COMPLETED;
                }

                if (entity.ImportStatusCode == PurchaseOrderConstants.NHAP_HANG)
                {
                    if (entity.Items.Count != 0)
                    {
                        foreach (var item in entity.Items)
                        {
                            var newProduct = new StockTransactionCreateModel
                            {
                                ReceiptInventoryQuantity = item.Quantity,
                                ExportInventoryQuantity = 0,
                                SellableQuantity = item.Quantity,
                                InitialStockQuantity = vatTu.FirstOrDefault(x => x.Id == item.ProductId)?.InitialStockQuantity + item.Quantity,
                                ProductCode = vatTu.FirstOrDefault(x => x.Id == item.ProductId)?.Code,
                                ProductName = vatTu.FirstOrDefault(x => x.Id == item.ProductId)?.Name,
                                ProductId = item.ProductId,
                                WareCode = entity.WareCode,
                                OriginalDocumentType = PurchaseOrderConstants.NHAP_HANG,
                                OriginalDocumentId = entity.Id,
                                //ClosingInventoryQuantity = item.Quantity,
                                OriginalDocumentCode = entity.OrderCode,
                                //OpeningInventoryQuantity = vatTu.FirstOrDefault(x => x.Id == item.ProductId)?.InitialStockQuantity,
                                UnitPrice = item.UnitPrice,
                                Unit = item.Unit,
                                CreatedByUserId = entity.CreatedByUserId,
                                Action = ActionConstants.RECEIPT_ORDER,
                            };

                            await _stockTransactionHandler.Create(newProduct);

                            // Update InitialStockQuantity

                            var product = await _dbContext.sm_Product.Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                            var productInventoryByIdAndWareCode = await _dbContext.sm_ProductInventory
                                .Where(x => x.ProductId == item.ProductId && x.WarehouseCode == entity.WareCode)
                                .FirstOrDefaultAsync();
                            var productInventory = await _dbContext.sm_ProductInventory
                                .Where(x => x.ProductId == item.ProductId)
                                .FirstOrDefaultAsync();
                            
                            if (product == null)
                            {
                                return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                            }
                            else
                            {
                                #region Update tồn kho tồng và tổng số lượng có thể bán
                                product.InitialStockQuantity += item.Quantity;
                                product.SellableQuantity += item.Quantity;
                                _dbContext.sm_Product.Update(product);
                                #endregion
                                
                                #region Check khi có tồn tại bản ghi có chứa wareCode thì update, còn không thì tạo bản ghi mới
                                var productInventoryEntity = _dbContext.sm_ProductInventory
                                    .Where(x => x.ProductId == item.ProductId);
                        
                                if (productInventoryEntity.Any(x => x.WarehouseCode == entity.WareCode))
                                {
                                    if (productInventoryByIdAndWareCode != null)
                                    {
                                        productInventoryByIdAndWareCode.SellableQuantity += item.Quantity;
                                        _dbContext.sm_ProductInventory.Update(productInventoryByIdAndWareCode);
                                    }
                                    else
                                    {
                                        productInventory.SellableQuantity += item.Quantity;
                                        productInventory.WarehouseCode = entity.WareCode;
                                        _dbContext.sm_ProductInventory.Update(productInventory);
                                    }
                                }
                                else
                                {
                                    await _productInventoryHandler.Create(new ProductInventoryCreateModel
                                    {
                                        WarehouseCode = entity.WareCode,
                                        SellableQuantity = item.Quantity,
                                        ProductId = item.ProductId,
                                    }, currentUser);
                                }
                                #endregion
                            }
                        }

                    }
                    
                    #region Nhập hàng thì sẽ cập nhật lịch sử xử lý trong yêu cầu vật tư
                    var materialRequestEntity =
                        await _dbContext.sm_MaterialRequest.FirstOrDefaultAsync(x => x.Id == entity.MaterialRequestId);

                    if (materialRequestEntity != null)
                    {
                        foreach (var item in materialRequestEntity.ListHistoryProcess)
                        {
                            allHistoryProcess.Add(item);
                        }

                        var newHistoryProcess = new jsonb_HistoryProcess()
                        {
                            UserName =  currentUser.FullName,
                            Description = "đã tạo đơn mua hàng thành công từ yêu cầu",
                            CreatedOnDate =  DateTime.Now,
                        };

                        allHistoryProcess.Add(newHistoryProcess);
                        materialRequestEntity.ListHistoryProcess = allHistoryProcess;
                        _dbContext.sm_MaterialRequest.Update(materialRequestEntity);
                    }
                    #endregion
                }
                var result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                {
                    if (entity.ImportStatusCode == PurchaseOrderConstants.NHAP_HANG)
                    {
                        // Create Debt Transaction when import order
                        await _debtTransactionHandler.CreateDebtPurchaseOrder(entity.Id, currentUser);

                        // Create Inventory Note when import order
                        await _inventoryNoteHandler.CreateImportPurchaseOrder(entity.Id, currentUser);
                    }
                }
                
                return Helper.CreateSuccessResponse(_mapper.Map<PurchaseOrderViewModel>(entity), "Nhập hàng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<PurchaseOrderViewModel>(ex);
            }

        }

        public async Task<Response<PurchaseOrderViewModel>> RejectOrder(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_PurchaseOrder.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                entity.StatusCode = PurchaseOrderConstants.StatusCode.CANCELLED;
                entity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<PurchaseOrderViewModel>(entity), "Hủy đơn nhập hàng thành công");

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<PurchaseOrderViewModel>(ex);
            }
        }

        public async Task<Response<PurchaseOrderViewModel>> ChargePaymentOrder(Guid id, PurchaseOrderCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_PurchaseOrder
                    .Include(x => x.sm_Construction)
                    .Include(x => x.sm_Contract)
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Sm_Product)
                    .Include(x => x.sm_Supplier)
                    .Include(x => x.mk_DuAn)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                #region Validated ChargePayment
                if (model.ListPayment != null && model.ListPayment.Count > 0)
                {
                    foreach (var item in model.ListPayment)
                    {
                        if (item.LinePaidAmount < 0)
                        {
                            return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>(string.Format("Tiền thanh toán không được âm"));
                        }

                        if (item.LinePaidAmount == 0 || item.LinePaidAmount == null)
                        {
                            return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>(string.Format("Vui lòng nhập số tiền thanh toán"));
                        }

                        if (item.LinePaidAmount > entity.RemainingAmount)
                        {
                            return Helper.CreateBadRequestResponse<PurchaseOrderViewModel>(string.Format("Tiền thanh toán không được lớn hơn tổng tiền"));
                        }
                    }
                }
                #endregion

                if (model.ListPayment.Count != 0)
                {
                    var supplier = await _dbContext.sm_Supplier.AsNoTracking()
                                        .Where(x => x.Id == entity.SupplierId)
                                        .Select(x => new NhaCungCapViewModel { Id = x.Id, Name = x.Name, Code = x.Code })
                                        .FirstOrDefaultAsync();

                    foreach (var item in model.ListPayment)
                    {
                        await _cashbookTransactionHandler.Create(new CashbookTransactionCreateUpdateModel()
                        {
                            EntityCode = supplier.Code,
                            EntityId = supplier.Id,
                            OriginalDocumentId = entity.Id,
                            ConstructionId = entity.ConstructionId,
                            ContractId = entity.ContractId,
                            OriginalDocumentType = OriginDocumentCashbookTransactionConstants.PURCHASE_ORDER,
                            OriginalDocumentCode = entity.OrderCode,
                            ProjectId = entity.ProjectId,
                            TransactionTypeCode = CashbookTransactionConstants.PaymentVoucherType,
                            PurposeCode = SaleOrderStatusConstants.AUTO_PAYMENT,
                            PaymentMethodCode = item.PaymentMethod,
                            Amount = item.LinePaidAmount ?? 0,
                            EntityName = supplier?.Name,
                            ReceiptDate = DateTime.Now,
                            EntityTypeCode = "supplier",
                            EntityTypeName = "Nhà cung cấp",
                            Description = $"Phiếu chi tự động tạo khi thanh toán cho nhà cung cấp theo đơn nhập hàng {entity.OrderCode}",
                            IsDebt = true
                        }, currentUser);
                    }

                    foreach (var item in entity.ListPayment)
                    {
                        model.ListPayment.Add(item);

                        #region Cập nhật tổng chi tiêu và Tổng đơn hàng của nhà cung cấp
                        var supplierEntity = await _dbContext.sm_Supplier.Where(x => x.Id == entity.SupplierId).FirstOrDefaultAsync();

                        var purchaseOrdersHasDone = _dbContext.sm_PurchaseOrder
                            .Where(x => x.SupplierId == entity.SupplierId && x.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN).ToList();

                        var purchaseOrdersHasHalf = _dbContext.sm_PurchaseOrder
                            .Where(x => x.SupplierId == entity.SupplierId && x.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA).ToList();

                        if (supplierEntity != null)
                        {
                            supplierEntity.OrderCount = purchaseOrdersHasDone.Count + purchaseOrdersHasHalf.Count;
                            supplierEntity.ExpenseAmount += item.LinePaidAmount ?? 0;
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

                if (entity.RemainingAmount == 0 || (entity.PaidAmount == entity.Total))
                {
                    entity.PaymentStatusCode = PaymentStatusConstants.DA_THANH_TOAN;

                    if (entity.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN && entity.ImportStatusCode == PurchaseOrderConstants.NHAP_HANG)
                    {
                        entity.StatusCode = PurchaseOrderConstants.StatusCode.COMPLETED;
                    }
                }
                else if (entity.PaidAmount != 0 && entity.PaidAmount < entity.Total)
                {
                    entity.PaymentStatusCode = PaymentStatusConstants.THANH_TOAN_MOT_NUA;
                    entity.StatusCode = PurchaseOrderConstants.StatusCode.FINALIZED;
                }
                else if (entity.PaidAmount == 0)
                {
                    entity.PaymentStatusCode = PaymentStatusConstants.CHUA_THANH_TOAN;
                    entity.StatusCode = PurchaseOrderConstants.StatusCode.FINALIZED;
                }

                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    if (entity.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN ||
                        entity.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA)
                    {
                        #region Cập nhật tổng chi tiêu và Tổng đơn hàng của nhà cung cấp
                        var supplierEntity = await _dbContext.sm_Supplier.Where(x => x.Id == entity.SupplierId).FirstOrDefaultAsync();

                        var purchaseOrdersHasDone = _dbContext.sm_PurchaseOrder
                            .Where(x => x.SupplierId == entity.SupplierId && x.PaymentStatusCode == PaymentStatusConstants.DA_THANH_TOAN).ToList();

                        var purchaseOrdersHasHalf = _dbContext.sm_PurchaseOrder
                            .Where(x => x.SupplierId == entity.SupplierId && x.PaymentStatusCode == PaymentStatusConstants.THANH_TOAN_MOT_NUA).ToList();

                        if (supplierEntity != null)
                        {
                            supplierEntity.OrderCount = purchaseOrdersHasDone.Count + purchaseOrdersHasHalf.Count;
                            supplierEntity.ExpenseAmount = purchaseOrdersHasDone.Sum(x => x.PaidAmount ?? 0) + purchaseOrdersHasHalf.Sum(x => x.PaidAmount ?? 0);
                            await _dbContext.SaveChangesAsync();
                        }
                        #endregion
                    }
                }

                var result = await GetById(entity.Id);

                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<PurchaseOrderViewModel>(result.Data, "Thanh toán thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<PurchaseOrderViewModel>(ex);
            }
        }

        /// <summary>
        /// Hàm xuất file danh sách đơn nhập hàng excel theo query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<string>> ExportListToExcel(PurchaseOrderQueryModel query)
        {
            try
            {
                // Tạo predicate để lọc dựa trên các tham số trong query
                var predicate = BuildQuery(query);

                // Lấy danh sách đơn nhập hàng từ cơ sở dữ liệu dựa trên lọc và phân trang
                var purchaseOrders = await _dbContext.sm_PurchaseOrder.AsNoTracking()
                    .Include(x => x.Items)
                    .Include(x => x.sm_Supplier)
                    .Include(x => x.mk_DuAn)
                    .Include(x => x.sm_Construction)
                    .Where(predicate)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .GetPageAsync(query);

                // Kiểm tra nếu không có dữ liệu trả về trong trang hiện tại
                if (purchaseOrders == null || purchaseOrders.Content == null || purchaseOrders.Content.Count == 0)
                    return Helper.CreateNotFoundResponse<string>("Không có đơn nhập hàng nào tồn tại trong hệ thống.");

                // Đặt tên file và đường dẫn template
                var fileName = $"danh sách đơn nhập hàng_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}.xlsx";
                var filePath = Path.Combine(_staticsFolder, fileName);
                var templatePath = Path.Combine(_staticsFolder, "excel-template/PurchaseOrderTemplate.xlsx");

                if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                    return Helper.CreateBadRequestResponse<string>("Không tìm thấy file template");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Mở template Excel và điền dữ liệu vào file
                using (var package = new ExcelPackage(new FileInfo(templatePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Sử dụng worksheet đầu tiên

                    // Đặt độ rộng cố định cho cột A (cột STT) là 8
                    worksheet.Column(1).Width = 8;

                    // Điền dữ liệu vào bảng Excel (giả sử bắt đầu từ hàng thứ 4)
                    int startRow = 4;
                    int index = 1;

                    foreach (var order in purchaseOrders.Content)
                    {
                        worksheet.Cells[startRow, 1].Value = index; // STT
                        worksheet.Cells[startRow, 2].Value = order.CreatedOnDate.ToString("dd/MM/yyyy"); // Ngày tạo
                        worksheet.Cells[startRow, 3].Value = order.LastModifiedOnDate?.ToString("dd/MM/yyyy"); // Ngày cập nhật
                        worksheet.Cells[startRow, 4].Value = order.OrderCode; // Mã đơn nhập

                        // Kiểm tra trạng thái và gán tên trạng thái
                        string statusName;
                        switch (order.StatusCode)
                        {
                            case PurchaseOrderConstants.StatusCode.FINALIZED:
                                statusName = "Đang giao dịch";
                                break;
                            case PurchaseOrderConstants.StatusCode.COMPLETED:
                                statusName = "Hoàn thành";
                                break;
                            case PurchaseOrderConstants.StatusCode.CANCELLED:
                                statusName = "Đã hủy";
                                break;
                            default:
                                statusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }
                        worksheet.Cells[startRow, 5].Value = statusName; // Gán tên trạng thái
                        
                        // Kiểm tra trạng thái và gán tên trạng thái
                        string paymentStatusName;
                        switch (order.PaymentStatusCode)
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
                        
                        worksheet.Cells[startRow, 6].Value = paymentStatusName;
                        
                        // Kiểm tra trạng thái và gán tên trạng thái
                        string importStatusName;
                        switch (order.ImportStatusCode)
                        {
                            case PurchaseOrderConstants.CHUA_NHAP:
                                importStatusName = "Chưa nhập";
                                break;
                            case PurchaseOrderConstants.NHAP_HANG:
                                importStatusName = "Đã nhập";
                                break;
                            default:
                                importStatusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }

                        worksheet.Cells[startRow, 7].Value = importStatusName;
                        worksheet.Cells[startRow, 8].Value = order.Total; // Giá trị đơn
                        worksheet.Cells[startRow, 9].Value = CodeTypeCollection.Instance.FetchCode(order.WareCode, LanguageConstants.Default, order.TenantId)?.Title ?? null; // Kho
                        worksheet.Cells[startRow, 10].Value = order.CreatedByUserName; // Người tạo
                        worksheet.Cells[startRow, 11].Value = order.LastModifiedByUserName; // Người cập nhật cuối
                        worksheet.Cells[startRow, 12].Value = order.sm_Supplier?.Code; // Mã nhà cung cấp
                        worksheet.Cells[startRow, 13].Value = order.sm_Supplier?.Name; // Tên nhà cung cấp
                        worksheet.Cells[startRow, 14].Value = $"{order.sm_Supplier?.Address}" +
                            $"{(string.IsNullOrEmpty(order.sm_Supplier?.WardName) ? "" : $", {order.sm_Supplier?.WardName}")}" +
                            $"{(string.IsNullOrEmpty(order.sm_Supplier?.DistrictName) ? "" : $", {order.sm_Supplier?.DistrictName}")}" +
                            $"{(string.IsNullOrEmpty(order.sm_Supplier?.ProvinceName) ? "" : $", {order.sm_Supplier?.ProvinceName}")}"; // Địa chỉ nhà cung cấp
                        worksheet.Cells[startRow, 15].Value = order.sm_Supplier?.PhoneNumber; // Số điện thoại
                        worksheet.Cells[startRow, 16].Value = order.mk_DuAn?.TenDuAn; // Tên dự án
                        worksheet.Cells[startRow, 17].Value = order.Note; // Ghi chú

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
