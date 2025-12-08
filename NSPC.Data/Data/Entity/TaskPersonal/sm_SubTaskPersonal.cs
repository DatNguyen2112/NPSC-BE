using NSPC.Data.Entity;
using SaleManagement.Data.Data.Entity.TaskHistory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("sm_SubTaskPersonal")]
    public class sm_SubTaskPersonal: BaseTableService<sm_SubTaskPersonal>
    {
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int LineNo { get; set; }

        /// <value>Tên công việc con</value>
        [Required]
        public string Name { get; set; }

        /// <value>Đã hoàn thành/ Chưa hoàn thành</value>
        public bool IsCompleted { get; set; }

        /// <value>Ngày hết hạn</value>
        public DateTime? DueDate { get; set; }

        /// <value>Công việc của dự án</value>
        public Guid TaskPersonalId { get; set; }
        [ForeignKey("TaskPersonalId")]
        public sm_TaskPersonal TaskPersonal { get; set; }
    }
}

