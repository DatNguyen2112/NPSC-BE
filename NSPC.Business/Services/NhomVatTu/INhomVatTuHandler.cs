using NSPC.Business.Services.ChucVu;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.NhomVatTu
{
    public interface INhomVatTuHandler
    {
        Task<Response<NhomVatTuViewModel>> Create(NhomVatTuCreateUpdateModel model);
        Task<Response<NhomVatTuViewModel>> Update(Guid id, NhomVatTuCreateUpdateModel model);
        Task<Response<NhomVatTuViewModel>> GetById(Guid id);
        Task<Response<Pagination<NhomVatTuViewModel>>> GetPage(NhomVatTuQueryModel query);
        Task<Response<NhomVatTuViewModel>> Delete(Guid id);
    }
}
