using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("bsd_Navigation")]
    public class BsdNavigation : BaseTable<BsdNavigation>
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Parent")]
        public Guid? ParentId { get; set; }

        [Required]
        [StringLength(64)]
        public string Code { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string UrlRewrite { get; set; }

        [Required]
        [StringLength(450)]
        public string IdPath { get; set; }

        [Required]
        [StringLength(900)]
        public string Path { get; set; }

        [StringLength(1024)]
        public string SubUrl { get; set; }

        [StringLength(64)]
        public string IconClass { get; set; }

        public bool? Status { get; set; }
        public int? Order { get; set; }
        public bool HasChild { get; set; }
        public int Level { get; set; }
        public int? Type { get; set; }

        public BsdNavigation Parent { get; set; }

        [InverseProperty("Parent")]
        public ICollection<BsdNavigation> InverseParent { get; set; }

        [JsonIgnore]
        public ICollection<BsdNavigationMapRole> NavigationRole { get; set; }
        public string QueryParams { get; set; }
        public bool IsDefault { get; set; }
    }
}