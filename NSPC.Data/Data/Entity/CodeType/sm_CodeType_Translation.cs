using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("sm_CodeType_Translation")]
    public class sm_CodeType_Translation : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Language { get; set; }
        public string Type { get; set; }

        public Guid CodeTypeId { get; set; }

        [ForeignKey("CodeTypeId")]
        public virtual sm_CodeType ub_CodeType { get; set; }
    }
}