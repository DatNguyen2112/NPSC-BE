using NSPC.Data.Data.Entity.Asset;
using NSPC.Data.Data.Entity.AssetHistory;
using NSPC.Data.Data.Entity.AssetLocation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Data.Entity.AssetAllocation;
public enum AllocationStatus
{
    /// <value>Yêu cầu đang chờ</value>
    Pending,
    /// <value>Yêu cầu đã chấp nhận</value>
    Accepted,
    /// <value>Yêu cầu đã bị từ chối</value>
    Rejected
}

[Table("sm_AssetAllocation")]
public class sm_AssetAllocation : BaseTableService<sm_AssetAllocation>
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

    /// <value>Loại thao tác</value>
    [Required]
    public AssetBusinessOperation Operation { get; set; }

    /// <value>Ngày thực hiện</value>
    [Required]
    public DateTime ExecutionDate { get; set; }

    /// <value>ID vị trí trước</value>
    public Guid? FromLocationId { get; set; }

    /// <summary>
    /// Navigation property to sm_AssetLocation
    /// </summary>
    [ForeignKey(nameof(FromLocationId))]
    public sm_AssetLocation FromLocation { get; set; }

    /// <value>ID vị trí sau</value>
    public Guid? ToLocationId { get; set; }

    /// <summary>
    /// Navigation property to sm_AssetLocation
    /// </summary>
    [ForeignKey(nameof(ToLocationId))]
    public sm_AssetLocation ToLocation { get; set; }

    /// <value>ID người sử dụng trước</value>
    public Guid? FromUserId { get; set; }

    /// <summary>
    /// Navigation property to idm_User
    /// </summary>
    [ForeignKey(nameof(FromUserId))]
    public idm_User FromUser { get; set; }

    /// <value>ID người sử dụng sau</value>
    public Guid? ToUserId { get; set; }

    /// <summary>
    /// Navigation property to idm_User
    /// </summary>
    [ForeignKey(nameof(ToUserId))]
    public idm_User ToUser { get; set; }

    /// <value>Ghi chú</value>
    [MaxLength(255)]
    public string Description { get; set; }

    /// <value>Người tạo</value>
    [ForeignKey(nameof(CreatedByUserId))]
    public idm_User CreatedByUser { get; set; }

    /// <value>Người sửa cuối</value>
    [ForeignKey(nameof(LastModifiedByUserId))]
    public idm_User LastModifiedByUser { get; set; }

    /// <value>Trạng thái khi gửi yêu cầu cấp phát, điều chuyển,thu hồi email</value>
    public AllocationStatus? Status { get; set; }

    /// <value>Lý do từ chối</value>
    [MaxLength(255)]
    public string RejectionReason { get; set; }

    /// <value>Mã code xác nhận gmail</value>
    public string Code { get; set; }
}