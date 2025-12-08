using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Data.Entity.PhongBan;

namespace NSPC.Data.Data.Entity.AssetLocation;

[Table("sm_AssetLocation")]
public class sm_AssetLocation : BaseTableService<sm_AssetLocation>
{
    /// <value>ID</value>
    [Key]
    public Guid Id { get; set; }

    /// <value>Tên vị trí</value>
    [Required, MaxLength(100)]
    public string Name { get; set; }

    /// <value>Mã vị trí</value>
    [Required]
    public string Code { get; set; }

    /// <value>ID vị trí cha</value>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Navigation property to parent location.
    /// </summary>
    [ForeignKey(nameof(ParentId))]
    public sm_AssetLocation Parent { get; set; }

    /// <value>Ghi chú</value>
    [MaxLength(255)]
    public string Description { get; set; }

    /// <value>Người tạo</value>
    [ForeignKey(nameof(CreatedByUserId))]
    public idm_User CreatedByUser { get; set; }

    /// <value>Người sửa cuối</value>
    [ForeignKey(nameof(LastModifiedByUserId))]
    public idm_User LastModifiedByUser { get; set; }

    /// <summary>
    /// Navigation property to child locations.
    /// </summary>
    public virtual ICollection<sm_AssetLocation> Children { get; set; }
}