using NSPC.Data.Data.Entity.ChamCong;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.BangTinhLuong
{
    [Table("mk_BangTinhLuong")]
    public class mk_BangTinhLuong : BaseTableService<mk_BangTinhLuong>
    {
        [Key]
        public Guid Id { get; set; }
        public string TenBangTinhLuong { get; set; }
        public string TenCongTy { get; set; }
        public string DiaChiCongTy { get; set; }
        public string MaSoThue { get; set; }
        public Guid IdBangChamCongActive { get; set; }
        public DateTime ThoiGian { get; set; }
        public int SoNgayCongTrongThang { get; set; }
        public bool KichHoatBangLuong { get; set; }
        public virtual ICollection<mk_BangLuongItem> BangLuongItem { get; set; }
    }
}
