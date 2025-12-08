using NSPC.Data.Data.Entity.NguyenVatLieu;
using NSPC.Data.Data.Entity.NhomVatTu;
using NSPC.Data.Data.Entity.VatTu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.Bom
{
    [Table("mk_Bom")]
    public class mk_Bom : BaseTableService<mk_Bom>
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdSanPham { get; set; }
        [ForeignKey("IdSanPham")]
        public virtual sm_Product Sm_Product { get; set; }
        public virtual ICollection<mk_NguyenVatLieu> NguyenVatLieu { get; set; }
        public decimal? TongTienThanhToan { get; set; }
        public decimal? TongTien { get; set; }
        public double VAT { get; set; }
    }
}
