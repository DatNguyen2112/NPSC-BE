using NPSC.Data;
using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data.Entity.Contract;
using NSPC.Data.Data.Entity.DuAn;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.InventoryNote
{
    [Table("sm_IssueManagement")]
    public class sm_IssueManagement : BaseTableService<sm_IssueManagement>
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(64)]
        public string Code { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string PriorityLevel { get; set; }
        public string Content { get; set; }
        [StringLength(512)]
        public string Description { get; set; }

        [Column(TypeName = "jsonb")]
        public List<jsonb_Attachment> Attachments { get; set; }    

        [StringLength(64)]
        public string Status{ get; set; } // Mã trạng thái: value ---> DRAFT, COMPLETED, CANCELLED   
        public Guid? ConstructionId { get; set; }
        [ForeignKey("ConstructionId")]
        public virtual sm_Construction sm_Construction { get; set; }
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual idm_User Idm_User { get; set; }
        public Guid? IssueLogId { get; set; }
        [ForeignKey("IssueLogId")]
        public virtual sm_IssueActivityLog sm_IssueActivityLog { get; set; }
    }
}
