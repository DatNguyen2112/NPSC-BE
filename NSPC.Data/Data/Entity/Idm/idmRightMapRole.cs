using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("IdmRightMapRole")]
    public class IdmRightMapRole : BaseTableService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string RoleCode { get; set; }
        public string RightCode { get; set; }
        public Guid? RightId { get; set; }
        public Guid? RoleId { get; set; }
        [ForeignKey("RoleId")]
        public IdmRole Role { get; set; }
        [ForeignKey("RightId")]
        public IdmRight Right { get; set; }
    }
}

