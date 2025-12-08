/*using NSPC.Business.Services.ChucVu;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.SanPham
{
    public interface ISanPhamHandler
    {
        Task<Response<SanPhamViewModel>> Create(SanPhamCreateUpdateModel model);
        Task<Response<SanPhamViewModel>> Update(Guid id, SanPhamCreateUpdateModel model);
        Task<Response<SanPhamViewModel>> GetById(Guid id);
        Task<Response<Pagination<SanPhamViewModel>>> GetPage(SanPhamQueryModel query);
        Task<Response<SanPhamViewModel>> Delete(Guid id);
    }
}
*/