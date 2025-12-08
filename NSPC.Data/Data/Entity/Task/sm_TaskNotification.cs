using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum NotificationStatus
{
    Mentioned, // Nhắc đến bạn
    Overdue, // Công việc quá hạn (dấu chấm than đỏ)
    Due, // Công việc đến hạn (vào ngày hạn chót)
    Joined, // Tham gia công việc (mũi tên cong xanh)
    Left, // Rời khỏi công việc (mũi tên cong đỏ)
    PendingApproval, // Gửi duyệt (vòng tròn hồng)
    WarningSoonExpire, // Cảnh báo sắp hết hạn (dấu chấm than vàng, vào ngày hạn chót - 3)
    StatusInProgress, // Công việc chuyển sang trạng thái Đang thực hiện
    StatusPassed, // Đánh dấu Đạt
    StatusFailed, // Đánh dấu Không đạt
    NextTaskCompletion, // Thông báo hoàn thành công việc tiếp theo
    VehicleRequestSendApproval,
    VehicleRequestApprove,
    VehicleRequestReject,
    VehicleRequestWaitingForSharing,
    VehicleRequestShared,
    CreateIssue, // Tạo vướng mắc
    CancelIssue, // Huỷ vướng mắc
    ResolveIssue, // Xử lý vướng mắc
    ReOpenIssue, // Mở lại vướng mắc
}

namespace NSPC.Data
{
    /// <value>Bảng công việc con</value>
    [Table("sm_TaskNotification")]
    public class sm_TaskNotification : BaseTableService<sm_TaskNotification>
    {
        [Key]
        public Guid Id { get; set; }


        /// <value>Loại thông báo (ví dụ: cảnh báo, nhắc nhở, thành công, v.v.)</value>
        [MaxLength(50)]
        public NotificationStatus NotificationStatus { get; set; }

        /// <value>Trạng thái xem</value>
        public bool IsRead { get; set; } = false;

        /// <value>Người nhận thông báo</value>
        public Guid? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual idm_User idm_User { get; set; }


        /// <value>Loại người phê duyệt</value>
        public string? ApprovalType { get; set; }

        /// <value>Ảnh người tạo ra thông báo</value>
        public string? AvatarUrl { get; set; }

        /// <value>ID công việc</value>
        public Guid? TaskId { get; set; }

        [ForeignKey(nameof(TaskId))]
        public sm_Task Task { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual idm_User CreatedByUser { get; set; }

        [Column(TypeName = "jsonb")]
        public List<string> AdditionalData { get; set; } = new();
    }
}