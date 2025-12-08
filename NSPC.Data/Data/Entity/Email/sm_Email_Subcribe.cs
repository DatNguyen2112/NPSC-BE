using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NSPC.Data
{
    [Table("sm_Email_Subscribe")]
    public class sm_Email_Subscribe : BaseTableDefault
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime? SubscribeDate { get; set; }
        public DateTime? UnsubscribeDate { get; set; }

        [MaxLength(20)]
        public string Status { get; set; }
        public int TotalEmailSentCount { get; set; }
    }
}
