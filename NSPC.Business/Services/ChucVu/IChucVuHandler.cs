using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.ChucVu
{
    public interface IChucVuHandler
    {
        Task<Response<ChucVuViewModel>> Create(ChucVuCreateUpdateModel model, RequestUser byUser);
        Task<Response<ChucVuViewModel>> Update(Guid id, ChucVuCreateUpdateModel model, RequestUser byUser);
        Task<Response<ChucVuViewModel>> GetById(Guid id);
        Task<Response<Pagination<ChucVuViewModel>>> GetPage(ChucVuQueryModel query);
        Task<Response<ChucVuViewModel>> Delete(Guid id);
    }
}
