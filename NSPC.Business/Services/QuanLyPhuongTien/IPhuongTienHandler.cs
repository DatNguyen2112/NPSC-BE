using NSPC.Common;

namespace NSPC.Business.Services.QuanLyPhuongTien
{
    public interface IPhuongTienHandler
    {
        Task<Response<PhuongTienViewModel>> Create(PhuongTienCreateUpdateModel model);
        Task<Response<PhuongTienViewModel>> Update(Guid id, PhuongTienCreateUpdateModel model);
        Task<Response<PhuongTienViewModel>> GetById(Guid id);
        Task<Response<Pagination<PhuongTienViewModel>>> GetPage(PhuongTienQueryModel query);
        Task<Response<PhuongTienViewModel>> Delete(Guid id);
        Task<Response> DeleteMany(List<Guid> ids);
    }
}
