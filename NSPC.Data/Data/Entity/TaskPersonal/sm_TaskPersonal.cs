using NSPC.Data.Entity;
using SaleManagement.Data.Data.Entity.TaskHistory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum TaskPersonalStatus
{
    /// <value>Chưa bắt đầu</value>
    NotStarted,

    /// <value>Đang thực hiện</value>
    InProgress,

    /// <value>Không đạt</value>
    Failed,

    /// <value>Đạt</value>
    Passed
}
public enum PriorityPersonalLevel
{
    /// <value>Cao</value>
    High,
    /// <value>Trung bình</value>x
    Medium,
    /// <value>Thấp</value>
    Low,
}
namespace NSPC.Data
{
    [Table("sm_TaskPersonal")]
    public class sm_TaskPersonal: BaseTableService<sm_TaskPersonal>
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Code { get; set; }
        
        /// <value>Tên công việc</value>
        [Required]
        public string Name { get; set; }
        
        /// <value>Ngày bắt đầu thực hiện</value>
        public DateTime? StartDateTime { get; set; }

        /// <value>Ngày kết thúc thực hiện</value>
        public DateTime? EndDateTime { get; set; }
        
        /// <value>Loại công việc</value>
        public string TaskType { get; set; }
        
        /// <value>Độ ưu tiên</value>
        public PriorityPersonalLevel PriorityLevel { get; set; } = PriorityPersonalLevel.Medium;

        /// <value>Trạng thái</value>
        public TaskStatus Status { get; set; }
        
        /// <value>Ghi chú</value>
        public string Note { get; set; }
        
        public virtual ICollection<sm_SubTaskPersonal> SubTasksPersonal { get; set; } = new List<sm_SubTaskPersonal>();
    }
}

