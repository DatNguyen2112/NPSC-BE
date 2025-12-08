using NSPC.Data.Entity;
using SaleManagement.Data.Data.Entity.TaskHistory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public enum TaskStatus
{
    /// <value>Chưa bắt đầu</value>
    NotStarted,

    /// <value>Đang thực hiện</value>
    InProgress,

    /// <value>Chờ duyệt</value>
    PendingApproval,

    /// <value>Không đạt</value>
    Failed,

    /// <value>Đạt</value>
    Passed
}
public enum PriorityLevel
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
    [Table("sm_Task")]
    public class sm_Task : BaseTableService<sm_Task>
    {
        [Key]
        public Guid Id { get; set; }

        /// <value>Mã công việc</value>
        public string Code { get; set; }

        /// <value>Tên công việc</value>
        [Required]
        public string Name { get; set; }

        /// <value>Ngày bắt đầu thực hiện</value>
        public DateTime? StartDateTime { get; set; }

        /// <value>Ngày kết thúc thực hiện</value>
        public DateTime? EndDateTime { get; set; }

        /// <value>Mô tả</value>
        public string Description { get; set; }

        /// <value>Độ ưu tiên</value>
        public PriorityLevel PriorityLevel { get; set; } = PriorityLevel.Medium;

        /// <value>Trạng thái</value>
        public TaskStatus Status { get; set; }

        /// <summary>
        /// Vị trí của công việc
        /// </summary>
        public int? StepOrder { get; set; }

        /// <value>Tệp đính kèm</value>
        [Column(TypeName = "jsonb")]
        public List<jsonb_Attachment> Attachments { get; set; }

        /// <value>Fk dự án</value>
        public Guid? ConstructionId { get; set; }
        [ForeignKey("ConstructionId")]
        public sm_Construction Construction { get; set; }

        /// <value>Lưu trữ id của TemplateStage ko fk</value>
        public Guid? IdTemplateStage { get; set; }

        
        /// <value>Thứ tự ưu tiên của công việc</value>
        public int? PriorityOrder { get; set; }

        /// <value>FK giai đoạn thực hiện</value>
        public Guid? TemplateStageId { get; set; }
        [ForeignKey("TemplateStageId")]
        public sm_TemplateStage TemplateStage { get; set; }
        public virtual ICollection<sm_SubTask> SubTasks { get; set; } = new List<sm_SubTask>();
        public virtual ICollection<sm_TaskUsageHistory> TaskUsageHistories { get; set; } = new List<sm_TaskUsageHistory>();
        public virtual ICollection<sm_TaskApprover> Approvers { get; set; } = new List<sm_TaskApprover>();
        public virtual ICollection<sm_TaskExecutor> Executors { get; set; } = new List<sm_TaskExecutor>();
    }
}
