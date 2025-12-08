using NSPC.Data.Data.Entity.PhongBan;
using NSPC.Data.Data.Entity.VatTu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.ChucVu
{
    [Table("mk_ChucVu")]
    public class mk_ChucVu : BaseTableService<mk_ChucVu>
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string MaChucVu { get; set; }
        [Required]
        public string TenChucVu { get; set; }
        public string GhiChu { get; set; }
    }
}
