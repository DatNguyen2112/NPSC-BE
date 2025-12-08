using NSPC.Business.Services.ChucVu;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.Bom
{
    public interface IBomHandler
    {
        Task<Response<BomViewModel>> Create(BomCreateUpdateModel model, RequestUser byUser);
        Task<Response<BomViewModel>> Update(Guid id, BomCreateUpdateModel model, RequestUser byUser);
        Task<Response<BomViewModel>> GetById(Guid id);
        Task<Response<Pagination<BomViewModel>>> GetPage(BomQueryModel query);
        Task<Response> Delete(Guid id);
    }
}
