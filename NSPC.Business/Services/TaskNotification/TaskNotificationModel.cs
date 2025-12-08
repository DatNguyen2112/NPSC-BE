using NSPC.Business.Services.WorkItem;
using NSPC.Common;
namespace NSPC.Business.Services.TaskNotification
{
    public class TaskNotificationViewModel
    {
        public Guid Id { get; set; }
        public string NotificationStatus { get; set; }
        public string CreatedByUserName { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsRead { get; set; }
        public UserModel idm_User { get; set; }
        public string ApprovalType { get; set; }
        public TaskViewModel Task { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public List<string> AdditionalData { get; set; } = new();
    }
    public class TaskNotificationQueryModel : PaginationRequest
    {
        public Guid UserId { get; set; }
    }
    public class SubmitFcmTokenModel
    {
        public string Token { get; set; }
    }
}
