using NSPC.Data.Data.Entity.KiemKho;
//using NSPC.Data.Data.Entity.QuanLyKho;
using NSPC.Data.Data.Entity.VatTu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.XuatNhapTon
{
    [Table("mk_XuatNhapTon")]
    public class mk_XuatNhapTon : BaseTableService<mk_XuatNhapTon>
    {
        [Key]
        public Guid Id { get; set; }
        public string MaVatTu { get; set; }
        public string TenVatTu { get; set; }
        public decimal? DonGia { get; set; }
        public string DonViTinh { get; set; }
        public string MaKho { get; set; }
        public int? SoLuong { get; set; }
        public int? SoLuongKiemKe { get; set; }
        public int? SoLuongChenhLech { get; set; }
        public decimal? TongTien { get; set; }
        public string LoaiXuatNhapTon { get; set; }
        public Guid? IdKiemKho { get; set; }
        [ForeignKey("IdKiemKho")]
        public virtual mk_KiemKho mk_KiemKho { get; set; }
        public Guid IdVatTu { get; set; }
        [ForeignKey("IdVatTu")]
        public virtual sm_Product Sm_Product { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual idm_User CreatedByUser { get; set; }
    }
}
