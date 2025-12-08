using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NSPC.Business.Services.StockTransaction;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.DebtTransaction;
using NSPC.Data.Data.Entity.NhaCungCap;
using NSPC.Data.Data.Entity.StockTransaction;
using Serilog;
using SharpCompress.Common;
using System.Linq.Expressions;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.DebtTransaction
{
    public class DebtTransactionHandler : IDebtTransactionHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public DebtTransactionHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<DebtTransactionViewModel>> Create(DebtTransactionCreateModel model, RequestUser currentUser)
        {
            try
            {
                var debtTransaction = new sm_DebtTransaction
                {
                    Id = Guid.NewGuid(),
                    EntityId = model.EntityId,
                    EntityCode = model.EntityCode,
                    EntityType = model.EntityType,
                    EntityName = model.EntityName,
                    OriginalDocumentId = model.OriginalDocumentId,
                    OriginalDocumentCode = model.OriginalDocumentCode,
                    OriginalDocumentType = model.OriginalDocumentType,
                    ChangeAmount = model.ChangeAmount,
                    DebtAmount = model.DebtAmount,
                    Action = model.Action,
                    Note = model.Note,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now
                };

                _dbContext.sm_DebtTransaction.Add(debtTransaction);

                await _dbContext.SaveChangesAsync();

                var result = await GetById(debtTransaction.Id);

                if (result.IsSuccess)
                    result.Message = "Thêm mới thành công";
                return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<DebtTransactionViewModel>(ex);
            }
        }
        public async Task<Response<Pagination<DebtTransactionViewModel>>> GetPage(DebtTransactionQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_DebtTransaction.AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<DebtTransactionViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<DebtTransactionViewModel>>(ex);
            }
        }
        public async Task<Response<DebtTransactionViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_DebtTransaction.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<DebtTransactionViewModel>("Công nợ không tồn tại trong hệ thống.");

                var result = _mapper.Map<DebtTransactionViewModel>(entity);

                return new Response<DebtTransactionViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<DebtTransactionViewModel>(ex);
            }
        }

        /// <summary>
        /// Filter data
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private Expression<Func<sm_DebtTransaction, bool>> BuildQuery(DebtTransactionQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_DebtTransaction>(true);
            if (query.EntityId.HasValue)
            {
                predicate = predicate.And(x => x.EntityId == query.EntityId);
            }

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date.AddDays(1).AddTicks(-1));
            }

            // Công nợ dương hoặc âm: Dương: 0, Âm: 1
            if (query.DebtAmountType.HasValue)
            {
                if (query.DebtAmountType == 0)
                {
                    predicate = predicate.And(x => x.ChangeAmount >= 0);
                }
                else
                {
                    predicate = predicate.And(x => x.ChangeAmount < 0);
                }
            }

            return predicate;
        }

        /// <summary>
        /// Hàm tạo công nợ từ phiếu thu/chi (CashbookTransaction)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response> CreateDebtCashbookTransaction(Guid id, RequestUser currentUser)
        {
            try
            {
                // lấy thông tin phiếu thu/chi từ bảng sm_Cashbook_Transaction
                var cashbook = await _dbContext.sm_Cashbook_Transaction.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (cashbook == null)
                    return Helper.CreateNotFoundResponse<DebtTransactionViewModel>("Phiếu thu/chi không tồn tại trong hệ thống.");

                // lấy thông tin giao dịch nợ mới nhất theo id từ bảng sm_DebtTransaction
                var latestDebtTransaction = await _dbContext.sm_DebtTransaction.AsNoTracking()
                    .Where(x => x.EntityId == cashbook.EntityId && x.EntityType == cashbook.EntityTypeCode)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .FirstOrDefaultAsync();

                // Nếu không tìm thấy thông tin giao dịch nợ mới nhất thì tạo mới
                if (latestDebtTransaction == null)
                {
                    // Thiết lập ChangeAmount theo logic cụ thể
                    decimal changeAmount;

                    if (cashbook.TransactionTypeCode == CashbookTransactionConstants.ReceiptVoucherType) // Phiếu thu
                    {
                        changeAmount = cashbook.EntityTypeCode == CashbookTransactionEntityTypeCodeConstants.CUSTOMER
                            ? -Math.Abs(cashbook.Amount) // Phiếu thu khách hàng: số âm
                            : Math.Abs(cashbook.Amount);     // Phiếu thu nhà cung cấp: số dương
                    }
                    else if (cashbook.TransactionTypeCode == CashbookTransactionConstants.PaymentVoucherType) // Phiếu chi
                    {
                        changeAmount = cashbook.EntityTypeCode == CashbookTransactionEntityTypeCodeConstants.CUSTOMER
                            ? Math.Abs(cashbook.Amount)       // Phiếu chi khách hàng: số dương
                            : -Math.Abs(cashbook.Amount); // Phiếu chi nhà cung cấp: số âm
                    }
                    else
                    {
                        throw new Exception("Loại phiếu không hợp lệ.");
                    }

                    var debtTransaction = new DebtTransactionCreateModel
                    {
                        EntityId = cashbook.EntityId,
                        EntityCode = cashbook.EntityCode,
                        EntityType = cashbook.EntityTypeCode == CashbookTransactionEntityTypeCodeConstants.CUSTOMER
                            ? DebtTransactionEntityTypesConstants.CUSTOMER
                            : DebtTransactionEntityTypesConstants.SUPPLIER,
                        EntityName = cashbook.EntityName,
                        OriginalDocumentId = cashbook.Id,
                        OriginalDocumentCode = cashbook.Code,

                        // Loại tài liệu ban đầu dựa vào loại giao dịch (phiếu thu hoặc phiếu chi)
                        OriginalDocumentType = cashbook.TransactionTypeCode == CashbookTransactionConstants.ReceiptVoucherType
                            ? DebtTransactionOriginalDocumentTypesConstants.RECEIPT_VOUCHER
                            : DebtTransactionOriginalDocumentTypesConstants.PAYMENT_VOUCHER,

                        // Số tiền thay đổi (ChangeAmount) đã tính toán ở trên
                        ChangeAmount = changeAmount,

                        // DebtAmount = ChangeAmount
                        DebtAmount = changeAmount,

                        // Hành động dựa vào loại giao dịch (phiếu thu hoặc phiếu chi)
                        Action = cashbook.TransactionTypeCode == CashbookTransactionConstants.ReceiptVoucherType
                            ? DebtTransactionActionsCodesConstants.RECEIPT_VOUCHER_CREATE
                            : DebtTransactionActionsCodesConstants.PAYMENT_VOUCHER_CREATE,

                        // Ghi chú dựa vào loại giao dịch (phiếu thu hoặc phiếu chi)
                        Note = cashbook.TransactionTypeCode == CashbookTransactionConstants.ReceiptVoucherType
                            ? DebtTransactionNotesConstants.RECEIPT_VOUCHER_CREATE
                            : DebtTransactionNotesConstants.PAYMENT_VOUCHER_CREATE,
                    };

                    // Gọi hàm tạo bản ghi giao dịch nợ
                    await Create(debtTransaction, currentUser);
                }
                else // Nếu đã có thông tin giao dịch nợ mới nhất thì tính toán DebtAmount mới
                {
                    // Thiết lập ChangeAmount theo logic cụ thể
                    decimal changeAmount;

                    if (cashbook.TransactionTypeCode == CashbookTransactionConstants.ReceiptVoucherType) // Phiếu thu
                    {
                        changeAmount = cashbook.EntityTypeCode == CashbookTransactionEntityTypeCodeConstants.CUSTOMER
                            ? -Math.Abs(cashbook.Amount) // Phiếu thu khách hàng: số âm
                            : Math.Abs(cashbook.Amount);     // Phiếu thu nhà cung cấp: số dương
                    }
                    else if (cashbook.TransactionTypeCode == CashbookTransactionConstants.PaymentVoucherType) // Phiếu chi
                    {
                        changeAmount = cashbook.EntityTypeCode == CashbookTransactionEntityTypeCodeConstants.CUSTOMER
                            ? Math.Abs(cashbook.Amount)       // Phiếu chi khách hàng: số dương
                            : -Math.Abs(cashbook.Amount); // Phiếu chi nhà cung cấp: số âm
                    }
                    else
                    {
                        throw new Exception("Loại phiếu không hợp lệ.");
                    }

                    var debtTransaction = new DebtTransactionCreateModel
                    {
                        EntityId = cashbook.EntityId,
                        EntityCode = cashbook.EntityCode,
                        EntityType = cashbook.EntityTypeCode == CashbookTransactionEntityTypeCodeConstants.CUSTOMER
                            ? DebtTransactionEntityTypesConstants.CUSTOMER
                            : DebtTransactionEntityTypesConstants.SUPPLIER,
                        EntityName = cashbook.EntityName,
                        OriginalDocumentId = cashbook.Id,
                        OriginalDocumentCode = cashbook.Code,

                        // Loại tài liệu ban đầu dựa vào loại giao dịch (phiếu thu hoặc phiếu chi)
                        OriginalDocumentType = cashbook.TransactionTypeCode == CashbookTransactionConstants.ReceiptVoucherType
                            ? DebtTransactionOriginalDocumentTypesConstants.RECEIPT_VOUCHER
                            : DebtTransactionOriginalDocumentTypesConstants.PAYMENT_VOUCHER,

                        // Số tiền thay đổi (ChangeAmount) đã tính toán ở trên
                        ChangeAmount = changeAmount,

                        // DebtAmount: Số tiền nợ cũ + ChangeAmount
                        DebtAmount = latestDebtTransaction.DebtAmount + changeAmount,

                        // Hành động dựa vào loại giao dịch (phiếu thu hoặc phiếu chi)
                        Action = cashbook.TransactionTypeCode == CashbookTransactionConstants.ReceiptVoucherType
                            ? DebtTransactionActionsCodesConstants.RECEIPT_VOUCHER_CREATE
                            : DebtTransactionActionsCodesConstants.PAYMENT_VOUCHER_CREATE,

                        // Ghi chú dựa vào loại giao dịch (phiếu thu hoặc phiếu chi)
                        Note = cashbook.TransactionTypeCode == CashbookTransactionConstants.ReceiptVoucherType
                            ? DebtTransactionNotesConstants.RECEIPT_VOUCHER_CREATE
                            : DebtTransactionNotesConstants.PAYMENT_VOUCHER_CREATE,
                    };

                    // Gọi hàm tạo bản ghi giao dịch nợ
                    await Create(debtTransaction, currentUser);
                }

                // Cập nhật DebtAmount của Khách hàng hoặc Nhà cung cấp mỗi bước insert vào sm_DebtTransaction = DebtAmount mới nhất trong sm_DebtTransaction
                var debtAmount = await _dbContext.sm_DebtTransaction.Where(dt => dt.EntityId == cashbook.EntityId && dt.EntityType == cashbook.EntityTypeCode)
                    .OrderByDescending(dt => dt.CreatedOnDate).Select(dt => dt.DebtAmount).FirstOrDefaultAsync();

                if (cashbook.EntityTypeCode == CashbookTransactionEntityTypeCodeConstants.CUSTOMER)
                {
                    var customer = await _dbContext.sm_Customer.FirstOrDefaultAsync(x => x.Id == cashbook.EntityId);
                    if (customer != null)
                    {
                        customer.DebtAmount = debtAmount;
                        await _dbContext.SaveChangesAsync();
                    }
                }
                else if (cashbook.EntityTypeCode == CashbookTransactionEntityTypeCodeConstants.SUPPLIER)
                {
                    var supplier = await _dbContext.sm_Supplier.FirstOrDefaultAsync(x => x.Id == cashbook.EntityId);
                    if (supplier != null)
                    {
                        supplier.TotalDebtAmount = debtAmount;
                        await _dbContext.SaveChangesAsync();
                    }
                }
                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: id: {@id}", id);
                return Helper.CreateExceptionResponse<DebtTransactionViewModel>(ex);
            }
        }

        /// <summary>
        /// Hàm tạo công nợ khi xuất kho từ đơn bán hàng (SalesOrder)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response> CreateDebtSaleOrder(Guid id, RequestUser currentUser)
        {
            try
            {
                // lấy thông tin đơn bán hàng từ bảng sm_SalesOrder
                var saleOrder = await _dbContext.sm_SalesOrder.Include(x => x.sm_Customer).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (saleOrder == null)
                    return Helper.CreateNotFoundResponse<DebtTransactionViewModel>("Đơn bán hàng không tồn tại trong hệ thống.");

                if (saleOrder.ExportStatusCode == OriginalDocumentTypeConstants.XUAT_KHO)
                {
                    // lấy thông tin giao dịch nợ mới nhất theo id từ bảng sm_DebtTransaction
                    var latestDebtTransaction = await _dbContext.sm_DebtTransaction.AsNoTracking()
                        .Where(x => x.EntityId == saleOrder.CustomerId && x.EntityType == DebtTransactionEntityTypesConstants.CUSTOMER)
                        .OrderByDescending(x => x.CreatedOnDate)
                        .FirstOrDefaultAsync();

                    if (latestDebtTransaction == null)
                    {
                        return Helper.CreateNotFoundResponse<DebtTransactionViewModel>("Không tìm thấy thông tin công nợ của khách hàng");
                    }

                    // Tạo bản ghi giao dịch nợ tương ứng với đơn hàng
                    var debtTransaction = new DebtTransactionCreateModel
                    {
                        EntityId = saleOrder.sm_Customer.Id,
                        EntityCode = saleOrder.sm_Customer.Code,
                        EntityType = DebtTransactionEntityTypesConstants.CUSTOMER,
                        EntityName = saleOrder.sm_Customer.Name,
                        OriginalDocumentId = saleOrder.Id,
                        OriginalDocumentCode = saleOrder.OrderCode,

                        // Loại tài liệu ban đầu dựa vào loại giao dịch Đơn bán hàng
                        OriginalDocumentType = DebtTransactionOriginalDocumentTypesConstants.SALES_ORDER,

                        // Số tiền thay đổi (ChangeAmount) đã tính toán ở trên
                        ChangeAmount = Math.Abs(saleOrder.Total),

                        // DebtAmount: Số tiền nợ cũ + ChangeAmount
                        DebtAmount = latestDebtTransaction.DebtAmount + Math.Abs(saleOrder.Total),

                        // Hành động dựa vào loại giao dịch Đơn bán hàng
                        Action = DebtTransactionActionsCodesConstants.INVENTORY_EXPORT,

                        // Ghi chú dựa vào loại giao dịch đơn bán hàng
                        Note = DebtTransactionNotesConstants.INVENTORY_EXPORT
                    };
                    // Gọi hàm tạo bản ghi giao dịch nợ
                    await Create(debtTransaction, currentUser);

                    // Cập nhật DebtAmount của Khách hàng mỗi bước insert vào sm_DebtTransaction = DebtAmount mới nhất trong sm_DebtTransaction
                    var debtAmount = await _dbContext.sm_DebtTransaction.Where(dt => dt.EntityId == saleOrder.CustomerId && dt.EntityType == CashbookTransactionEntityTypeCodeConstants.CUSTOMER)
                        .OrderByDescending(dt => dt.CreatedOnDate).Select(dt => dt.DebtAmount).FirstOrDefaultAsync();

                    var customer = await _dbContext.sm_Customer.FirstOrDefaultAsync(x => x.Id == saleOrder.CustomerId);
                    if (customer != null)
                    {
                        customer.DebtAmount = debtAmount;
                        await _dbContext.SaveChangesAsync();
                    }
                }
                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: id: {@id}", id);
                return Helper.CreateExceptionResponse<DebtTransactionViewModel>(ex);
            }
        }

        /// <summary>
        /// Hàm tạo công nợ khi nhập hàng từ đơn nhập hàng (PurchaseOrder)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response> CreateDebtPurchaseOrder(Guid id, RequestUser currentUser)
        {
            try
            {
                // lấy thông tin đơn nhập hàng từ bảng sm_PurchaseOrder
                var purchaseOrder = await _dbContext.sm_PurchaseOrder
                    .Include(x => x.Items)
                    .Include(x => x.sm_Supplier)
                    .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (purchaseOrder == null)
                    return Helper.CreateNotFoundResponse<DebtTransactionViewModel>("Đơn nhập hàng không tồn tại trong hệ thống.");

                if (purchaseOrder.ImportStatusCode == PurchaseOrderConstants.NHAP_HANG)
                {
                    // lấy thông tin giao dịch nợ mới nhất theo id từ bảng sm_DebtTransaction
                    var latestDebtTransaction = await _dbContext.sm_DebtTransaction.AsNoTracking()
                        .Where(x => x.EntityId == purchaseOrder.SupplierId && x.EntityType == DebtTransactionEntityTypesConstants.SUPPLIER)
                        .OrderByDescending(x => x.CreatedOnDate)
                        .FirstOrDefaultAsync();

                    if (latestDebtTransaction == null)
                    {
                        return Helper.CreateNotFoundResponse<DebtTransactionViewModel>("Không tìm thấy thông tin công nợ của nhà cung cấp.");
                    }

                    // Tạo bản ghi giao dịch nợ tương ứng với đơn nhập hàng
                    var debtTransaction = new DebtTransactionCreateModel
                    {
                        EntityId = purchaseOrder.sm_Supplier.Id,
                        EntityCode = purchaseOrder.sm_Supplier.Code,
                        EntityType = DebtTransactionEntityTypesConstants.SUPPLIER,
                        EntityName = purchaseOrder.sm_Supplier.Name,
                        OriginalDocumentId = purchaseOrder.Id,
                        OriginalDocumentCode = purchaseOrder.OrderCode,

                        // Loại tài liệu ban đầu dựa vào loại giao dịch Đơn nhập hàng
                        OriginalDocumentType = DebtTransactionOriginalDocumentTypesConstants.PURCHASE_ORDER,

                        // Số tiền thay đổi (ChangeAmount) đã tính toán ở trên
                        ChangeAmount = Math.Abs(purchaseOrder.Total ?? 0),

                        // DebtAmount: Số tiền nợ cũ + ChangeAmount
                        DebtAmount = latestDebtTransaction.DebtAmount + Math.Abs(purchaseOrder.Total ?? 0),

                        // Hành động dựa vào loại giao dịch Đơn nhập hàng
                        Action = DebtTransactionActionsCodesConstants.INVENTORY_IMPORT,

                        // Ghi chú dựa vào loại giao dịch đơn nhâp hàng
                        Note = DebtTransactionNotesConstants.INVENTORY_IMPORT
                    };
                    // Gọi hàm tạo bản ghi giao dịch nợ
                    await Create(debtTransaction, currentUser);

                    // Cập nhật DebtAmount của Nhà cung cấp mỗi bước insert vào sm_DebtTransaction = DebtAmount mới nhất trong sm_DebtTransaction
                    var debtAmount = await _dbContext.sm_DebtTransaction
                        .Where(dt => dt.EntityId == purchaseOrder.SupplierId && dt.EntityType == CashbookTransactionEntityTypeCodeConstants.SUPPLIER)
                        .OrderByDescending(dt => dt.CreatedOnDate).Select(dt => dt.DebtAmount).FirstOrDefaultAsync();

                    var supplier = await _dbContext.sm_Supplier.FirstOrDefaultAsync(x => x.Id == purchaseOrder.SupplierId);
                    if (supplier != null)
                    {
                        supplier.TotalDebtAmount = debtAmount;
                        await _dbContext.SaveChangesAsync();
                    }
                }
                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: id: {@id}", id);
                return Helper.CreateExceptionResponse<DebtTransactionViewModel>(ex);
            }
        }

        /// <summary>
        /// Hàm tạo công nợ khi khách hàng trả hàng / trả hàng nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response> CreateDebtOrderReturn(Guid id, RequestUser currentUser)
        {
            try
            {
                // lấy thông tin đơn nhập hàng từ bảng sm_Return_Order
                var returnOrder = await _dbContext.sm_Return_Order.Include(x => x.OrderItems).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

                if (returnOrder == null)
                {
                    if (returnOrder.EntityTypeCode == OrderReturnConstants.EntityTypeCode.SUPPLIER)
                    {
                        return Helper.CreateNotFoundResponse<DebtTransactionViewModel>($"Đơn trả hàng {returnOrder.EntityTypeName} không tồn tại trong hệ thống.");
                    }

                    if (returnOrder.EntityTypeCode == OrderReturnConstants.EntityTypeCode.CUSTOMER)
                    {
                        return Helper.CreateNotFoundResponse<DebtTransactionViewModel>($"Đơn {returnOrder.EntityTypeName} trả hàng không tồn tại trong hệ thống.");
                    }
                }

                if (returnOrder.StatusCode == OrderReturnConstants.StatusCode.RETURNED)
                {
                    sm_DebtTransaction latestDebtTransaction = null;
                    decimal debtAmount = 0;
                    sm_Supplier supplier = null;
                    sm_Customer customer = null;

                    // lấy thông tin giao dịch nợ mới nhất theo id từ bảng sm_DebtTransaction
                    if (returnOrder.EntityTypeCode == OrderReturnConstants.EntityTypeCode.CUSTOMER)
                    {
                        latestDebtTransaction = await _dbContext.sm_DebtTransaction.AsNoTracking()
                        .Where(x => x.EntityId == returnOrder.EntityId && x.EntityType == DebtTransactionEntityTypesConstants.CUSTOMER)
                        .OrderByDescending(x => x.CreatedOnDate)
                        .FirstOrDefaultAsync();
                    }

                    if (returnOrder.EntityTypeCode == OrderReturnConstants.EntityTypeCode.SUPPLIER)
                    {
                        latestDebtTransaction = await _dbContext.sm_DebtTransaction.AsNoTracking()
                        .Where(x => x.EntityId == returnOrder.EntityId && x.EntityType == DebtTransactionEntityTypesConstants.SUPPLIER)
                        .OrderByDescending(x => x.CreatedOnDate)
                        .FirstOrDefaultAsync();
                    }

                    if (latestDebtTransaction == null)
                    {
                        return Helper.CreateNotFoundResponse<DebtTransactionViewModel>("Không tìm thấy thông tin công nợ của khách hàng");
                    }

                    // Tạo bản ghi giao dịch nợ tương ứng với đơn trả hàng
                    var debtTransaction = new DebtTransactionCreateModel
                    {
                        EntityId = returnOrder.EntityId,
                        EntityCode = returnOrder.EntityCode,
                        EntityType = returnOrder.EntityTypeCode == OrderReturnConstants.EntityTypeCode.CUSTOMER ? DebtTransactionEntityTypesConstants.CUSTOMER : DebtTransactionEntityTypesConstants.SUPPLIER,
                        EntityName = returnOrder.EntityName,
                        OriginalDocumentId = returnOrder.Id,
                        OriginalDocumentCode = returnOrder.OrderCode,

                        // Loại tài liệu ban đầu dựa vào loại giao dịch Đơn trả hàng
                        OriginalDocumentType = returnOrder.EntityTypeCode == OrderReturnConstants.EntityTypeCode.CUSTOMER ? DebtTransactionOriginalDocumentTypesConstants.CUSTOMER_RETURN : DebtTransactionOriginalDocumentTypesConstants.SUPPLIER_RETURN,

                        // Số tiền thay đổi (ChangeAmount) đã tính toán ở trên
                        ChangeAmount = -Math.Abs(returnOrder.RefundSubTotal),

                        // DebtAmount: Số tiền nợ cũ - ChangeAmount
                        DebtAmount = latestDebtTransaction.DebtAmount - Math.Abs(returnOrder.RefundSubTotal),

                        // Hành động dựa vào loại giao dịch Đơn trả hàng
                        Action = returnOrder.EntityTypeCode == OrderReturnConstants.EntityTypeCode.CUSTOMER ? DebtTransactionActionsCodesConstants.RETURN_INVENTORY_EXPORT : DebtTransactionActionsCodesConstants.RETURN_INVENTORY_IMPORT,

                        // Ghi chú dựa vào loại giao dịch đơn trả hàng
                        Note = returnOrder.EntityTypeCode == OrderReturnConstants.EntityTypeCode.CUSTOMER ? DebtTransactionNotesConstants.RETURN_INVENTORY_EXPORT : DebtTransactionNotesConstants.RETURN_INVENTORY_IMPORT
                    };

                    // Gọi hàm tạo bản ghi giao dịch nợ
                    await Create(debtTransaction, currentUser);

                    // lấy thông tin giao dịch nợ mới nhất theo id từ bảng sm_DebtTransaction
                    if (returnOrder.EntityTypeCode == OrderReturnConstants.EntityTypeCode.CUSTOMER)
                    {
                        debtAmount = await _dbContext.sm_DebtTransaction
                        .Where(dt => dt.EntityId == returnOrder.EntityId && dt.EntityType == CashbookTransactionEntityTypeCodeConstants.CUSTOMER)
                        .OrderByDescending(dt => dt.CreatedOnDate).Select(dt => dt.DebtAmount).FirstOrDefaultAsync();

                        customer = await _dbContext.sm_Customer.FirstOrDefaultAsync(x => x.Id == returnOrder.EntityId);
                    }

                    if (returnOrder.EntityTypeCode == OrderReturnConstants.EntityTypeCode.SUPPLIER)
                    {
                        debtAmount = await _dbContext.sm_DebtTransaction
                        .Where(dt => dt.EntityId == returnOrder.EntityId && dt.EntityType == CashbookTransactionEntityTypeCodeConstants.SUPPLIER)
                        .OrderByDescending(dt => dt.CreatedOnDate).Select(dt => dt.DebtAmount).FirstOrDefaultAsync();

                        supplier = await _dbContext.sm_Supplier.FirstOrDefaultAsync(x => x.Id == returnOrder.EntityId);

                    }

                    if (supplier != null)
                    {
                        supplier.TotalDebtAmount = debtAmount;
                        await _dbContext.SaveChangesAsync();
                    }

                    if (customer != null)
                    {
                        customer.DebtAmount = debtAmount;
                        await _dbContext.SaveChangesAsync();
                    }

                }

                return Helper.CreateSuccessResponse();

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: id: {@id}", id);
                return Helper.CreateExceptionResponse<DebtTransactionViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<DebtTransactionReportViewModel>>> GetPageDebtReportCustomer(DebtTransactionQueryModel query)
        {
            try
            {
                var startDate = query.DateRange[0].Value.Date;
                var endDate = query.DateRange[1].Value.Date.AddDays(1).AddTicks(-1).AddMilliseconds(-1);
                query.DateRange[1] = endDate;
                var predicate = BuildQueryDebtReport(query);
                var iquery = _dbContext.sm_DebtTransaction.AsNoTracking().Where(predicate);

                var queryResult = iquery.GroupBy(x => new { x.EntityId, x.EntityCode, x.EntityName, x.EntityType })
                    .Select(x => new DebtTransactionReportViewModel
                    {
                        Id = x.Key.EntityId,
                        EntityId = x.Key.EntityId,
                        EntityCode = x.Key.EntityCode,
                        EntityType = x.Key.EntityType,
                        EntityName = x.Key.EntityName,
                        OpeningDebt = _dbContext.sm_DebtTransaction.AsNoTracking().Where(y => y.CreatedOnDate < startDate && y.EntityId == x.Key.EntityId).OrderByDescending(y => y.CreatedOnDate).FirstOrDefault() != null
                        ? _dbContext.sm_DebtTransaction.AsNoTracking().Where(y => y.CreatedOnDate < startDate && y.EntityId == x.Key.EntityId).OrderByDescending(y => y.CreatedOnDate).FirstOrDefault().DebtAmount
                        : 0,

                        DebtIncrease = _dbContext.sm_DebtTransaction.AsNoTracking().Where(y => y.CreatedOnDate.Date >= startDate && y.CreatedOnDate.Date <= endDate && y.ChangeAmount > 0 && y.EntityId == x.Key.EntityId).Sum(y => y.ChangeAmount),

                        DebtDecrease = _dbContext.sm_DebtTransaction.AsNoTracking().Where(y => y.CreatedOnDate.Date >= startDate && y.CreatedOnDate.Date <= endDate && y.ChangeAmount < 0 && y.EntityId == x.Key.EntityId).Sum(y => y.ChangeAmount) * -1,

                        DebtRemain = (_dbContext.sm_DebtTransaction.AsNoTracking().Where(y => y.CreatedOnDate.Date >= startDate && y.CreatedOnDate.Date <= endDate && y.ChangeAmount > 0 && y.EntityId == x.Key.EntityId).Sum(y => y.ChangeAmount))
                        - (_dbContext.sm_DebtTransaction.AsNoTracking().Where(y => y.CreatedOnDate.Date >= startDate && y.CreatedOnDate.Date <= endDate && y.ChangeAmount < 0 && y.EntityId == x.Key.EntityId).Sum(y => y.ChangeAmount) * -1),

                        ClosingDebt = _dbContext.sm_DebtTransaction.AsNoTracking().Where(y => y.CreatedOnDate <= endDate && y.EntityId == x.Key.EntityId).OrderByDescending(y => y.CreatedOnDate).FirstOrDefault() != null
                        ? _dbContext.sm_DebtTransaction.AsNoTracking().Where(y => y.CreatedOnDate <= endDate && y.EntityId == x.Key.EntityId).OrderByDescending(y => y.CreatedOnDate).FirstOrDefault().DebtAmount
                        : 0
                    });

                // Áp dụng bộ lọc MinClosingDebt, MaxClosingDebt, và Positive trên queryResult
                if (query.MinClosingDebt.HasValue)
                {
                    queryResult = queryResult.Where(x => x.ClosingDebt >= query.MinClosingDebt.Value);
                }

                if (query.MaxClosingDebt.HasValue)
                {
                    queryResult = queryResult.Where(x => x.ClosingDebt <= query.MaxClosingDebt.Value);
                }

                // Query Nợ cuối kỳ khác 0 (Positive)
                if (query.Positive.HasValue && query.Positive.Value)
                {
                    queryResult = queryResult.Where(x => x.ClosingDebt != 0);
                }

                // Phân trang và lấy kết quả
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<DebtTransactionReportViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<DebtTransactionReportViewModel>>(ex);
            }
        }

        private Expression<Func<sm_DebtTransaction, bool>> BuildQueryDebtReport(DebtTransactionQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_DebtTransaction>(true);

            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.EntityCode.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.EntityName.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.EntityType != null && query.EntityType.Count() > 0)
                predicate.And(s => query.EntityType.Contains(s.EntityType));

            // Lọc theo DateRange
            if (query.DateRange != null && query.DateRange.Any())
            {
                var startDate = query.DateRange[0]?.Date;
                var endDate = query.DateRange[1]?.Date.AddDays(1).AddTicks(-1).AddMilliseconds(-1);

                if (startDate.HasValue && endDate.HasValue)
                {
                    // Điều kiện lọc ngoài DateRange
                    predicate.And(x => x.CreatedOnDate >= startDate || x.CreatedOnDate <= endDate);
                }
            }

            return predicate;
        }
    }
}
