using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface ISupplierReturnHandler
    {
        Task<Response<SupplierReturnViewModel>> Create(SupplierReturnCreateUpdateModel model, RequestUser byUser);
        Task<Response<SupplierReturnViewModel>> CancelledReturnedOrder(Guid id);
        Task<Response<SupplierReturnViewModel>> SupplierReturnOrder(Guid id, RequestUser byUser);
        Task<Response<SupplierReturnViewModel>> RefundPaymentOrder(Guid id, RefundSupplierCreateModel model, RequestUser byUser);
        Task<Response<SupplierReturnViewModel>> GetById(Guid id);
        Task<Response<Pagination<SupplierReturnViewModel>>> GetPage(SupplierReturnQueryModel query, RequestUser byUser);
        Task<Response<SupplierReturnViewModel>> Delete(Guid id);
        Task<Response<string>> ExportListToExcel(SupplierReturnQueryModel query);
    }
}
