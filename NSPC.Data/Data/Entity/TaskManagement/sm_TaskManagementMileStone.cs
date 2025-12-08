using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data
{
    [Table("sm_TaskManagementMileStone")]
    public class sm_TaskManagementMileStone : BaseTableService<sm_TaskManagementMileStone>
    {
        public Guid Id { get; set; }
        [ForeignKey("sm_TaskManagement")]
        public Guid TaskManagementId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public sm_TaskManagement sm_TaskManagement { get; set; }
    }
}
