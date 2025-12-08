using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.EInvoice
{
    public interface IEInvoiceHandler
    {
        Task<Response<EInvoiceViewModel>> CreateAsync(EInvoiceCreateUpdateModel model, RequestUser currentUser);
        Task<Response<Pagination<EInvoiceViewModel>>> GetPageAsync(EInvoiceQueryModel query);
        Task<Response<EInvoiceViewModel>> GetByIdAsync(Guid id);
        Task<Response<EInvoiceViewModel>> UpdateAsync(Guid id, EInvoiceCreateUpdateModel model, RequestUser currentUser);
        Task<Response> DeleteAsync(Guid id);
        Task<Response> ActiveAsync(Guid id);
        Task<Response<EInvoiceViewModel>> PaymentHistoryAsync(Guid id, PaymentHistoryViewModel model, RequestUser currentUser);
    }
}
