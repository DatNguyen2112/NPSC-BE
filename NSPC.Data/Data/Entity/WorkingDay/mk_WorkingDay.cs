using NSPC.Data.Data.Entity.ChamCong;
using NSPC.Data.Data.Entity.DuAn;
using NSPC.Data.Data.Entity.NguyenVatLieu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.WorkingDay
{
    [Table("mk_WorkingDay")]
    public class mk_WorkingDay : BaseTableService<mk_WorkingDay>
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int Day { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
        [Required, MaxLength(10)]
        public string Type { get; set; }
        public string Note { get; set; }
        [MaxLength(10)]
        public string OriginalType { get; set; }
        public bool IsOverride { get; set; }
        //[ForeignKey("IdChamCong")]
        //public virtual mk_ChamCong mk_ChamCong { get; set; }
    }
}
