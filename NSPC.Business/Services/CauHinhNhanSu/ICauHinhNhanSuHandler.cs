using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.CauHinhNhanSu
{
    public interface ICauHinhNhanSuHandler
    {
        //Task<Response<CauHinhNhanSuViewModel>> Create(CauHinhNhanSuCreateUpdateModel model);
        Task<Response<CauHinhNhanSuViewModel>> Update(Guid id, CauHinhNhanSuCreateUpdateModel model);
        Task<Response<CauHinhNhanSuViewModel>> GetById(Guid id);
        Task<Response<Pagination<CauHinhNhanSuViewModel>>> GetPage(CauHinhNhanSuQueryModel query);
        //Task<Response<CauHinhNhanSuViewModel>> Delete(Guid id);
        Task<Response> SeedCauHinh();

    }
}
