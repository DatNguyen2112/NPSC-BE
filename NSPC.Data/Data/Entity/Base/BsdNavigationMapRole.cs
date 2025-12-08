using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("bsd_Navigation_Map_Role")]
    public class BsdNavigationMapRole : BaseTableDefault
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("Navigation")]
        public Guid NavigationId { get; set; }

        public Guid RoleId { get; set; }
        public Guid? FromSubNavigation { get; set; }
        public BsdNavigation Navigation { get; set; }
    }
}