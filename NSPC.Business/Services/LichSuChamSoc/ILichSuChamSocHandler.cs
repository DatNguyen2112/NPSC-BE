using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface ILichSuChamSocHandler
    {
        Task<Response<LichSuChamSocViewModel>> Create(LichSuChamSocCreateUpdateModel model);
        Task<Response<LichSuChamSocViewModel>> Update(Guid id, LichSuChamSocCreateUpdateModel model);
        Task<Response<LichSuChamSocViewModel>> ChangeActivity(Guid id, LichSuChamSocCreateUpdateModel model);
        Task<Response<LichSuChamSocViewModel>> GetById(Guid id);
        Task<Response<Pagination<LichSuChamSocViewModel>>> GetPage(LichSuChamSocQueryModel query);
        Task<Response<LichSuChamSocViewModel>> Delete(Guid id);
        Task<Response<LichSuChamSocViewModel>> ConfirmTaskCompletion(Guid id);
        Task<Response<LichSuChamSocViewModel>> RestoreTask(Guid id);
    }
}
