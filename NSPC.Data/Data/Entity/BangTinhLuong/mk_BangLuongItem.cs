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
    [Table("mk_BangLuongItem")]
    public class mk_BangLuongItem : BaseTableService<mk_BangLuongItem>
    {
        [Key]
        public Guid Id { get; set; }
        public string MaSo { get; set; }
        public string TenNhanVien { get; set; }
        public string ChucVu { get; set; }
        public decimal? LuongCoBan { get; set; }
        public mk_CacKhoanTroCap CacKhoanTroCap { get; set; }
        public decimal? Tong { get; set; }
        public decimal? NgayCong { get; set; }
        public decimal? Luong { get; set; }
        public int Order { get; set; }
        public decimal? BhxhNLD { get; set; }
        public decimal? BhytNLD { get; set; }
        public decimal? BhtnNLD { get; set; }
        public decimal? TongNLD { get; set; }
        public decimal? BhxhNSDLD { get; set; }
        public decimal? BhytNSDLD { get; set; }
        public decimal? BhtnNSDLD { get; set; }
        public decimal? TongNSDLD { get; set; }
        public decimal? TongTatCa { get; set; }
        public decimal? ThuNhapCaNhan { get; set; }
        public decimal? GiamTruBanThan { get; set; }
        public decimal? SoNguoiPhuThuoc { get; set; }
        public decimal? SoTienPhuThuoc { get; set; }
        public decimal? ThuNhapTinhThue { get; set; }
        public decimal? ThueTNCNPhaiNop { get; set; }
        public decimal? ThucLinh { get; set; }
        public DateTime? NgayNhan { get; set; }
        public string GhiChu { get; set; }
        public Guid IdBangTinhLuong { get; set; }
        [ForeignKey("IdBangTinhLuong")]
        public virtual mk_BangTinhLuong mk_BangTinhLuong { get; set; }
    }
}
