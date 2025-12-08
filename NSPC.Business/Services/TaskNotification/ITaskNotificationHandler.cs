using NSPC.Common;

namespace NSPC.Business.Services.TaskNotification
{
    public interface ITaskNotificationHandler
    {
        Task<Response<TaskNotificationViewModel>> GetById(Guid id);
        Task<Response<Pagination<TaskNotificationViewModel>>> GetPage(TaskNotificationQueryModel query);
        Task<Response> MarkAsRead(Guid notificationId);
        Task<Response> DeleteAllByUserId(Guid userId);
        Task<Response> MarkAllAsReadByUserId(Guid userId);
        Task<Response<int>> CountUnread(Guid userId);
        Task<Response> SubmitFcmToken(string token);
        Task<Response> CreatePushNotification(TaskNotificationViewModel notification);
    }
}
