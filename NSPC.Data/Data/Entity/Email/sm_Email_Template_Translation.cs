using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    public class sm_Email_Template_Translation : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("sm_Email_Template")]
        public Guid EmailTemplateId { get; set; }

        [MaxLength(512)]
        public string TitleTemplate { get; set; }

        public string BodyTemplate { get; set; }

        [MaxLength(3)]
        public string Language { get; set; }

        public virtual sm_Email_Template sm_Email_Template { get; set; }
    }
}