using NSPC.Common;

namespace NSPC.Business.Services.QuanLyLaiXe
{
    public interface ILaiXeHandler
    {
        Task<Response<LaiXeViewModel>> Create(LaiXeCreateUpdateModel model);
        Task<Response<LaiXeViewModel>> Update(Guid id, LaiXeCreateUpdateModel model);
        Task<Response<LaiXeViewModel>> GetById(Guid id);
        Task<Response<Pagination<LaiXeViewModel>>> GetPage(LaiXeQueryModel query);
        Task<Response<LaiXeViewModel>> Delete(Guid id);
        Task<Response> DeleteMany(List<Guid> ids);
    }
}
