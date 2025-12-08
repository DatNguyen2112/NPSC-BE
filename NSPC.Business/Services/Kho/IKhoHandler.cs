using NSPC.Common;

namespace NSPC.Business.Services.Business
{
    public interface IKhoHandler
    {
        Task<Response<KhoViewModel>> Create(KhoCreateUpdateModel model);
        Task<Response<KhoViewModel>> Update(Guid id, KhoCreateUpdateModel model);
        Task<Response<KhoViewModel>> GetById(Guid id);
        Task<Response<Pagination<KhoViewModel>>> GetPage(KhoQueryModel query);
        Task<Response<KhoViewModel>> Delete(Guid id);
        Task<Response> DeleteMany(List<Guid> ids);
    }
}
