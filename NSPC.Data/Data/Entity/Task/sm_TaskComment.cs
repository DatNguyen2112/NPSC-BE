using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    [Table("sm_TaskComment")]
    public class sm_TaskComment : BaseTableService<sm_TaskComment>
    {
        [Key]
        public Guid Id { get; set; }

        public Guid TaskId { get; set; }

        public List<string> TagIds { get; set; } = new List<string>();
        public string Content { get; set; }

        [ForeignKey("TaskId")]
        public virtual sm_Task Task { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual idm_User CreatedByUser { get; set; }
    }
}
