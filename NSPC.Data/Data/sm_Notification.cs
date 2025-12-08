using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NSPC.Data
{
    public class sm_Notification : BaseTableDefault
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        public Guid? ReceiverUserId { get; set; }
        public bool IsReceiverRead { get; set; }
        public bool IsReceiverSeen { get; set; }
        public DateTime? ReceiverReadOnDate { get; set; }
        public string Type { get; set; }
        [ForeignKey(nameof(ReceiverUserId))]
        public virtual idm_User ReceiverUser { get; set; }
        public Guid? CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        public virtual idm_User CreatedByUser { get; set; }
        public string JsonData { get; set; }
        public string JsonDataType { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }

    }

    public class sm_Notification_Template : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public ICollection<sm_Notification_Template_Translation> Translations { get; set; }
    }

    public class sm_Notification_Template_Translation : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid NotificationTemplateId { get; set; }

        [MaxLength(512)]
        public string TitleTemplate { get; set; }

        public string BodyPlainTextTemplate { get; set; }
        public string BodyHtmlTemplate { get; set; }

        [MaxLength(3)]
        public string Language { get; set; }

        [ForeignKey("NotificationTemplateId")]
        public virtual sm_Notification_Template ub_Notification_Template { get; set; }
    }

}
