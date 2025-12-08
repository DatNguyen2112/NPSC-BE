using NSPC.Common;

namespace NSPC.Business.Services.QuanLyLoaiXe
{
    public interface ILoaiXeHandler
    {
        Task<Response<LoaiXeViewModel>> Create(LoaiXeCreateUpdateModel model);
        Task<Response<LoaiXeViewModel>> Update(Guid id, LoaiXeCreateUpdateModel model);
        Task<Response<LoaiXeViewModel>> GetById(Guid id);
        Task<Response<Pagination<LoaiXeViewModel>>> GetPage(LoaiXeQueryModel query);
        Task<Response<LoaiXeViewModel>> Delete(Guid id);
        Task<Response> DeleteMany(List<Guid> ids);
    }
}
