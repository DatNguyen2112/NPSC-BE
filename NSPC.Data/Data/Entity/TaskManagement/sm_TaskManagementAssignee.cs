using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data
{
    [Table("sm_TaskManagementAssignee")]
    public class sm_TaskManagementAssignee : BaseTableService<sm_TaskManagementAssignee>
    {
        public Guid Id { get; set; }
        public Guid TaskManagementId { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("TaskManagementId")]
        public sm_TaskManagement sm_TaskManagement { get; set; }
        [ForeignKey("UserId")]
        public idm_User idm_User { get; set; }
    }
}
