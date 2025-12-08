using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.NhaCungCap
{
    public interface INhaCungCapHandler
    {
        Task<Response<NhaCungCapViewModel>> Create(NhaCungCapCreateUpdateModel model, RequestUser byUser);
        Task<Response<NhaCungCapViewModel>> Update(Guid id, NhaCungCapCreateUpdateModel model, RequestUser byUser);
        Task<Response<NhaCungCapViewModel>> GetById(Guid id);
        Task<Response<Pagination<NhaCungCapViewModel>>> GetPage(NhaCungCapQueryModel query);
        Task<Response<NhaCungCapViewModel>> Delete(Guid id);
        Task<Response<MultipleDeleteModel>> DeleteMultiple(List<Guid> ids);
    }
}
