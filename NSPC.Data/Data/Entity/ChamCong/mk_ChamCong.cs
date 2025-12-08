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
    [Table("mk_ChamCong")]
    public class mk_ChamCong : BaseTableService<mk_ChamCong>
    {
        [Key]
        public Guid Id { get; set; }
        public string TenBangChamCong { get; set; }
        public DateTime Date { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int Cong { get; set; }
        public virtual ICollection<mk_ChamCongItem> ListChamCong { get; set; }
        public bool KichHoatBangChamCong { get; set; }
    }
}
