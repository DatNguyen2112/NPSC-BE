using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface ICustomerReturnHandler
    {
        Task<Response<CustomerReturnViewModel>> Create(CustomerReturnCreateUpdateModel model, RequestUser byUser);
        Task<Response<CustomerReturnViewModel>> CanceledReturnOrder(Guid id);
        Task<Response<CustomerReturnViewModel>> CustomerReturnInventory(Guid id, RequestUser byUser);
        Task<Response<CustomerReturnViewModel>> RefundPaymentOrder(Guid id, RefundCreateModel model, RequestUser byUser);
        Task<Response<CustomerReturnViewModel>> GetById(Guid id);
        Task<Response<Pagination<CustomerReturnViewModel>>> GetPage(CustomerReturnQueryModel query, RequestUser byUser);
        Task<Response<CustomerReturnViewModel>> Delete(Guid id);
        Task<Response<string>> ExportListToExcel(CustomerReturnQueryModel query);
    }
}
