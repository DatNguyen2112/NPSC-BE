using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface INotificationTemplateHandler
    {
        Task<Response> GetById(Guid id);

        Task<Response> Delete(Guid NotificationTemplateId);

        Task<Response> UpdateNotificationTemplate(NotificationTemplateCreateUpdateModel param);

        Task<Response> GetAdminListPageAsync(NotificationTemplateQueryModel filterObject);
    }
}