using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.PhongBan
{
    [Table("mk_PhongBan")]
    public class mk_PhongBan : BaseTableService<mk_PhongBan>
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string MaPhongBan { get; set; }
        [Required]
        public string TenPhongBan { get; set; }
        public string GhiChu { get; set; }
    }
}
