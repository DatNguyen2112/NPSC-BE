using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace NSPC.Data.Data.Entity.Feedback

{
    [Table("sm_Feedback")]
    public class sm_Feedback : BaseTableService<sm_Feedback>
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string[] Module { get; set; }
        public string Content { get; set; }
        public int Rate { get; set; }

    }
}
