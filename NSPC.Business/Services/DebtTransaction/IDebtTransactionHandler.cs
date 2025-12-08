using NSPC.Business.Services.Quotation;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.DebtTransaction
{
    public interface IDebtTransactionHandler
    {
        Task<Response<DebtTransactionViewModel>> Create(DebtTransactionCreateModel model, RequestUser byUser);
        Task<Response<Pagination<DebtTransactionViewModel>>> GetPage(DebtTransactionQueryModel query);
        Task<Response<DebtTransactionViewModel>> GetById(Guid id);
        Task<Response> CreateDebtCashbookTransaction(Guid id, RequestUser byUser);
        Task<Response> CreateDebtSaleOrder(Guid id, RequestUser byUser);
        Task<Response> CreateDebtPurchaseOrder(Guid id, RequestUser byUser);
        Task<Response> CreateDebtOrderReturn(Guid id, RequestUser byUser);
        Task<Response<Pagination<DebtTransactionReportViewModel>>> GetPageDebtReportCustomer(DebtTransactionQueryModel query);
    }
}
