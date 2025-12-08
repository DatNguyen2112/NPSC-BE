using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data
{
    [Table("sm_TaskManagementComment")]
    public class sm_TaskManagementComment : BaseTableService<sm_TaskManagementComment>
    {
        public Guid Id { get; set; }
        public Guid TaskManagementId { get; set; }
        public Guid? TaskManagementCommentReplyId { get; set; }
        public string Content { get; set; }
        [ForeignKey("TaskManagementId")]
        public sm_TaskManagement sm_TaskManagement { get; set; }
        [ForeignKey("CreatedByUserId")]
        public idm_User idm_User { get; set; }
    }
}
