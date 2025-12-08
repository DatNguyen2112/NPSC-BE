using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Business.Services.InventoryNote;
using NSPC.Business.Services.NhaCungCap;
using NSPC.Business.Services.StockTransaction;
using NSPC.Business.Services.VatTu;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.VatTu;
using NSPC.Data.Entity;
using Serilog;
using System.Linq.Expressions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static NSPC.Common.Helper;
using System.Xml.XPath;

namespace NSPC.Business.Services
{
    public class SupplierReturnHandler : ISupplierReturnHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IDebtTransactionHandler _debtTransactionHandler;
        private readonly ICashbookTransactionHandler _cashbookTransactionHandler;
        private readonly IStockTransactionHandler _stockTransactionHandler;
        private readonly IInventoryNoteHandler _inventoryNoteHandler;
        private readonly string _staticsFolder;

        public SupplierReturnHandler
            (SMDbContext dbContext, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, 
            IDebtTransactionHandler debtTransactionHandler, 
            ICashbookTransactionHandler cashbookTransactionHandler, 
            IStockTransactionHandler stockTransactionHandler, 
            IInventoryNoteHandler inventoryNoteHandler)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _debtTransactionHandler = debtTransactionHandler;
            _cashbookTransactionHandler = cashbookTransactionHandler;
            _stockTransactionHandler = stockTransactionHandler;
            _inventoryNoteHandler = inventoryNoteHandler;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
        }

        public async Task<Response<SupplierReturnViewModel>> Create(SupplierReturnCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                List<sm_Product> allOrderProductItems = new List<sm_Product>();

                #region Validated

                // Validate EntityId
                if (!await _dbContext.sm_Supplier.AnyAsync(x => x.Id == model.EntityId))
                {
                    return Helper.CreateNotFoundResponse<SupplierReturnViewModel>
                        ($"Supplier Id: {model.EntityId} not found.");
                }

                //Validate Product Item Model
                //1.Product ton tai trong he thong
                //2.So luong khong duoc < 0
                //3.So luong khong duoc vuot qua so luong goc
                //4.Phai chon san pham de tra hang
                if (model.OrderItems != null && model.OrderItems.Count > 0)
                {
                    //Fill product
                    var allProductIds = model.OrderItems.Select(x => x.ProductId).ToList();
                    allOrderProductItems = await _dbContext.sm_Product.AsNoTracking()
                                                  .Where(x => allProductIds.Contains(x.Id))
                                                  .ToListAsync();
                    var returnedTotalQuantity = model.OrderItems.Sum(x => x.ReturnedQuantity);

                    if (model.OrderItems.Any(x => x.ReturnedQuantity < 0))
                    {
                        return Helper.CreateBadRequestResponse<SupplierReturnViewModel>
                                    ("Danh sách sản phẩm không được có số lượng hoàn trả nhỏ hơn 0.");
                    }

                    if (model.OrderItems.Count == 1)
                    {
                        if (model.OrderItems.Any(x => x.ReturnedQuantity == 0))
                        {
                            return Helper.CreateBadRequestResponse<SupplierReturnViewModel>
                                        ("Vui lòng chọn sản phẩm để hoàn trả");
                        }
                    }

                    if (model.OrderItems.Any(x => x.ReturnedQuantity > x.InitialQuantity))
                    {
                        return Helper.CreateBadRequestResponse<SupplierReturnViewModel>
                                   ("Danh sách sản phẩm không được có số lượng hoàn trả lớn hơn số lượng gốc.");
                    }
                    
                    if (returnedTotalQuantity <= 0)
                    {
                        return Helper.CreateBadRequestResponse<SupplierReturnViewModel>
                            ("Vui lòng nhập số lượng cần hoàn trả");
                    }

                    foreach (var orderItem in model.OrderItems)
                    {
                        var product = allOrderProductItems.Where(x => x.Id == orderItem.ProductId).FirstOrDefault();

                        if (product == null)
                        {
                            return Helper.CreateBadRequestResponse<SupplierReturnViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {orderItem.ProductId}");
                        }

                        if (orderItem.ReturnedUnitPrice < 0)
                        {
                            return Helper.CreateBadRequestResponse<SupplierReturnViewModel>
                                ("Danh sách sản phẩm không được có đơn giá trả nhỏ hơn 0");
                        }
                    }
                }
                #endregion
                
                #region Fill Item's Line No
                if (model.OrderItems != null && model.OrderItems.Count > 0)
                {
                    model.OrderItems = model.OrderItems.OrderBy(x => x.LineNo).ToList();
                    for (int i = 0; i < model.OrderItems.Count; i++)
                        model.OrderItems[i].LineNo = i + 1;
                }
                #endregion

                // Mapping và tạo entity
                var entity = _mapper.Map<sm_Return_Order>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.RefundSubTotal = 0;
                entity.EntityTypeCode = OrderReturnConstants.EntityTypeCode.SUPPLIER;
                entity.EntityTypeName = OrderReturnConstants.EntityTypeName.NHA_CUNG_CAP;
                entity.StatusCode = OrderReturnConstants.StatusCode.RETURNING;
                entity.OrderCode = await GetNewCode(OrderReturnConstants.SupplierReturnOrderCodePrefix);
                entity.Note = model.Note;
                entity.PaidAmount = 0;
                entity.RefundSubTotal = 0;
                entity.RemainingRefundAmount = 0;

                #region Tính toán OrderItem
                foreach (var orderItem in entity.OrderItems)
                {
                    var product = allOrderProductItems.FirstOrDefault(x => x.Id == orderItem.ProductId);
                    var allStockTransactions =
                        await _stockTransactionHandler.GetByIdAndWareCode(orderItem.ProductId, entity.WareCode);
                    var productInventoryEntity = _dbContext.sm_ProductInventory.FirstOrDefault(x =>
                        x.ProductId == orderItem.ProductId && x.WarehouseCode == entity.WareCode);
                    
                    if (allStockTransactions.Data != null && allStockTransactions.Data.Count > 0)
                    {
                        if (productInventoryEntity != null)
                        {
                            if (orderItem.ReturnedQuantity > productInventoryEntity.SellableQuantity)
                            {
                                return Helper.CreateBadRequestResponse<SupplierReturnViewModel>
                                ($"Số lượng trả hàng nhà cung cấp đang lớn hơn số lượng có thể giao dịch trong " +
                                 $"{allStockTransactions.Data[0].WareName}, vui lòng kiểm tra lại");
                            }
                        }
                    }

                    if (product != null)
                    {
                        orderItem.ProductCode = product.Code;
                        orderItem.ProductName = product.Name;
                        orderItem.Unit = product.Unit;
                        orderItem.LineAmount = orderItem.ReturnedQuantity * orderItem.ReturnedUnitPrice;
                        orderItem.RemainingQuantity = orderItem.InitialQuantity - orderItem.ReturnedQuantity;
                        
                        await UpdateOrderAfterReturn(entity.OriginalDocumentId, orderItem.ProductId, orderItem.RemainingQuantity);
                    }
                }

                entity.RefundSubTotal = entity.OrderItems.Sum(x => x.LineAmount);
                #endregion

                #region Tính toán Payment
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
                entity.RemainingRefundAmount = entity.RefundSubTotal - entity.PaidAmount;
                #endregion
                
                // Validate List Payment
                // Rules:
                // 1. Payment method phải chính xác
                // 2. Số tiền nhập không được < 0
                // 3. Điền supplier từ entity
                if (model.ListPayment != null && model.ListPayment.Count > 0)
                {
                    // Validate: Payment Method phải chính xác
                    //var validPaymentMethods = _dbContext.sm_CodeType.Where(x => x.Type ==

                    if (model.ListPayment.Any(x => x.LinePaidAmount < 0))
                        return Helper.CreateBadRequestResponse<SupplierReturnViewModel>
                            ("Danh sách thanh toán không được có số tiền nhỏ hơn 0.");
                    
                    if (model.ListPayment.Any(x => x.LinePaidAmount == 0 || x.LinePaidAmount == null))
                        return Helper.CreateBadRequestResponse<SupplierReturnViewModel>
                            ("Vui lòng nhập vào số tiền thanh toán");

                    if (model.ListPayment.Any(x => x.LinePaidAmount > entity.RefundSubTotal))
                    {
                        return Helper.CreateBadRequestResponse<SupplierReturnViewModel>(string.Format("Danh sách thanh toán không được có số tiền lớn hơn số tiền cần hoàn trả"));
                    }
                    
                    if (model.ListPayment.Sum(x => x.LinePaidAmount) > entity.RefundSubTotal)
                    {
                        return Helper.CreateBadRequestResponse<SupplierReturnViewModel>(string.Format("Số tiền hoàn trả không được có lớn hơn tổng tiền"));
                    }
                    
                    // Fill supplier from model
                    model.ListPayment.ForEach(x => x.SupplierId = model.EntityId);
                }

                #region Trạng thái đơn và trạng thái thanh toán
                // Đã thanh toán
                if (entity.RemainingRefundAmount == 0)
                    entity.RefundStatusCode = OrderReturnConstants.RefundStatusCode.PAID;
                else if (entity.PaidAmount != 0)
                    entity.RefundStatusCode = OrderReturnConstants.RefundStatusCode.PARTIAL;
                else if (entity.PaidAmount == 0)
                    entity.RefundStatusCode = OrderReturnConstants.RefundStatusCode.UNPAID;
                #endregion

                _dbContext.sm_Return_Order.Add(entity);

                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Check xem đơn hàng đã hoàn trả hết chưa thì update trường IsReturned
                    var purchaseOrder = await _dbContext.sm_PurchaseOrder.FirstOrDefaultAsync(x => x.Id == entity.OriginalDocumentId);

                    if (purchaseOrder != null)
                    {
                        if (entity.OrderItems != null && entity.OrderItems.Count > 0)
                        {
                            if (entity.OrderItems.Any(x => x.RemainingQuantity > 0))
                            {
                                purchaseOrder.IsReturned = false;
                            }
                            else
                            {
                                purchaseOrder.IsReturned = true;
                            }
                        }
                        
                        await _dbContext.SaveChangesAsync();
                    }
                    #endregion

                    #region Sổ quỹ (Cashbook Transaction)
                    // Điền tên supplier vào sổ quỹ
                    var supplier = await _dbContext.sm_Supplier.AsNoTracking()
                                        .Where(x => x.Id == entity.EntityId)
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
                                // sau khi trả hàng NCC sẽ thực hiện xuất kho
                                OriginalDocumentType = OriginDocumentCashbookTransactionConstants.PURCHASE_RETURN,
                                OriginalDocumentCode = entity.OrderCode,
                                ProjectId = entity.ProjectId,
                                // sau khi khách hàng trả hàng sẽ nhập lại vào kho và tạo phiếu thu
                                TransactionTypeCode = CashbookTransactionConstants.ReceiptVoucherType,
                                PurposeCode = CashbookTransactionConstants.AUTO_RECEIPT_PURPOSE,
                                PaymentMethodCode = item.PaymentMethod,
                                Amount = item.LinePaidAmount ?? 0,
                                EntityName = supplier?.Name,
                                ReceiptDate = entity.CreatedOnDate,
                                EntityTypeCode = EntityTypeCodeConstants.SUPPLIER,
                                EntityTypeName = EntityTypeNameConstants.SUPPLIER,
                                Description = $"Phiếu thu tự động tạo khi nhận tiền hoàn từ nhà cung cấp cho đơn trả hàng {entity.OrderCode}",
                                IsDebt = true
                            }, currentUser);
                        }
                    }
                    #endregion
                    
                    #region Cập nhật lại số lượng có thể bán
                    foreach (var item in entity.OrderItems)
                    {
                        var productEntity =
                            await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                        var productInventoryEntity = await _dbContext.sm_ProductInventory
                            .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseCode == entity.WareCode);
                       
                        if (productInventoryEntity != null)
                        {
                            productInventoryEntity.SellableQuantity -= item.ReturnedQuantity;
                        }
                        
                        if (productEntity != null)
                        {
                            productEntity.SellableQuantity -= item.ReturnedQuantity;
                            
                        }
                        
                        await _dbContext.SaveChangesAsync();
                    }
                    #endregion
                }

                var result = await GetById(entity.Id);
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<SupplierReturnViewModel>(result.Data, "Tạo đơn trả hàng thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<SupplierReturnViewModel>(ex);
            };
        }

        private async Task<string> GetNewCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_Return_Order.AsNoTracking().Where(x => x.OrderCode.Contains(code)).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

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

        /// <summary>
        /// Update số lượng hoàn trả còn lại
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="ProductId"></param>
        /// <param name="QuantityToReturn"></param>
        /// <returns></returns>
        private async Task<Response> UpdateOrderAfterReturn(Guid OrderId, Guid ProductId, decimal QuantityToReturn)
        {
            try
            {
                // Tìm đơn hàng gốc 
                var order = await _dbContext.sm_PurchaseOrder.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == OrderId);

                if (order == null)
                {
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>(string.Format("Đơn hàng không tồn tại trong hệ thống!"));
                }

                var orderItem = order.Items.FirstOrDefault(x => x.ProductId == ProductId);

                if (orderItem == null)
                {
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>(string.Format("Không tìm thấy sản phẩm trong đơn hàng"));
                }
                else
                {
                    orderItem.RemainingQuantity = QuantityToReturn;

                    if (orderItem.RemainingQuantity > 0)
                    {
                        orderItem.IsReturnedItem = false;
                    }
                    else
                    {
                        orderItem.IsReturnedItem = true;
                    }

                    await _dbContext.SaveChangesAsync();
                }

                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<PurchaseOrderViewModel>(ex);
            }

        }
        
        // <summary>
        /// Update số lượng hoàn trả còn lại
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="ProductId"></param>
        /// <param name="QuantityToReturn"></param>
        /// <returns></returns>
        private async Task<Response> UpdateOrderAfterCancelled(Guid OrderId, Guid ProductId, decimal QuantityToCancelled)
        {
            try
            {
                // Tìm đơn hàng gốc 
                var order = await _dbContext.sm_PurchaseOrder.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == OrderId);

                if (order == null)
                {
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>(string.Format("Đơn hàng không tồn tại trong hệ thống!"));
                }

                var orderItem = order.Items.FirstOrDefault(x => x.ProductId == ProductId);

                if (orderItem == null)
                {
                    return Helper.CreateNotFoundResponse<PurchaseOrderViewModel>(string.Format("Không tìm thấy sản phẩm trong đơn hàng"));
                }
                else
                {
                    orderItem.RemainingQuantity += QuantityToCancelled;

                    if (orderItem.RemainingQuantity > 0)
                    {
                        orderItem.IsReturnedItem = false;
                    }
                    else
                    {
                        orderItem.IsReturnedItem = true;
                    }

                    await _dbContext.SaveChangesAsync();
                }

                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<PurchaseOrderViewModel>(ex);
            }

        }

        /// <summary>
        /// Lấy chi tiết đơn trả hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<SupplierReturnViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Return_Order
                                    .Include(x => x.OrderItems)
                                    .Include(x => x.mk_DuAn)
                                    .Include(x => x.sm_Construction)
                                    .Include(x => x.sm_Contract)
                                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<SupplierReturnViewModel>("Đơn trả hàng không tồn tại trong hệ thống.");

                var result = _mapper.Map<SupplierReturnViewModel>(entity);

                return new Response<SupplierReturnViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<SupplierReturnViewModel>(ex);
            }
        }

        /// <summary>
        /// Xóa đơn trả hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<SupplierReturnViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Return_Order.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<SupplierReturnViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<SupplierReturnViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<SupplierReturnViewModel>(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách đơn trả hàng
        /// </summary>
        /// <param name="query"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<Pagination<SupplierReturnViewModel>>> GetPage(SupplierReturnQueryModel query, RequestUser currentUser)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_Return_Order
                    .Include(x => x.OrderItems)
                    .Include(x => x.mk_DuAn)
                    .Include(x => x.sm_Construction)
                    .Include(x => x.sm_Contract)
                    .Where(predicate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<SupplierReturnViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<SupplierReturnViewModel>>(ex);
            }
        }

        private Expression<Func<sm_Return_Order, bool>> BuildQuery(SupplierReturnQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            
            var predicate = PredicateBuilder.New<sm_Return_Order>(true);
            predicate.And(x => x.EntityTypeCode == OrderReturnConstants.EntityTypeCode.SUPPLIER);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.OrderCode.ToLower().Contains(query.FullTextSearch.ToLower())
                                    || s.OriginalDocumentCode.ToLower().Contains(query.FullTextSearch.ToLower())
                                    || s.EntityName.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.ReasonCode))
                predicate.And(s => s.ReasonCode == query.ReasonCode);

            if (!string.IsNullOrEmpty(query.EntityTypeCode))
                predicate.And(s => s.EntityTypeCode == query.EntityTypeCode);
            
            if (!string.IsNullOrEmpty(query.OriginalDocumentCode))
                predicate.And(s => s.OriginalDocumentCode.Contains(query.OriginalDocumentCode));
            
            if (!string.IsNullOrEmpty(query.OrderCode))
                predicate.And(s => s.OrderCode.Contains(query.OrderCode));

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date < query.DateRange[1].Value.AddDays(1));
            }

            if (query.EntityId.HasValue)
            {
                predicate = predicate.And(x => x.EntityId == query.EntityId);
            }
            
            if (!currentUser.ListRights.Contains("PURCHASEORDERRETURN." + RightActionConstants.VIEWALL))
                predicate.And(s => s.CreatedByUserId == currentUser.UserId);

            return predicate;
        }

        /// <summary>
        /// Hoàn tiền đơn trả hàng NCC
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response<SupplierReturnViewModel>> RefundPaymentOrder(Guid id, RefundSupplierCreateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Return_Order
                    .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Sm_Product)
                    .Include(x => x.mk_DuAn)
                    .Include(x => x.sm_Construction)
                    .Include(x => x.sm_Contract)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<SupplierReturnViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                #region Validated ChargePayment
                if (model.ListPayment != null && model.ListPayment.Count > 0)
                {
                    foreach (var item in model.ListPayment)
                    {
                        if (item.LinePaidAmount < 0)
                        {
                            return Helper.CreateBadRequestResponse<SupplierReturnViewModel>(string.Format("Tiền hoàn trả không được âm"));
                        }
                        
                        if (item.LinePaidAmount == 0 || item.LinePaidAmount == null)
                        {
                            return Helper.CreateBadRequestResponse<SupplierReturnViewModel>(string.Format("Vui lòng nhập số tiền thanh toán"));
                        }
                    
                        if (item.LinePaidAmount > entity.RemainingRefundAmount)
                        {
                            return Helper.CreateBadRequestResponse<SupplierReturnViewModel>(string.Format("Tiền hoàn trả không được lớn hơn tổng tiền cần phải hoàn trả"));
                        }
                    }
                }
                #endregion

                if (model.ListPayment.Count != 0)
                {
                    var supplier = await _dbContext.sm_Supplier.AsNoTracking()
                                        .Where(x => x.Id == entity.EntityId)
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
                            // sau khi trả hàng nhà cung cấp sẽ xuất kho
                            OriginalDocumentType = OriginDocumentCashbookTransactionConstants.PURCHASE_RETURN,
                            OriginalDocumentCode = entity.OrderCode,
                            ProjectId = entity.ProjectId,
                            // sau khi trả hàng NCC sẽ xuất kho và tạo phiếu thu
                            TransactionTypeCode = CashbookTransactionConstants.ReceiptVoucherType,
                            PurposeCode = CashbookTransactionConstants.AUTO_RECEIPT_PURPOSE,
                            PaymentMethodCode = item.PaymentMethod,
                            Amount = item.LinePaidAmount ?? 0,
                            EntityName = supplier?.Name,
                            ReceiptDate = DateTime.Now,
                            EntityTypeCode = EntityTypeCodeConstants.SUPPLIER,
                            EntityTypeName = EntityTypeNameConstants.SUPPLIER,
                            Description = $"Phiếu thu tự động tạo khi nhận tiền hoàn từ nhà cung cấp cho đơn trả hàng {entity.OrderCode}",
                            IsDebt = true
                        }, currentUser);
                    }

                    foreach (var item in entity.ListPayment)
                    {
                        model.ListPayment.Add(item);
                    }

                }

                entity.ListPayment = model.ListPayment;

                // Tính toán số tiền đã trả
                entity.PaidAmount = model.ListPayment.Sum(x => x.LinePaidAmount);
                entity.RemainingRefundAmount = entity.RefundSubTotal - entity.PaidAmount;
                entity.LastModifiedOnDate = DateTime.Now;

                if (entity.RemainingRefundAmount == 0 || (entity.PaidAmount == entity.RefundSubTotal))
                {
                    entity.RefundStatusCode = OrderReturnConstants.RefundStatusCode.PAID;
                }
                else if (entity.PaidAmount != 0 && entity.PaidAmount < entity.RefundSubTotal)
                {
                    entity.RefundStatusCode = OrderReturnConstants.RefundStatusCode.PARTIAL;
                }
                else if (entity.PaidAmount == 0)
                {
                    entity.RefundStatusCode = OrderReturnConstants.RefundStatusCode.UNPAID;
                }

                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<SupplierReturnViewModel>(entity), "Hoàn trả tiền thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<SupplierReturnViewModel>(ex);
            }
        }


        /// <summary>
        /// Xuất kho trả hàng NCC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<SupplierReturnViewModel>> SupplierReturnOrder(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Return_Order
                    .Include(x => x.OrderItems)
                    .Include(x => x.mk_DuAn)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<SupplierReturnViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                entity.StatusCode = OrderReturnConstants.StatusCode.RETURNED;
                entity.LastModifiedOnDate = DateTime.Now;

                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    if (entity.StatusCode == OrderReturnConstants.StatusCode.RETURNED)
                    {
                        #region Tạo công nợ khi thực hiện nhập kho khách trả hàng
                        await _debtTransactionHandler.CreateDebtOrderReturn(entity.Id, currentUser);
                        #endregion

                        #region Cập nhật lại tồn kho của sản phẩm
                        foreach (var item in entity.OrderItems)
                        {
                            var productItem = await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                            var initialQuantity = await _dbContext.sm_Product.Where(x => x.Id == item.ProductId).OrderByDescending(x => x.CreatedOnDate).Select(x => x.InitialStockQuantity).FirstOrDefaultAsync();
                            var productInventoryEntity = await _dbContext.sm_ProductInventory
                                .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseCode == entity.WareCode);

                            if (productInventoryEntity != null)
                            {
                                productInventoryEntity.SellableQuantity = initialQuantity - item.ReturnedQuantity;
                                await _dbContext.SaveChangesAsync();
                            }

                            if (productItem != null)
                            {
                                productItem.InitialStockQuantity = initialQuantity - item.ReturnedQuantity;
                                productItem.SellableQuantity = initialQuantity - item.ReturnedQuantity;
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        #endregion


                        #region Taọ bản ghi StockTransaction(Xuất nhập tồn)
                        foreach (var item in entity.OrderItems)
                        {
                            var productItem = await _dbContext.sm_Product
                                .Where(x => x.Id == item.ProductId)
                                .Select(x => new VatTuViewModel { Code = x.Code, Name = x.Name, InitialStockQuantity = x.InitialStockQuantity })
                                .FirstOrDefaultAsync();

                            var newProduct = new StockTransactionCreateModel
                            {
                                ReceiptInventoryQuantity = 0,
                                ExportInventoryQuantity = item.ReturnedQuantity,
                                SellableQuantity = -item.ReturnedQuantity,
                                InitialStockQuantity = productItem.InitialStockQuantity,
                                ProductCode = productItem.Code,
                                ProductName = productItem.Name,
                                ProductId = item.ProductId,
                                WareCode = entity.WareCode,
                                OriginalDocumentType = OrderReturnConstants.OriginalDocumentType.PURCHASE_RETURN,
                                OriginalDocumentCode = entity.OrderCode,
                                OriginalDocumentId = entity.Id,
                                UnitPrice = item.ReturnedUnitPrice,
                                Unit = item.Unit,
                                CreatedByUserId = entity.CreatedByUserId,
                                Action = OrderReturnConstants.ActionCode.SUPPLIER_RETURN,
                            };
                            await _stockTransactionHandler.Create(newProduct);
                        }
                        #endregion

                        #region Tạo phiếu xuất kho khi thực hiện nhận hàng hoàn trả
                        
                       var res =  await _inventoryNoteHandler.CreateSupplierReturn(entity.Id, currentUser);

                        if (!res.IsSuccess)
                        {
                            return Helper.CreateBadRequestResponse<SupplierReturnViewModel>(res.Message);
                        }

                        #endregion
                    }
                }

                var result = await GetById(id);
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<SupplierReturnViewModel>(result.Data, "Xuất kho trả hàng nhà cung cấp thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<SupplierReturnViewModel>(ex);
            }
        }

        /// <summary>
        /// Hủy đơn trả hàng NCC
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response<SupplierReturnViewModel>> CancelledReturnedOrder(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Return_Order.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<SupplierReturnViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                entity.StatusCode = OrderReturnConstants.StatusCode.CANCELLED;
                entity.LastModifiedOnDate = DateTime.Now;

                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Cập nhật lại số lượng có thể bán
                    foreach (var item in entity.OrderItems)
                    {
                        var productEntity =
                            await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                        var productInventoryEntity = await _dbContext.sm_ProductInventory
                            .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseCode == entity.WareCode);

                        #region Sau khi hủy thì sẽ tăng lại về số lượng ban đầu
                        // Gọi hàm xử lý
                        await UpdateOrderAfterCancelled(entity.OriginalDocumentId, item.ProductId,
                            item.ReturnedQuantity);
                        #endregion
                        
                        if (productInventoryEntity != null)
                        {
                            productInventoryEntity.SellableQuantity += item.ReturnedQuantity;
                        } 
                        
                        if (productEntity != null)
                        {
                            productEntity.SellableQuantity += item.ReturnedQuantity;
                        }
                        
                        await _dbContext.SaveChangesAsync();
                    }
                    #endregion
                }

                var result = await GetById(id);
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<SupplierReturnViewModel>(result.Data, "Hủy đơn trả hàng thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<SupplierReturnViewModel>(ex);
            }
        }
        
        /// <summary>
        /// Hàm xuất file danh sách phiếu trả hàng nhà cung cấp tồn excel theo query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<string>> ExportListToExcel(SupplierReturnQueryModel query)
        {
            try
            {
                // Tạo predicate để lọc dựa trên các tham số trong query
                var predicate = BuildQuery(query);

                // Lấy danh sách phiếu kiểm kho từ cơ sở dữ liệu dựa trên lọc và phân trang
                var supplierReturnOrders = await _dbContext.sm_Return_Order.AsNoTracking()
                    .Include(x => x.OrderItems)
                    .Where(predicate)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .GetPageAsync(query);

                // Kiểm tra nếu không có dữ liệu trả về trong trang hiện tại
                if (supplierReturnOrders == null || supplierReturnOrders.Content == null ||
                    supplierReturnOrders.Content.Count == 0)
                    return Helper.CreateNotFoundResponse<string>("Không có phiếu trả hàng nào tồn tại trong hệ thống.");

                // Đặt tên file và đường dẫn template
                var fileName = $"danh sách phiếu trả hàng NCC_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}.xlsx";
                var filePath = Path.Combine(_staticsFolder, fileName);
                var templatePath = Path.Combine(_staticsFolder, "excel-template/SupplierReturnTemplate.xlsx");

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

                    foreach (var order in supplierReturnOrders.Content)
                    {
                        worksheet.Cells[startRow, 1].Value = index; // STT
                        worksheet.Cells[startRow, 2].Value = order.CreatedOnDate.ToString("dd/MM/yyyy"); // Ngày tạo
                        worksheet.Cells[startRow, 3].Value = order.LastModifiedOnDate?.ToString("dd/MM/yyyy"); // Ngày nhận hàng
                        worksheet.Cells[startRow, 4].Value =
                            order.OrderCode; // Mã đơn trả hàng

                        // Kiểm tra trạng thái và gán tên trạng thái
                        string statusName;
                        switch (order.StatusCode)
                        {
                            case OrderReturnConstants.StatusCode.RETURNING:
                                statusName = "Chưa nhận";
                                break;
                            case OrderReturnConstants.StatusCode.RETURNED:
                                statusName = "Hoàn thành";
                                break;
                            default:
                                statusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }
                        worksheet.Cells[startRow, 5].Value = statusName; // Gán tên trạng thái
                        worksheet.Cells[startRow, 6].Value = order.RefundSubTotal;
                        worksheet.Cells[startRow, 7].Value = CodeTypeCollection.Instance
                            .FetchCode(order.WareCode, LanguageConstants.Default, order.TenantId)?.Title ?? null; // Kho;
                        worksheet.Cells[startRow, 8].Value = order.CreatedByUserName; // Người tạo
                        worksheet.Cells[startRow, 9].Value = order.EntityCode; // Mã nhà cung cấp
                        worksheet.Cells[startRow, 10].Value = order.EntityName; // Tên nhà cung cấp
                        worksheet.Cells[startRow, 11].Value = CodeTypeCollection.Instance
                            .FetchCode(order.ReasonCode, LanguageConstants.Default, order.TenantId)?.Title ?? null; // Lý do trả hàng

                        startRow++;
                        index++;
                    }

                    int lastDataRow = startRow - 1;

                    // Thêm đường viền cho các ô đã điền dữ liệu
                    using (var range =
                           worksheet.Cells[4, 1, lastDataRow,
                               11]) // điều chỉnh cột cuối (18) tùy theo số lượng cột bạn có
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
