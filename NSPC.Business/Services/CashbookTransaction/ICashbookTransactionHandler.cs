using NSPC.Business.Services.InventoryNote;
using NSPC.Business.Services.Quotation;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.CashbookTransaction
{
    public interface ICashbookTransactionHandler
    {
        Task<Response<CashbookTransactionViewModel>> Create(CashbookTransactionCreateUpdateModel model, RequestUser byUser);
        Task<Response<CashbookTransactionViewModel>> CreateMultiple(CashbookTransactionCreateUpdateMutipleModel model, RequestUser currentUser);
        Task<Response<CashbookTransactionViewModel>> GetById(Guid id);
        Task<Response<Pagination<CashbookTransactionViewModel>>> GetPage(CashbookTransactionQueryModel query);
        Task<Response<CashbookTransactionViewModel>> Update(Guid id, CashbookTransactionCreateUpdateModel model, RequestUser byUser);
        Task<Response> DeactiveCashbookTransactionsAsync(List<Guid> ids);
        Task<Response<CashbookTransactionSummaryViewModel>> GetTransactionSummary(CashbookTransactionQueryModel query);
        Task<Response<string>> ExportExcelListCashbookTransaction(string type);
        Task<Response<string>> ExportExcelListCashbookTransactionCurrentPage(string type, CashbookTransactionQueryModel query);
        Task<Response<string>> ExportExcelListVouchersPage(CashbookTransactionQueryModel query); // Xuất file excel danh sách sổ quỹ
        Task<Response<CashFlowReportViewModel>> GetCashFlowReportAsync(CashbookTransactionQueryModel query);
        Task<Response<CashbookDashboardViewModel>> DashboardWithNoFilter(CashbookDashboardFilterDatetimeModel filter);
        Task<Response<CashbookDashboardViewModel>> DashboardWithTotalFilter(CashbookDashboardFilterDatetimeModel filter);
        Task<Response<CashbookTransactionViewModel>> PayInvoiceAsync(Guid id, RequestUser byUser);
        Task<Response<CashbookDashboardViewModel>> ReceiptsDashboardFilter(CashbookDashboardFilterModel filter);
        Task<Response<string>> ExportExcelCashFlowReportAsync(CashbookTransactionQueryModel query);
    }
}
