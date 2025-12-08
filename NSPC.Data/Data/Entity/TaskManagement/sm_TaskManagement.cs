using NSPC.Data.Data.Entity.DuAn;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data
{
    [Table("sm_TaskManagement")] 
    
    public class sm_TaskManagement : BaseTableService<sm_TaskManagement>
    {
        public Guid Id { get; set; }
        // [ForeignKey("Parent")]
        // public Guid? ParentId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        [ForeignKey("sm_Construction")]
        public Guid? ConstructionId { get; set; }
        public string Status { get; set; }
        [Column(TypeName = "jsonb")]
        public List<jsonb_Attachment> Attachments { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        // [InverseProperty("Parent")] 
        // public ICollection<sm_TaskManagement> InverseParent { get; set; }
        // public sm_TaskManagement Parent { get; set; }
        public sm_Construction sm_Construction { get; set; }
        public ICollection<sm_TaskManagementAssignee> sm_TaskManagementAssignees { get; set; }
        public ICollection<sm_TaskManagementComment> sm_TaskManagementComments { get; set; }
        public ICollection<sm_TaskManagementHistory> sm_TaskManagementHistories { get; set; }
        public ICollection<sm_TaskManagementMileStone> sm_TaskManagementMileStones { get; set; }

    }
}
