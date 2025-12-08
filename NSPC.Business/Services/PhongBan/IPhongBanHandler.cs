using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.PhongBan
{
    public interface IPhongBanHandler
    {
        Task<Response<PhongBanViewModel>> Create(PhongBanCreateUpdateModel model, RequestUser byUser);
        Task<Response<PhongBanViewModel>> Update(Guid id, PhongBanCreateUpdateModel model, RequestUser byUser);
        Task<Response<PhongBanViewModel>> GetById(Guid id);
        Task<Response<Pagination<PhongBanViewModel>>> GetPage(PhongBanQueryModel query);
        Task<Response<PhongBanViewModel>> Delete(Guid id);
    }
}
