using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Data.Entity.Asset;
using NSPC.Data.Data.Entity.AssetLocation;

namespace NSPC.Data.Data.Entity.AssetHistory;

public enum AssetBusinessOperation
{
    /// <summary>
    /// Thêm mới
    /// </summary>
    AddNew,

    /// <summary>
    /// Cấp phát
    /// </summary>
    Allocate,

    /// <summary>
    /// Thu hồi
    /// </summary>
    Revoke,

    /// <summary>
    /// Bảo trì
    /// </summary>
    StartMaintenance,

    /// <summary>
    /// Hoàn thành bảo trì
    /// </summary>
    CompleteMaintenance,

    /// <summary>
    /// Điều chuyển
    /// </summary>
    Transfer,

    /// <summary>
    /// Báo hỏng
    /// </summary>
    ReportDamaged,

    /// <summary>
    /// Báo mất
    /// </summary>
    ReportLost,

    /// <summary>
    /// Báo huỷ
    /// </summary>
    ReportDestroyed,

    /// <summary>
    /// Thanh lý
    /// </summary>
    Liquidate
}

[Table("sm_AssetUsageHistory")]
public class sm_AssetUsageHistory : BaseTableService<sm_AssetUsageHistory>
{
    /// <value>ID</value>
    [Key]
    public Guid Id { get; set; }

    /// <value>ID tài sản</value>
    [Required]
    public Guid AssetId { get; set; }

    /// <summary>
    /// Navigation property to sm_Asset
    /// </summary>
    [ForeignKey(nameof(AssetId))]
    public sm_Asset Asset { get; set; }

    /// <value>Nghiệp vụ</value>
    [Required]
    public AssetBusinessOperation Operation { get; set; }

    /// <value>Trạng thái tài sản</value>
    [Required]
    public AssetStatus AssetStatus { get; set; }

    /// <value>Ngày thực hiện</value>
    [Required]
    public DateTime ExecutionDate { get; set; }

    /// <value>ID vị trí tài sản</value>
    public Guid? LocationId { get; set; }

    /// <summary>
    /// Navigation property to sm_AssetLocation
    /// </summary>
    [ForeignKey(nameof(LocationId))]
    public sm_AssetLocation Location { get; set; }

    /// <value>Chi phí</value>
    public decimal? Cost { get; set; }

    /// <value>Ghi chú</value>
    [MaxLength(255)]
    public string Description { get; set; }

    /// <value>ID người sử dụng</value>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Navigation property to idm_User
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public idm_User User { get; set; }

    [MaxLength(50)]
    public string EntityType { get; set; }

    public Guid? EntityId { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public idm_User CreatedByUser { get; set; }

    [ForeignKey(nameof(LastModifiedByUserId))]
    public idm_User LastModifiedByUser { get; set; }
}