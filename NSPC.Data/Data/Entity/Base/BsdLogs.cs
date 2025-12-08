using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("logs")]
    public class Logs
    {
        [Key]
        public DateTime timestamp { get; set; }

        public string message { get; set; }
        public string message_template { get; set; }
        public int level { get; set; }
        public string exception { get; set; }

        [Column(TypeName = "jsonb")]
        public string log_event { get; set; }

        public string ApplicationId { get; set; }
        public string UserId { get; set; }
    }
}