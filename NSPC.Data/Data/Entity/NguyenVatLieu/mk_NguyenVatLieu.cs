using NSPC.Data.Data.Entity.Bom;
using NSPC.Data.Data.Entity.VatTu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.NguyenVatLieu
{
    [Table("mk_NguyenVatLieu")]
    public class mk_NguyenVatLieu : BaseTableService<mk_NguyenVatLieu>
    {
        [Key]
        public Guid Id { get; set; }
        public string TenNguyenVatLieu { get; set; }
        public string MaNguyenVatLieu { get; set; }
        public string DonViTinh { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public Guid IdBom { get; set; }
        [ForeignKey("IdBom")]
        public virtual mk_Bom mk_Bom { get; set; }
        public Guid IdVatTu { get; set; }
        [ForeignKey("IdVatTu")]
        public virtual sm_Product Sm_Product { get; set; }
        public decimal? TongTien { get; set; }
    }
}
