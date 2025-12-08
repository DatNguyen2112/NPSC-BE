using NSPC.Business.Services.ChucVu;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.VatTu
{
    public interface IVatTuHandler
    {
        Task<Response<VatTuViewModel>> Create(VatTuCreateUpdateModel model, RequestUser byUser);
        Task<Response<VatTuViewModel>> Update(Guid id, VatTuCreateUpdateModel model, RequestUser byUser);
        Task<Response<VatTuViewModel>> GetById(Guid id);
        Task<Response<Pagination<VatTuViewModel>>> GetPage(VatTuQueryModel query);
        Task<Response<VatTuViewModel>> Delete(Guid id);
        Task<Response> DeleteMultiple(List<Guid> ids);
    }
}
