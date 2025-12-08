using NSPC.Data.Data.Entity.VatTu;
using NSPC.Data.Data.Entity.WorkingDay;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.ChamCong
{
    [Table("mk_ChamCongItem")]
    public class mk_ChamCongItem : BaseTableService<mk_ChamCongItem>
    {
        [Key]
        public Guid Id { get; set; }
        public string MaSo { get; set; }
        public string HoVaTen { get; set; }
        public string ChucVu { get; set; }
        public DateTime? Date { get; set; }
        public decimal? NgayCong { get; set; }
        public List<NgayTrongThang> NgayTrongThang { get; set; }
        public decimal? LamThemNgayThuong { get; set; }
        public decimal? LamThemChuNhat { get; set; }
        public decimal? LamThemNgayLe { get; set; }
        public int Order { get; set; }
        public decimal? LuongCoBan { get; set; }
        public decimal? AnCa { get; set; }
        public decimal? DienThoai { get; set; }
        public decimal? TrangPhuc { get; set; }
        public Guid IdChamCong { get; set; }
        [ForeignKey("IdChamCong")]
        public virtual mk_ChamCong mk_ChamCong { get; set; }
    }
}
