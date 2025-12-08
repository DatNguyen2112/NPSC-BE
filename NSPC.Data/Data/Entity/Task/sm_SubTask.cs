using NSPC.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    /// <value>Bảng công việc con</value>
    [Table("sm_SubTask")]
    public class sm_SubTask : BaseTableService<sm_SubTask>
    {
        [Key]
        public Guid Id { get; set; }

        /// <value>Tên công việc con</value>
        [Required]
        public string Name { get; set; }

        /// <value>Đã hoàn thành/ Chưa hoàn thành</value>
        public bool IsCompleted { get; set; }

        /// <value>Ngày hết hạn</value>
        public DateTime? DueDate { get; set; }

        /// <value>Tệp đính kèm</value>
        [Column(TypeName = "jsonb")]
        public List<jsonb_Attachment> Attachments { get; set; }

        /// <value>Công việc của dự án</value>
        public Guid TaskId { get; set; }
        [ForeignKey("TaskId")]
        public sm_Task Task { get; set; }
        public virtual ICollection<sm_SubTaskExecutor> SubTaskExecutors { get; set; } = new List<sm_SubTaskExecutor>();
    }
}
