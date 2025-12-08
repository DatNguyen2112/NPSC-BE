using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.ChamCong
{
    public interface IChamCongHandler
    {
        Task<Response<ChamCongIdCreatedModel>> Create(ChamCongCreateUpdateModel model);
        Task<Response<ChamCongViewModel>> Update(Guid id, ChamCongCreateUpdateModel model);
        Task<Response<ChamCongViewModel>> GetById(Guid id);
        Task<Response<Pagination<ChamCongViewModel>>> GetPage(ChamCongQueryModel query);
        Task<Response<ChamCongViewModel>> Delete(Guid id);
        Task<Response> ChangeActiveStatusAsync(Guid id, bool status, Guid byUserId);
        Task<Response<List<ChamCongItemViewModel>>> Import(string path);
    }
}
