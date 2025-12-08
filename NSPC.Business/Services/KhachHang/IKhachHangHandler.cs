using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business
{
    public interface IKhachHangHandler
    {
        Task<Response<KhachHangViewModel>> Create(KhachHangCreateUpdateModel model, RequestUser byUser);
        Task<Response<KhachHangViewModel>> Update(Guid id, KhachHangCreateUpdateModel model, RequestUser byUser);
        Task<Response<KhachHangViewModel>> GetById(Guid id);
        Task<Response<Pagination<KhachHangViewModel>>> GetPage(KhachHangQueryModel query);
        Task<Response> Delete(Guid id);
        Task<Response<MultipleDeleteModel>> DeleteMultiple(List<Guid> ids);
        Task<Response> CheckIfCustomerHasQuotes(Guid id);
        Task<Response<CustomerSummaryView>> GetCustomerSummary(KhachHangQueryModel query);
        Task<Response<KhachHangViewModel>> ChangeCustomerType(Guid id, KhachHangCreateUpdateModel model, RequestUser byUser);
    }
}
