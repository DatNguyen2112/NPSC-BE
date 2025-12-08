using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Data.Entity.ActivityHistory;

[Table("sm_ActiviyHisroty")]
public class sm_ActiviyHisroty : BaseTableService<sm_ActiviyHisroty>
{
    [Key]
    public Guid Id { get; set; }

    public Guid EntityId { get; set; }
    public string EntityType { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }
}