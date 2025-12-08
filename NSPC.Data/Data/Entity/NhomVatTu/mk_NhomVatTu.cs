using NSPC.Data.Data.Entity.VatTu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.NhomVatTu
{
    [Table("mk_NhomVatTu")]
    public class mk_NhomVatTu: BaseTableService<mk_NhomVatTu>
    {
        [Key]
        public Guid Id { get; set; }
        public virtual ICollection<sm_Product> Sm_Products { get; set; }
        [Required]
        public string MaNhom { get; set; }
        public string TenNhom { get; set; }
        public string GhiChu { get; set; }
    }
}
