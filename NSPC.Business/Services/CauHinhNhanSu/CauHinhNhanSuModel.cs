using NSPC.Business.Services.ChucVu;
using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using NSPC.Data.Data.Entity.ChucVu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.CauHinhNhanSu
{
    public class CauHinhNhanSuViewModel
    {
        public Guid Id { get; set; }
        public string? Ma { get; set; }
        public string TenNhanSu { get; set; }
        public ChucVuViewModel ChucVu { get; set; }
        public PhongBanViewModel PhongBan { get; set; }
        public decimal? LuongCoBan { get; set; }
        public decimal? AnCa { get; set; }
        public decimal? DienThoai { get; set; }
        public decimal? TrangPhuc { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }
    public class CauHinhNhanSuCreateUpdateModel
    {
        public decimal? LuongCoBan { get; set; }
        public decimal? AnCa { get; set; }
        public decimal? DienThoai { get; set; }
        public decimal? TrangPhuc { get; set; }
    }
    public class CauHinhNhanSuQueryModel : PaginationRequest
    {
        public string Ma { get; set; }
        public string ChucVu { get; set; }
        public string PhongBan { get; set; }
    }
}
