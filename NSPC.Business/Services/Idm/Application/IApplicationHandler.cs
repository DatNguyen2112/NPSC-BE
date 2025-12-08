using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business
{
    public interface IApplicationHandler
    {
        Task<Response<ApplicationModel>> Create(ApplicationCreateUpdateModel model, RequestUser requestUser);
        Task<Response<ApplicationModel>> Update(Guid id, ApplicationCreateUpdateModel model, RequestUser requestUser);
        Task<Response> Delete(Guid id, RequestUser requestUser);
        Task<Response<Pagination<ApplicationModel>>> GetPageAsync(ApplicationQueryModel query, RequestUser requestUser);
        Task<Response<ApplicationModel>> GetById(Guid id, RequestUser requestUser);
    }
}
