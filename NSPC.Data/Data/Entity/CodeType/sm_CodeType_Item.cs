using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.CodeType
{
    [Table("sm_CodeType_Item")]
    public class sm_CodeType_Item : BaseTableService<sm_CodeType_Item>
    {
        [Key]
        public Guid Id { get; set; }

        public int LineNumber { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }

        public string IconClass { get; set; }

        public Guid CodeTypeId { get; set; }

        [ForeignKey("CodeTypeId")]
        public virtual sm_CodeType sm_CodeType { get; set; }
    }
}
