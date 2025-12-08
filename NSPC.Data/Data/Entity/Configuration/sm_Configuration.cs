using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Data.Entity.Configuration;

[Table("sm_Configuration")]
public class sm_Configuration : BaseTableService<sm_Configuration>
{
    [Key]
    [Column(TypeName = "char")]
    [MaxLength(20)]
    public string Key { get; set; }

    public string Value { get; set; }
    public string Description { get; set; }
}