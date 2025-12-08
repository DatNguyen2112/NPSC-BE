using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("IdmRight")]
    public class IdmRight : BaseTableDefault
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(64)]
        public string Code { get; set; }
        [StringLength(128)]
        public string Description { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public ICollection<IdmRightMapRole> RightMapRole { get; set; }
        public bool IsDefault { get; set; }
    }
}

