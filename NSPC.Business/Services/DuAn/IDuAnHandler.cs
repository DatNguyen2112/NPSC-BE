using NSPC.Business.Services.ChucVu;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.DuAn
{
    public interface IDuAnHandler
    {
        Task<Response<DuAnViewModel>> Create(DuAnCreateUpdateModel model, RequestUser byUser);
        Task<Response<DuAnViewModel>> Update(Guid id, DuAnCreateUpdateModel model, RequestUser byUser);
        Task<Response<DuAnViewModel>> GetById(Guid id);
        Task<Response<Pagination<DuAnViewModel>>> GetPage(DuAnQueryModel query);
        Task<Response<DuAnViewModel>> Delete(Guid id);
    }
}
