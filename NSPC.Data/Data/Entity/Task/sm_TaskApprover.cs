using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    /// <summary>
    /// Bảng người phê duyệt
    /// </summary>
    [Table("sm_TaskApprover")]
    public class sm_TaskApprover : BaseTableService<sm_TaskApprover>
    {
        [Key]
        public Guid Id { get; set; }

        /// <value>Mã user</value>
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual idm_User Idm_User { get; set; }

        /// <value>Id công việc</value>
        public Guid TaskId { get; set; }

        [ForeignKey("TaskId")]
        public virtual sm_Task Task { get; set; }
    }
}