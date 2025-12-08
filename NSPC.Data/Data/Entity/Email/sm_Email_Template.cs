using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace NSPC.Data
{
    [Table("sm_Email_Template")]
    public class sm_Email_Template : BaseTableDefault
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(128)]
        public String Name { get; set; }

        [MaxLength(64)]
        public string Code { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        public ICollection<sm_Email_Template_Translation> sm_Email_Template_Translation { get; set; }
    }
}