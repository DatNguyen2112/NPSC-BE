using NSPC.Business.Services.InventoryNote;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.Quotation
{
    public interface IQuotationHandler
    {
        Task<Response<QuotationViewModel>> Create(QuotationCreateUpdateModel model, RequestUser byUser);
        Task<Response<QuotationViewModel>> Update(Guid id, QuotationCreateUpdateModel model, RequestUser byUser);
        Task<Response<QuotationViewModel>> GetById(Guid id);
        Task<Response<Pagination<QuotationViewModel>>> GetPage(QuotationQueryModel query);
        Task<Response> Delete(Guid id);
        Task<Response<string>> ExportExcelListPage(QuotationQueryModel query);
    }
}
