using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    /// <summary>
    /// Bảng nhân sự thực hiện của công việc con
    /// </summary>
    [Table("sm_SubTaskExecutor")]
    public class sm_SubTaskExecutor : BaseTableService<sm_SubTaskExecutor>
    {
        public Guid Id { get; set; }

        /// <value>Mã user</value>
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual idm_User Idm_User { get; set; }

        /// <value>Id công việc công việc con</value>
        public Guid SubTaskId { get; set; }

        [ForeignKey("SubTaskId")]
        public virtual sm_SubTask SubTask { get; set; }
    }
}