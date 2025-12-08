using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.PhongBan
{
    public class PhongBanViewModel
    {
        public Guid Id { get; set; }
        public string MaPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public string GhiChu { get; set; }
    }
    public class PhongBanCreateUpdateModel
    {
        public string MaPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public string GhiChu { get; set; }
    }
    public class PhongBanQueryModel : PaginationRequest
    {
        public string MaPhongBan { get; set; }
        public string TenPhongBan { get; set; }
    }
}
