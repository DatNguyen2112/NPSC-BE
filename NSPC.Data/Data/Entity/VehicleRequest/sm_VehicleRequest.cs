using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Entity;

namespace NSPC.Data.Data.Entity.VehicleRequest;

/// <summary>
/// Enum for vehicle request status
/// </summary>
public enum VehicleRequestStatus
{
    /// <summary>
    /// Nhập
    /// </summary>
    Draft,

    /// <summary>
    /// Chờ duyệt
    /// </summary>
    PendingApproval,

    /// <summary>
    /// Đã duyệt
    /// </summary>
    Approved,

    /// <summary>
    /// Từ chối
    /// </summary>
    Rejected,

    /// <summary>
    /// Chờ ghép xe
    /// </summary>
    WaitingForSharing,

    /// <summary>
    /// Đã ghép xe
    /// </summary>
    Shared
}

/// <summary>
/// Enum for priority levels
/// </summary>
public enum VehicleRequestPriority
{
    /// <summary>
    /// Thấp
    /// </summary>
    Low,

    /// <summary>
    /// Trung bình
    /// </summary>
    Medium,

    /// <summary>
    /// Cao
    /// </summary>
    High
}

[Table("sm_VehicleRequest")]
public class sm_VehicleRequest : BaseTableService<sm_VehicleRequest>
{
    /// <value>ID duy nhất của yêu cầu (Khóa chính)</value>
    [Key]
    public Guid Id { get; set; }

    /// <value>Mã yêu cầu dạng: VRN-</value>
    [Required]
    public string RequestCode { get; set; }

    /// <value>ID người sử dụng xe</value>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property to idm_User
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public idm_User User { get; set; }

    /// <value>Tên người sử dụng xe</value>
    public string UserName { get; set; }

    /// <value>ID đơn vị sử dụng xe</value>
    [Required]
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Navigation property to sm_CodeType
    /// </summary>
    [ForeignKey(nameof(DepartmentId))]
    public sm_CodeType Department { get; set; }

    /// <value>Tên đơn vị sử dụng xe</value>
    public string DepartmentName { get; set; }

    /// <value>Số điện thoại liên hệ của người yêu cầu</value>
    [MaxLength(20)]
    public string ContactPhone { get; set; }

    /// <value>ID Dự án</value>
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// Navigation property to sm_Construction
    /// </summary>
    [ForeignKey(nameof(ProjectId))]
    public sm_Construction Project { get; set; }

    /// <value>Tên dự án</value>
    public string ProjectName { get; set; }

    /// <value>Mục đích chuyến đi</value>
    [Required, MaxLength(500)]
    public string Purpose { get; set; }

    /// <value>Độ ưu tiên (Cao, Trung bình, Thấp)</value>
    [Required]
    public VehicleRequestPriority Priority { get; set; } = VehicleRequestPriority.Medium;

    /// <value>Số lượng người đi</value>
    [Required]
    public int NumPassengers { get; set; }

    /// <value>Thời gian bắt đầu sử dụng xe</value>
    [Required]
    public DateTime StartDateTime { get; set; }

    /// <value>Thời gian kết thúc sử dụng xe</value>
    [Required]
    public DateTime EndDateTime { get; set; }

    /// <value>Điểm xuất phát</value>
    [Required, MaxLength(255)]
    public string DepartureLocation { get; set; }

    /// <value>Nơi đến</value>
    [Required, MaxLength(255)]
    public string DestinationLocation { get; set; }

    /// <value>ID xe dự kiến yêu cầu</value>
    public Guid? RequestedVehicleId { get; set; }

    /// <summary>
    /// Navigation property to sm_PhuongTien
    /// </summary>
    [ForeignKey(nameof(RequestedVehicleId))]
    public sm_PhuongTien RequestedVehicle { get; set; }

    /// <value>Biển số của xe dự kiến yêu cầu</value>
    [MaxLength(20)]
    public string RequestedVehiclePlateNumber { get; set; }

    /// <value>Ghi chú của người yêu cầu</value>
    [MaxLength(500)]
    public string Notes { get; set; }

    /// <value>Trạng thái yêu cầu (Nhập, Chờ duyệt, Đã duyệt, Từ chối)</value>
    [Required]
    public VehicleRequestStatus Status { get; set; } = VehicleRequestStatus.Draft;

    /// <value>Lý do từ chối</value>
    [MaxLength(500)]
    public string RejectNotes { get; set; }

    /// <value>ID nhóm phiếu xin xe ghép</value>
    public Guid? SharingGroupId { get; set; }

    /// <value>Người tạo</value>
    [ForeignKey(nameof(CreatedByUserId))]
    public idm_User CreatedByUser { get; set; }

    /// <value>Người sửa cuối</value>
    [ForeignKey(nameof(LastModifiedByUserId))]
    public idm_User LastModifiedByUser { get; set; }
}