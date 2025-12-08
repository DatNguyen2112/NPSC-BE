using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface INotificationHandler
    {
        Task<Response<Pagination<NotificationViewModel>>> GetListPageAsync(NotificationQueryModel filterObject);

        Task<Response> MarkAsRead(Guid userId, int notificationId);

        Task<Response<UserNotificationInfoModel>> GetUserNotification(Guid userId, string language);
        Task<Response<UserNotificationInfoModel>> GetUserNewGlobalEventNotification(Guid userId, string language);

        Task<Response> CreateNotification(NotificationCreateModel model);
    }
}