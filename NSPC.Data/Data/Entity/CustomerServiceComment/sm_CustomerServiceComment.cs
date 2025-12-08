using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Entity
{
    [Table("sm_CustomerServiceComment")]
    public class sm_CustomerServiceComment: BaseTableService<sm_CustomerServiceComment>
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid ConstructionId  { get; set; }
        
        public List<string> TagIds  { get; set; } = new List<string>();
        public string Content { get; set; }
        
        [ForeignKey("ConstructionId")]
        public virtual sm_Construction sm_Construction { get; set; }
        
        [ForeignKey("CreatedByUserId")]
        public virtual idm_User CreatedByUser { get; set; }
    }
}
