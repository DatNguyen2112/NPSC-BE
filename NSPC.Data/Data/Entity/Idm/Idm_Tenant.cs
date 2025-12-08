using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data
{
    public class Idm_Tenant 
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SubDomain { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Plan { get; set; }
        public int MaxUsers { get; set; }
        public string StatusCode { get; set; }
        public string CompanyName { get; set; }
        public string MST { get; set; }
        public Guid? OwnerId { get; set; }
        public DateTime? ExpiresDate { get; set; }    
        public DateTime? ActiveDate { get; set; }
        public string EmailVerifyToken { get; set; }
        public bool IsDelete { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
        public DateTime CreatedOnDate { get; set; } = DateTime.Now;
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }

        [Column(TypeName = "jsonb")]
        public List<jsonb_Attachment> Attachments { get; set; }

    }
}
