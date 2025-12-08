using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("sm_TemplateStage")]

    public class sm_TemplateStage : BaseTableService<sm_TemplateStage>
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Vị trí của giai đoạn trong quy trình
        /// </summary>
        public int StepOrder { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        /// <summary>
        /// Hạn dự kiến hoàn thành
        /// </summary>
        public DateTime? ExpiredDate { get; set; }
        /// <summary>
        /// id của mẫu dự án
        /// </summary>
        public Guid ProjectTemplateId { get; set; }
        [ForeignKey("ProjectTemplateId")]
        public sm_ProjectTemplate ProjectTemplate { get; set; }
        public virtual ICollection<sm_Task> Tasks { get; set; } = new List<sm_Task>();
    }
}