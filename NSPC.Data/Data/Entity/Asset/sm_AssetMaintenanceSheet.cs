using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Data.Entity.Asset;

public enum MaintenanceType
{
    /// <summary>
    /// Bảo dưỡng
    /// </summary>
    Maintenance,

    /// <summary>
    /// Sửa chữa lỗi hỏng
    /// </summary>
    Repair
}

public enum MaintenanceLocation
{
    /// <summary>
    /// Tại đơn vị
    /// </summary>
    Internal,

    /// <summary>
    /// Tại nhà cung cấp
    /// </summary>
    Supplier
}

public enum MaintenanceStatus
{
    /// <summary>
    /// Đang bảo trì
    /// </summary>
    InProgress,

    /// <summary>
    /// Đã hoàn thành
    /// </summary>
    Completed
}

[Table("sm_AssetMaintenanceSheet")]
public class sm_AssetMaintenanceSheet : BaseTableService<sm_AssetMaintenanceSheet>
{
    /// <value>ID</value>
    [Key]
    public Guid Id { get; set; }

    /// <value>Mã phiếu bảo trì</value>
    [Required]
    public string Code { get; set; }

    /// <value>ID tài sản</value>
    [Required]
    public Guid AssetId { get; set; }

    /// <summary>
    /// Navigation property to sm_Asset
    /// </summary>
    [ForeignKey(nameof(AssetId))]
    public sm_Asset Asset { get; set; }

    /// <value>Loại bảo trì</value>
    [Required]
    public MaintenanceType MaintenanceType { get; set; }

    /// <value>Ngày bắt đầu</value>
    public DateTime? StartDate { get; set; }

    /// <value>Hạn bảo trì</value>
    [Required]
    public DateTime MaintenancePeriod { get; set; }

    /// <value>ID người thực hiện</value>
    public Guid? PerformerId { get; set; }

    /// <summary>
    /// Navigation property to idm_User
    /// </summary>
    [ForeignKey(nameof(PerformerId))]
    public idm_User Performer { get; set; }

    /// <value>Địa điểm bảo trì</value>
    [Required]
    public MaintenanceLocation MaintenanceLocation { get; set; }

    /// <value>Đơn vị bảo trì</value>
    [MaxLength(100)]
    public string MaintenancePlace { get; set; }

    /// <value>Chi phí dự kiến</value>
    public decimal? EstimatedCost { get; set; }

    /// <value>Nội dung bảo trì</value>
    [MaxLength(500)]
    public string MaintenanceContent { get; set; }

    /// <value>Trạng thái bảo trì</value>
    [Required]
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.InProgress;

    /// <value>Ngày hoàn thành bảo trì</value>
    public DateTime? CompleteDate { get; set; }

    /// <value>Người tạo phiếu</value>
    [ForeignKey(nameof(CreatedByUserId))]
    public idm_User CreatedByUser { get; set; }

    [ForeignKey(nameof(LastModifiedByUserId))]
    public idm_User LastModifiedByUser { get; set; }
}