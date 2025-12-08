using NSPC.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleManagement.Data.Data.Entity.TaskHistory;

public enum TaskActivityType
{
    // Người dùng tạo công việc mới
    CreatedTask,                      // "đã tạo công việc"

    // Người dùng chỉnh sửa thông tin công việc
    UpdatedTaskInfo,                 // "đã chỉnh sửa thông tin công việc"

    // Người dùng đã chỉnh sửa thông tin công việc con
    UpdateSubTask,                   // "đã chỉnh sửa thông tin công việc con"

    // Người dùng đã bắt đầu thực hiện công việc
    StartTask,                       // "đã bắt đầu thực hiện công việc"

    // Người dùng xóa công việc con
    DeletedSubtask,                 // "đã xóa công việc con Tác vụ con 3"

    // Người dùng đánh dấu hoàn thành công việc con
    MarkedSubtaskCompleted,         // "đã đánh dấu hoàn thành công việc con Tác vụ con 1"

    // Người dùng bỏ đánh dấu hoàn thành công việc con
    UnmarkedSubtaskCompleted,       // "đã bỏ đánh dấu hoàn thành công việc con Tác vụ con 1"

    // Người dùng thay đổi nhân sự thực hiện công việc con
    ChangedSubtaskAssignee, // "đã thay đổi nhân sự thực hiện công việc con Tác vụ con 1"

    // Người dùng thay đổi người duyệt công việc
    ChangedApprover,                // "đã thay đổi người phê duyệt"

    // Người dùng thay đổi nhân sự thực hiện công việc
    ChangedAssignee,                // "đã thay đổi nhân sự thực hiện công việc"

    // Người dùng gửi duyệt công việc
    SubmittedForApproval,           // "đã gửi duyệt công việc"

    // Người duyệt đánh dấu công việc là Không đạt
    MarkedAsFailed,                 // "đã đánh dấu công việc Không đạt"

    // Người duyệt đánh dấu công việc là Đạt
    MarkedAsPassed,                 // "đã đánh dấu công việc Đạt"

    // Người dùng tải lên file đính kèm
    UploadedAttachment,             // "đã tải lên file đính kèm"

    // Người dùng tải lên file đính kèm cho công việc con
    UploadedSubtaskAttachment,      // "đã tải lên file đính kèm công việc con Tác vụ con 1"
}

[Table("sm_TaskUsageHistory")]
public class sm_TaskUsageHistory : BaseTableService<sm_TaskUsageHistory>
{
    /// <value>ID</value>
    [Key]
    public Guid Id { get; set; }

    /// <value>ID công việc</value>
    public Guid? TaskId { get; set; }

    /// <summary>
    /// Navigation property to sm_Task
    /// </summary>
    [ForeignKey(nameof(TaskId))]
    public sm_Task Task { get; set; }

    /// <value>Nghiệp vụ</value>
    [Required]
    public TaskActivityType ActivityType { get; set; }

    public string NameSubtask { get; set; }

    /// <value>Ghi chú</value>
    public string Description { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public idm_User CreatedByUser { get; set; }

}