using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data
{
    [Table("sm_TaskManagementHistory")]
    public class sm_TaskManagementHistory : BaseTableService<sm_TaskManagementHistory>
    {
        public Guid Id { get; set; }
        public Guid TaskManagementId { get; set; }
        public string Action { get; set; }
        [ForeignKey("TaskManagementId")]
        public sm_TaskManagement sm_TaskManagement { get; set; }
    }
}
