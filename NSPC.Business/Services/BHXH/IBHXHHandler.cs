using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.BHXH
{
    public interface IBHXHHandler
    {
        Task<Response<BHXHViewModel>> Create(BHXHCreateUpdateModel model);
        Task<Response<BHXHViewModel>> Update(Guid id, BHXHCreateUpdateModel model);
        Task<Response<BHXHViewModel>> GetById(Guid id);
        Task<Response<Pagination<BHXHViewModel>>> GetPage(BHXHQueryModel query);
        Task<Response<BHXHViewModel>> Delete(Guid id);
    }
}
