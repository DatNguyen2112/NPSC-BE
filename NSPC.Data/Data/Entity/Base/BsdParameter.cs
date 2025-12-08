using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("bsd_Parameter")]
    public class BsdParameter : BaseTable<BsdParameter>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(64)]
        public string Name { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        public string Value { get; set; }

        public bool IsSystem { get; set; }

        [StringLength(64)]
        public string GroupCode { get; set; }
    }
}