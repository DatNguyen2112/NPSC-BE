using NSPC.Common;

namespace NSPC.Business
{
    public interface IRightHandler
    {
        Task<Response<RightViewModel>> Create(RightCreateUpdateModel model);
        Task<Response<RightViewModel>> Update(Guid id, RightCreateUpdateModel model);
        Task<Response> Delete(Guid id);
        Task<Response<List<RightViewModel>>> GetAll(); 
        Task<Response<Pagination<RightViewModel>>> GetPage(RightQueryModel query);
        Task<Response<RightViewModel>> GetById(Guid id);
        Task<Response> SeedRight(string groupCode, string groupName);
    }
}

