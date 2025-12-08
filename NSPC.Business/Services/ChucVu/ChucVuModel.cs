using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.ChucVu
{
    public class ChucVuViewModel
    {
        public Guid Id { get; set; }
        public string MaChucVu { get; set; }
        public string TenChucVu { get; set; }
        public string GhiChu { get; set; }
    }
    public class ChucVuCreateUpdateModel
    {
        public string MaChucVu { get; set; }
        public string TenChucVu { get; set; }
        public string GhiChu { get; set; }
    }
    public class ChucVuQueryModel : PaginationRequest
    {
        public string MaChucVu { get; set; }
        public string TenChucVu { get; set; }
    }
}
