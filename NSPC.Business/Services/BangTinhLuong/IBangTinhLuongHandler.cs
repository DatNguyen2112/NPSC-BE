using NSPC.Business.Services.ChamCong;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.BangTinhLuong
{
    public interface IBangTinhLuongHandler
    {
        Task<Response<BangTinhLuongIdCreatedModel>> Create(BangTinhLuongCreateUpdateModel model);
        Task<Response<BangTinhLuongViewModel>> Update(Guid id, BangTinhLuongCreateUpdateModel model);
        Task<Response<BangTinhLuongViewModel>> GetById(Guid id);
        Task<Response<Pagination<BangTinhLuongViewModel>>> GetPage(BangTinhLuongQueryModel query);
        Task<Response<BangTinhLuongViewModel>> Delete(Guid id);
        Task<Response> ChangeActiveStatusAsync(Guid id, bool status, Guid byUserId);
    }
}
