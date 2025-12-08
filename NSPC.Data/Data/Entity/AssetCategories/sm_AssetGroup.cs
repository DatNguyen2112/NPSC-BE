using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Data.Entity.AssetCategories;

[Table("sm_AssetGroup")]
public class sm_AssetGroup : BaseTableService<sm_AssetGroup>
{
    /// <value>ID</value>
    [Key]
    public Guid Id { get; set; }

    /// <value>Mã nhóm tài sản</value>
    [Required]
    public string Code { get; set; }

    /// <value>Tên nhóm tài sản</value>
    [Required, MaxLength(100)]
    public string Name { get; set; }

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
    /// Navigation property to asset types.
    /// </summary>
    public virtual ICollection<sm_AssetType> AssetTypes { get; set; }
}