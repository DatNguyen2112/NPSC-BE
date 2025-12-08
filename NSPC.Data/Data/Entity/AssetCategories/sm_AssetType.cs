using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Data.Entity.AssetCategories;

[Table("sm_AssetType")]
public class sm_AssetType : BaseTableService<sm_AssetType>
{
    /// <value>ID</value>
    [Key]
    public Guid Id { get; set; }

    /// <value>Mã loại tài sản</value>
    [Required]
    public string Code { get; set; }

    /// <value>Tên loại tài sản</value>
    [Required, MaxLength(100)]
    public string Name { get; set; }

    /// <value>ID nhóm tài sản</value>
    [Required]
    public Guid AssetGroupId { get; set; }

    /// <summary>
    /// Navigation property to asset group.
    /// </summary>
    [ForeignKey(nameof(AssetGroupId))]
    public sm_AssetGroup AssetGroup { get; set; }

    /// <value>Ghi chú</value>
    [MaxLength(255)]
    public string Description { get; set; }

    /// <value>Người tạo</value>
    [ForeignKey(nameof(CreatedByUserId))]
    public idm_User CreatedByUser { get; set; }

    /// <value>Người sửa cuối</value>
    [ForeignKey(nameof(LastModifiedByUserId))]
    public idm_User LastModifiedByUser { get; set; }
}