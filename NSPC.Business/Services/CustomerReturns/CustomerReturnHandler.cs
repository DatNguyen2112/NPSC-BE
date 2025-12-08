using AutoMapper;
using Microsoft.AspNetCore.Http;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.VatTu;
using NSPC.Data.Entity;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;
using System.Linq.Expressions;
using LinqKit;
using MongoDB.Driver.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.NhaCungCap;
using NSPC.Business.Services.StockTransaction;
using NSPC.Business.Services.VatTu;
using NSPC.Business.Services.InventoryNote;

namespace NSPC.Business.Services
{
    public class CustomerReturnHandler : ICustomerReturnHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IDebtTransactionHandler _debtTransactionHandler;
        private readonly ICashbookTransactionHandler _cashbookTransactionHandler;
        private readonly IStockTransactionHandler _stockTransactionHandler;
        private readonly IInventoryNoteHandler _inventoryNoteHandler;
        private readonly string _staticsFolder;

        public CustomerReturnHandler
            (SMDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IDebtTransactionHandler debtTransactionHandler,
            ICashbookTransactionHandler cashbookTransactionHandler,
            IStockTransactionHandler stockTransactionHandler,
            IInventoryNoteHandler inventoryNoteHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _debtTransactionHandler = debtTransactionHandler;
            _cashbookTransactionHandler = cashbookTransactionHandler;
            _stockTransactionHandler = stockTransactionHandler;
            _inventoryNoteHandler = inventoryNoteHandler;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
        }

