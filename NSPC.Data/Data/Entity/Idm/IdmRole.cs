using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data

{
    [Table("idm_Role")]
    public class IdmRole : BaseTableDefault
    {
        public IdmRole()
        {
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [StringLength(64)]
        public string Code { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        public bool IsSystem { get; set; }
        public int Level { get; set; }
        
        public ICollection<IdmRightMapRole> RoleMapRight { get; set; }
    }
}