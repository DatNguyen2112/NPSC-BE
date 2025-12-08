using NSPC.Common;
using NSPC.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface IPushNotificationHandler
    {
        /*Task<Response> PushNotificationByUser(Guid userId, PushMessageModel model);

        Task<Response> PushNotificationByDeviceId(PushMessageModel model);*/
        Task<Response> PushNotification(UserMessageModel model, List<idm_User> listReceiveUser);
    }
}