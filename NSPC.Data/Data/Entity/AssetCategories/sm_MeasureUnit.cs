using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Data.Entity.AssetCategories;

[Table("sm_MeasureUnit")]
public class sm_MeasureUnit : BaseTableService<sm_MeasureUnit>
{
    /// <value>ID</value>
    [Key]
    public Guid Id { get; set; }

    /// <value>Mã đơn vị tính</value>
    [Required]
    public string Code { get; set; }

    /// <value>Tên đơn vị tính</value>
    [Required, MaxLength(100)]
    public string Name { get; set; }

    /// <value>Người tạo</value>
    [ForeignKey(nameof(CreatedByUserId))]
    public idm_User CreatedByUser { get; set; }

    /// <value>Người sửa cuối</value>
    [ForeignKey(nameof(LastModifiedByUserId))]
    public idm_User LastModifiedByUser { get; set; }
}