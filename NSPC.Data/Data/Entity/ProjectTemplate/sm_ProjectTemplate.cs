using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("sm_ProjectTemplate")]

    public class sm_ProjectTemplate : BaseTableService<sm_ProjectTemplate>
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Mã
        /// </summary>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// Tên
        /// </summary>ư
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Danh sách giai đoạn thực hiện
        /// </summary>
        public virtual ICollection<sm_TemplateStage> TemplateStages { get; set; } = new List<sm_TemplateStage>();
    }
}