        /// <summary>
        /// Tạo đơn trả hàng
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<CustomerReturnViewModel>> Create(CustomerReturnCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                List<sm_Product> allOrderProductItems = new List<sm_Product>();

                #region Validated

                // Validate EntityId
                if (!await _dbContext.sm_Customer.AnyAsync(x => x.Id == model.EntityId))
                {
                    return Helper.CreateNotFoundResponse<CustomerReturnViewModel>
                        ($"Customer Id: {model.EntityId} not found.");
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
                        return Helper.CreateBadRequestResponse<CustomerReturnViewModel>
                                    ("Danh sách sản phẩm không được có số lượng hoàn trả nhỏ hơn 0.");
                    }

                    if (model.OrderItems.Count == 1)
                    {
                        if (model.OrderItems.Any(x => x.ReturnedQuantity == 0))
                        {
                            return Helper.CreateBadRequestResponse<CustomerReturnViewModel>
                                        ("Vui lòng chọn sản phẩm để hoàn trả");
                        }
                    }

                    if (returnedTotalQuantity <= 0)
                    {
                        return Helper.CreateBadRequestResponse<CustomerReturnViewModel>
                            ("Vui lòng nhập số lượng hoàn trả");
                    }

                    if (model.OrderItems.Any(x => x.ReturnedQuantity > x.InitialQuantity))
                    {
                        return Helper.CreateBadRequestResponse<CustomerReturnViewModel>
                                   ("Danh sách sản phẩm không được có số lượng hoàn trả lớn hơn số lượng gốc.");
                    }

                    foreach (var orderItem in model.OrderItems)
                    {
                        var product = allOrderProductItems.Where(x => x.Id == orderItem.ProductId).FirstOrDefault();

                        if (product == null)
                        {
                            return Helper.CreateBadRequestResponse<CustomerReturnViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {orderItem.ProductId}");
                        }

                        if (orderItem.ReturnedUnitPrice < 0)
                        {
                            return Helper.CreateBadRequestResponse<CustomerReturnViewModel>
                                ($"Danh sách sản phẩm không được có đơn giá trả nhỏ hơn 0");
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
                entity.EntityTypeCode = OrderReturnConstants.EntityTypeCode.CUSTOMER;
                entity.EntityTypeName = OrderReturnConstants.EntityTypeName.KHACH_HANG;
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.RefundSubTotal = 0;
                entity.StatusCode = OrderReturnConstants.StatusCode.RETURNING;
                entity.OrderCode = await GetNewCode(OrderReturnConstants.CustomerReturnOrderCodePrefix);
                entity.Note = model.Note;
                entity.PaidAmount = 0;
                entity.RefundSubTotal = 0;
                entity.RemainingRefundAmount = 0;

                #region Tính toán OrderItem
                foreach (var orderItem in entity.OrderItems)
                {
                    var product = allOrderProductItems.FirstOrDefault(x => x.Id == orderItem.ProductId);

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
                // 3. Điền customer từ entity
                if (model.ListPayment != null && model.ListPayment.Count > 0)
                {
                    // Validate: Payment Method phải chính xác
                    //var validPaymentMethods = _dbContext.sm_CodeType.Where(x => x.Type ==

                    if (model.ListPayment.Any(x => x.LinePaidAmount < 0))
                        return Helper.CreateBadRequestResponse<CustomerReturnViewModel>
                            ("Danh sách thanh toán không được có số tiền nhỏ hơn 0.");
                    
                    if (model.ListPayment.Any(x => x.LinePaidAmount == 0 || x.LinePaidAmount == null))
                        return Helper.CreateBadRequestResponse<CustomerReturnViewModel>
                            ("Vui lòng nhập số tiền thanh toán");
                    
                    if (model.ListPayment.Any(x => x.LinePaidAmount > entity.RefundSubTotal))
                        return Helper.CreateBadRequestResponse<CustomerReturnViewModel>
                            ("Danh sách thanh toán không được có số tiền lớn hơn số tiền đã thanh toán.");
                    
                    if (model.ListPayment.Sum(x => x.LinePaidAmount) > entity.RefundSubTotal)
                    {
                        return Helper.CreateBadRequestResponse<CustomerReturnViewModel>(string.Format("Số tiền hoàn trả không được có lớn hơn tổng tiền"));
                    }

                    // Fill supplier from model
                    model.ListPayment.ForEach(x => x.CustomerId = model.EntityId);
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
                    var salesOrder = await _dbContext.sm_SalesOrder.FirstOrDefaultAsync(x => x.Id == entity.OriginalDocumentId);

                    if (salesOrder != null)
                    {
                        if (entity.OrderItems != null && entity.OrderItems.Count > 0)
                        {
                            if (entity.OrderItems.Any(x => x.RemainingQuantity > 0))
                            {
                                salesOrder.IsReturned = false;
                            }
                            else
                            {
                                salesOrder.IsReturned = true;
                            }
                        }

                        await _dbContext.SaveChangesAsync();
                    }
                    #endregion

                    #region Sổ quỹ (Cashbook Transaction)
                    // Điền tên customer vào sổ quỹ
                    var customer = await _dbContext.sm_Customer.AsNoTracking()
                                        .Where(x => x.Id == entity.EntityId)
                                        .Select(x => new KhachHangViewModel { Id = x.Id, Name = x.Name, Code = x.Code })
                                        .FirstOrDefaultAsync();

                    if (entity.ListPayment != null && entity.ListPayment.Count() != 0)
                    {
                        foreach (var item in entity.ListPayment)
                        {
                            await _cashbookTransactionHandler.Create(new CashbookTransactionCreateUpdateModel()
                            {
                                EntityCode = customer.Code,
                                EntityId = customer.Id,
                                OriginalDocumentId = entity.Id,
                                ConstructionId = entity.ConstructionId,
                                ContractId = entity.ContractId,
                                // sau khi khách hàng trả hàng sẽ nhập lại vào kho
                                OriginalDocumentType = OriginDocumentCashbookTransactionConstants.SALES_RETURN,
                                OriginalDocumentCode = entity.OrderCode,
                                ProjectId = entity.ProjectId,
                                // sau khi khách hàng trả hàng sẽ nhập lại vào kho và tạo phiếu chi
                                TransactionTypeCode = CashbookTransactionConstants.PaymentVoucherType,
                                PurposeCode = SaleOrderStatusConstants.AUTO_PAYMENT,
                                PaymentMethodCode = item.PaymentMethod,
                                Amount = item.LinePaidAmount ?? 0,
                                EntityName = customer?.Name,
                                ReceiptDate = entity.CreatedOnDate,
                                EntityTypeCode = EntityTypeCodeConstants.CUSTOMER,
                                EntityTypeName = EntityTypeNameConstants.CUSTOMER,                           
                                IsDebt = true,
                                Description = $"Phiếu chi tự động tạo khi hoàn tiền cho khách hàng theo đơn trả hàng {entity.OrderCode}"
                            }, currentUser);
                        }
                    }
                    #endregion

                }

                var result = await GetById(entity.Id);
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<CustomerReturnViewModel>(result.Data, "Tạo đơn trả hàng thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<CustomerReturnViewModel>(ex);
            };
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
                var order = await _dbContext.sm_SalesOrder.Include(x => x.SalesOrderItems).FirstOrDefaultAsync(x => x.Id == OrderId);

                if (order == null)
                {
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>(string.Format("Đơn hàng không tồn tại trong hệ thống!"));
                }

                var orderItem = order.SalesOrderItems.FirstOrDefault(x => x.ProductId == ProductId);

                if (orderItem == null)
                {
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>(string.Format("Không tìm thấy sản phẩm trong đơn hàng"));
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
                return Helper.CreateExceptionResponse<SalesOrderViewModel>(ex);
            }

        }
        
        /// <summary>
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
                var order = await _dbContext.sm_SalesOrder.Include(x => x.SalesOrderItems).FirstOrDefaultAsync(x => x.Id == OrderId);

                if (order == null)
                {
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>(string.Format("Đơn hàng không tồn tại trong hệ thống!"));
                }

                var orderItem = order.SalesOrderItems.FirstOrDefault(x => x.ProductId == ProductId);

                if (orderItem == null)
                {
                    return Helper.CreateNotFoundResponse<SalesOrderViewModel>(string.Format("Không tìm thấy sản phẩm trong đơn hàng"));
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
                return Helper.CreateExceptionResponse<SalesOrderViewModel>(ex);
            }

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
        /// Lấy chi tiết đơn trả hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<CustomerReturnViewModel>> GetById(Guid id)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = await _dbContext.sm_Return_Order
                                    .Include(x => x.OrderItems)
                                    .Include(x => x.mk_DuAn)
                                    .Include(x => x.sm_Construction)
                                    .Include(x => x.sm_Contract)
                                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<CustomerReturnViewModel>("Đơn trả hàng không tồn tại trong hệ thống.");

                var result = _mapper.Map<CustomerReturnViewModel>(entity);

                return new Response<CustomerReturnViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<CustomerReturnViewModel>(ex);
            }
        }

        /// <summary>
        /// Xóa đơn trả hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<CustomerReturnViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Return_Order.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<CustomerReturnViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<CustomerReturnViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<CustomerReturnViewModel>(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách đơn trả hàng
        /// </summary>
        /// <param name="query"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<Pagination<CustomerReturnViewModel>>> GetPage(CustomerReturnQueryModel query, RequestUser currentUser)
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

                var result = _mapper.Map<Pagination<CustomerReturnViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<CustomerReturnViewModel>>(ex);
            }
        }

        private Expression<Func<sm_Return_Order, bool>> BuildQuery(CustomerReturnQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var predicate = PredicateBuilder.New<sm_Return_Order>(true);
            predicate.And(x => x.EntityTypeCode == OrderReturnConstants.EntityTypeCode.CUSTOMER);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.OrderCode.ToLower().Contains(query.FullTextSearch.ToLower())
                                    || s.OriginalDocumentCode.ToLower().Contains(query.FullTextSearch.ToLower())
                                    || s.EntityName.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.ReasonCode))
                predicate.And(s => s.ReasonCode == query.ReasonCode);
            
            if (!string.IsNullOrEmpty(query.OriginalDocumentCode))
                predicate.And(s => s.OriginalDocumentCode.Contains(query.OriginalDocumentCode));
            
            if (!string.IsNullOrEmpty(query.OrderCode))
                predicate.And(s => s.OrderCode.Contains(query.OrderCode));

            if (!string.IsNullOrEmpty(query.EntityTypeCode))
                predicate.And(s => s.EntityTypeCode == query.EntityTypeCode);

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

            if (!currentUser.ListRights.Contains("SALESORDERRETURN." + RightActionConstants.VIEWALL))
                predicate.And(s => s.CreatedByUserId == currentUser.UserId);
            
            return predicate;
        }

        /// <summary>
        /// Hoàn tiền đơn trả hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response<CustomerReturnViewModel>> RefundPaymentOrder(Guid id, RefundCreateModel model, RequestUser currentUser)
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
                    return Helper.CreateNotFoundResponse<CustomerReturnViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));
                
                #region Validated ChargePayment
                if (model.ListPayment != null && model.ListPayment.Count > 0)
                {
                    foreach (var item in model.ListPayment)
                    {
                        if (item.LinePaidAmount < 0)
                        {
                            return Helper.CreateBadRequestResponse<CustomerReturnViewModel>(string.Format("Tiền hoàn trả không được âm"));
                        }
                        
                        if (item.LinePaidAmount == 0 || item.LinePaidAmount == null)
                        {
                            return Helper.CreateBadRequestResponse<CustomerReturnViewModel>(string.Format("Vui lòng nhập số tiền thanh toán"));
                        }
                    
                        if (item.LinePaidAmount > entity.RemainingRefundAmount)
                        {
                            return Helper.CreateBadRequestResponse<CustomerReturnViewModel>(string.Format("Tiền hoàn trả không được lớn hơn tổng tiền cần phải hoàn trả"));
                        }
                    }
                }
                #endregion

                if (model.ListPayment.Count != 0)
                {
                    var customer = await _dbContext.sm_Customer.AsNoTracking()
                                        .Where(x => x.Id == entity.EntityId)
                                        .Select(x => new KhachHangViewModel { Id = x.Id, Name = x.Name, Code = x.Code })
                                        .FirstOrDefaultAsync();

                    foreach (var item in model.ListPayment)
                    {
                        await _cashbookTransactionHandler.Create(new CashbookTransactionCreateUpdateModel()
                        {
                            EntityCode = customer.Code,
                            EntityId = customer.Id,
                            OriginalDocumentId = entity.Id,
                            ConstructionId = entity.ConstructionId,
                            ContractId = entity.ContractId,
                            // sau khi khách hàng trả hàng sẽ nhập lại vào kho
                            OriginalDocumentType = OriginDocumentCashbookTransactionConstants.SALES_RETURN,
                            OriginalDocumentCode = entity.OrderCode,
                            ProjectId = entity.ProjectId,
                            // sau khi khách hàng trả hàng sẽ nhập lại vào kho và tạo phiếu chi
                            TransactionTypeCode = CashbookTransactionConstants.PaymentVoucherType,
                            PurposeCode = SaleOrderStatusConstants.AUTO_PAYMENT,
                            PaymentMethodCode = item.PaymentMethod,
                            Amount = item.LinePaidAmount ?? 0,
                            EntityName = customer?.Name,
                            ReceiptDate = DateTime.Now,
                            EntityTypeCode = EntityTypeCodeConstants.CUSTOMER,
                            EntityTypeName = EntityTypeNameConstants.CUSTOMER,
                            Description = $"Phiếu chi tự động tạo khi hoàn tiền cho khách hàng theo đơn trả hàng {entity.OrderCode}",
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

                return Helper.CreateSuccessResponse(_mapper.Map<CustomerReturnViewModel>(entity), "Hoàn trả tiền thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<CustomerReturnViewModel>(ex);
            }
        }

        /// <summary>
        /// Nhập kho khách trả hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<CustomerReturnViewModel>> CustomerReturnInventory(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Return_Order
                    .Include(x => x.OrderItems)
                    .Include(x => x.mk_DuAn)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<CustomerReturnViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

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
                                productInventoryEntity.SellableQuantity = initialQuantity + item.ReturnedQuantity;
                                await _dbContext.SaveChangesAsync();
                            }
                            
                            
                            if (productItem != null)
                            {
                                productItem.InitialStockQuantity = initialQuantity + item.ReturnedQuantity;
                                productItem.SellableQuantity = initialQuantity + item.ReturnedQuantity;
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        #endregion

                        #region Taọ bản ghi StockTransaction
                        foreach (var item in entity.OrderItems)
                        {
                            var productItem = await _dbContext.sm_Product
                                .Where(x => x.Id == item.ProductId)
                                .Select(x => new VatTuViewModel { Code = x.Code, Name = x.Name, InitialStockQuantity = x.InitialStockQuantity })
                                .FirstOrDefaultAsync();

                            var newProduct = new StockTransactionCreateModel
                            {
                                ReceiptInventoryQuantity = item.ReturnedQuantity,
                                SellableQuantity = item.ReturnedQuantity,
                                ExportInventoryQuantity = 0,
                                InitialStockQuantity = productItem.InitialStockQuantity,
                                ProductCode = productItem.Code,
                                ProductName = productItem.Name,
                                ProductId = item.ProductId,
                                WareCode = entity.WareCode,
                                OriginalDocumentType = OrderReturnConstants.OriginalDocumentType.SALES_RETURN,
                                OriginalDocumentCode = entity.OrderCode,
                                OriginalDocumentId = entity.Id,
                                UnitPrice = item.ReturnedUnitPrice,
                                Unit = item.Unit,
                                CreatedByUserId = entity.CreatedByUserId,
                                Action = OrderReturnConstants.ActionCode.CUSTOMER_RETURN,
                            };

                            await _stockTransactionHandler.Create(newProduct);
                        }
                        #endregion

                        #region Tạo phiếu nhập khi thực hiện nhận hàng hoàn trả
                        await _inventoryNoteHandler.CreateCustomerReturn(entity.Id, currentUser);
                        #endregion
                    }
                }

                var result = await GetById(id);
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<CustomerReturnViewModel>(result.Data, "Nhập kho khách trả hàng thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<CustomerReturnViewModel>(ex);
            }
        }


        /// <summary>
        /// Hủy đơn trả hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<CustomerReturnViewModel>> CanceledReturnOrder(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Return_Order.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<CustomerReturnViewModel>(string.Format("Phiếu không tồn tại trong hệ thống!"));

                entity.StatusCode = OrderReturnConstants.StatusCode.CANCELLED;
                entity.LastModifiedOnDate = DateTime.Now;

                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Sau khi hủy đơn trả hàng sẽ tăng lại về số lượng ban đầu
                    foreach (var item in entity.OrderItems)
                    {
                        await UpdateOrderAfterCancelled(entity.Id, item.ProductId, item.ReturnedQuantity);
                    }
                    #endregion
                }

                var result = await GetById(id);
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<CustomerReturnViewModel>(result.Data, "Hủy đơn trả hàng thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<CustomerReturnViewModel>(ex);
            }
        }
        
        /// <summary>
        /// Hàm xuất file danh sách phiếu trả hàng excel theo query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<string>> ExportListToExcel(CustomerReturnQueryModel query)
        {
            try
            {
                // Tạo predicate để lọc dựa trên các tham số trong query
                var predicate = BuildQuery(query);

                // Lấy danh sách phiếu kiểm kho từ cơ sở dữ liệu dựa trên lọc và phân trang
                var customerReturnOrders = await _dbContext.sm_Return_Order.AsNoTracking()
                    .Include(x => x.OrderItems)
                    .Where(predicate)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .GetPageAsync(query);

                // Kiểm tra nếu không có dữ liệu trả về trong trang hiện tại
                if (customerReturnOrders == null || customerReturnOrders.Content == null ||
                    customerReturnOrders.Content.Count == 0)
                    return Helper.CreateNotFoundResponse<string>("Không có phiếu trả hàng nào tồn tại trong hệ thống.");

                // Đặt tên file và đường dẫn template
                var fileName = $"danh sách phiếu trả hàng_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}.xlsx";
                var filePath = Path.Combine(_staticsFolder, fileName);
                var templatePath = Path.Combine(_staticsFolder, "excel-template/CustomerReturnTemplate.xlsx");

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

                    foreach (var order in customerReturnOrders.Content)
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
                        worksheet.Cells[startRow, 9].Value = order.EntityCode; // Mã khách hàng
                        worksheet.Cells[startRow, 10].Value = order.EntityName; // Tên khách hàng
                        worksheet.Cells[startRow, 11].Value = CodeTypeCollection.Instance
                            .FetchCode(order.ReasonCode, LanguageConstants.Default, order.TenantId)?.Title ?? null; // Lý do

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
