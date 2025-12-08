using AutoMapper;
using NSPC.Common;
using NSPC.Data;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using NSPC.Data.Data;


namespace NSPC.Business
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPushNotificationHandler _pushNotificationHandler;

        public NotificationHandler(IConfiguration configuration,
            SMDbContext dbContext, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor,
            IPushNotificationHandler pushNotificationHandler)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _pushNotificationHandler = pushNotificationHandler;
        }

        public async Task<Response<Pagination<NotificationViewModel>>> GetListPageAsync(NotificationQueryModel filter)
        {
            try
            {
                var result = new Pagination<NotificationViewModel>();

                var predicate = BuildQuery(filter);

                var query = _dbContext.sm_Notification.Where(predicate);

                var data = await query.GetPageAsync(filter);

                result = _mapper.Map<Pagination<NotificationViewModel>>(data);

                return new Response<Pagination<NotificationViewModel>>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query {@params}", filter);
                return Helper.CreateExceptionResponse<Pagination<NotificationViewModel>>(ex);
            }
        }

        public async Task<Response<UserNotificationInfoModel>> GetUserNotification(Guid userId, string language)
        {
            try
            {
                var notifications = _dbContext.sm_Notification.Where(x => x.ReceiverUserId == userId);
                var result = new UserNotificationInfoModel();

                result.TotalUnread = notifications.Count(x => x.IsReceiverRead == false);
                result.TotalUnseen = notifications.Count(x => x.IsReceiverSeen == false);

                var data = await notifications.OrderByDescending(x => x.CreatedOnDate).Take(10).ToListAsync();

                if (data.Count() > 0)
                {
                    result.RecentList = _mapper.Map<List<NotificationViewModel>>(data);
                }
                return new Response<UserNotificationInfoModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new Response<UserNotificationInfoModel>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<Response<UserNotificationInfoModel>> GetUserNewGlobalEventNotification(Guid userId, string language)
        {
            try
            {
                var notifications = _dbContext.sm_Notification.Where(x => x.ReceiverUserId == userId && x.Type.Equals(NotificationTypeConstants.New_GlobalEvent));
                var result = new UserNotificationInfoModel();

                result.TotalUnread = notifications.Count(x => x.IsReceiverRead == false);
                result.TotalUnseen = notifications.Count(x => x.IsReceiverSeen == false);

                var data = await notifications.OrderByDescending(x => x.CreatedOnDate).Take(10).ToListAsync();

                if (data.Count() > 0)
                {
                    result.RecentList = data.Select(x => FillContent(x, language)).ToList();
                }
                return new Response<UserNotificationInfoModel>(result);

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new Response<UserNotificationInfoModel>(HttpStatusCode.InternalServerError, null, ex.Message);
            }
        }

        public async Task<Response> MarkAsRead(Guid userId, int notificationId)
        {
            try
            {
                var notification = await _dbContext.sm_Notification.Where(x => x.Id == notificationId && x.ReceiverUserId == userId).FirstOrDefaultAsync();
                
                if (notification != null)
                {
                    notification.IsReceiverRead = true;
                    notification.IsReceiverSeen = true;
                    notification.ReceiverReadOnDate = DateTime.Now;
                    await _dbContext.SaveChangesAsync();
                    return new Response();
                }
                else
                {
                    return Helper.CreateForbiddenResponse();
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new Response(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private NotificationViewModel FillContent(sm_Notification notification, string language)
        {
            var template = NotificationTemplateCollection.Instance.FetchNotificationTemplate(notification.Type, language);

            var data = JsonConvert.DeserializeObject<ProfileApprovedNotificationModel>(notification.JsonData);
            // Fetch body
            switch (notification.Type)
            {
                case NotificationTypeConstants.Profile_Approved:
                    if (data != null)
                    {
                        var notificationModel = _mapper.Map<sm_Notification, NotificationViewModel>(notification);
                        notificationModel.Title = template.TitleTemplate;
                        var ownerDemand = CodeTypeCollection.Instance.FetchCode(data.OwnerDemandCode, language, notification.TenantId);

                        if (ownerDemand != null)
                        {
                            notificationModel.BodyHtml = string.Format(template.BodyHtmlTemplate, ownerDemand?.Title + " - " + data.Owner, data.ApprovedDate);
                            notificationModel.BodyPlainText = string.Format(template.BodyPlainTextTemplate, ownerDemand?.Title + " - " + data.Owner, data.ApprovedDate);
                        }    
                        else
                        {
                            notificationModel.BodyHtml = string.Format(template.BodyHtmlTemplate, Utils.GetTranslation("profile_empty_demand", language) + " - " + data.Owner, data.ApprovedDate);
                            notificationModel.BodyPlainText = string.Format(template.BodyPlainTextTemplate, Utils.GetTranslation("profile_empty_demand", language) + " - " + data.Owner, data.ApprovedDate);
                        }

                        return notificationModel;
                    }
                    break;
                case NotificationTypeConstants.Profile_Rejected:
                    if (data != null)
                    {
                        var notificationModel = _mapper.Map<sm_Notification, NotificationViewModel>(notification);
                        notificationModel.Title = template.TitleTemplate;
                        var ownerDemand = CodeTypeCollection.Instance.FetchCode(data.OwnerDemandCode, language, notification.TenantId);

                        if (ownerDemand != null)
                        {
                            notificationModel.BodyHtml = string.Format(template.BodyHtmlTemplate, ownerDemand?.Title + " - " + data.Owner, data.ApprovedDate);
                            notificationModel.BodyPlainText = string.Format(template.BodyPlainTextTemplate, ownerDemand?.Title + " - " + data.Owner, data.ApprovedDate);
                        }
                        else
                        {
                            notificationModel.BodyHtml = string.Format(template.BodyHtmlTemplate, Utils.GetTranslation("profile_empty_demand", language) + " - " + data.Owner, data.ApprovedDate);
                            notificationModel.BodyPlainText = string.Format(template.BodyPlainTextTemplate, Utils.GetTranslation("profile_empty_demand", language) + " - " + data.Owner, data.ApprovedDate);
                        }

                        return notificationModel;
                    }
                    break;
                case NotificationTypeConstants.Proposal_Received:
                    if (data != null)
                    {
                        var notificationModel = _mapper.Map<sm_Notification, NotificationViewModel>(notification);
                        notificationModel.Title = template.TitleTemplate;
                        var ownerDemand = CodeTypeCollection.Instance.FetchCode(data.OwnerDemandCode, language, notification.TenantId);

                        if (ownerDemand != null)
                        {
                            notificationModel.BodyHtml = string.Format(template.BodyHtmlTemplate, ownerDemand?.Title + " - " + data.Owner, data.ApprovedDate);
                            notificationModel.BodyPlainText = string.Format(template.BodyPlainTextTemplate, ownerDemand?.Title + " - " + data.Owner, data.ApprovedDate);
                        }
                        else
                        {
                            notificationModel.BodyHtml = string.Format(template.BodyHtmlTemplate, Utils.GetTranslation("profile_empty_demand", language) + " - " + data.Owner, data.ApprovedDate);
                            notificationModel.BodyPlainText = string.Format(template.BodyPlainTextTemplate, Utils.GetTranslation("profile_empty_demand", language) + " - " + data.Owner, data.ApprovedDate);
                        }

                       

                        return notificationModel;
                    }
                    break;
                case NotificationTypeConstants.New_GlobalEvent:
                    var eventData = JsonConvert.DeserializeObject<GlobalEventNotificationModel>(notification.JsonData);
                    if (eventData != null)
                    {
                        var notificationModel = _mapper.Map<sm_Notification, NotificationViewModel>(notification);
                        notificationModel.Title = template.TitleTemplate;

                        if (language == "vn")
                        {
                            notificationModel.BodyHtml = string.Format(template.BodyHtmlTemplate, eventData.TitleVn, eventData.CreatedOnDate);
                            notificationModel.BodyPlainText = string.Format(template.BodyPlainTextTemplate, eventData.TitleVn, eventData.CreatedOnDate);
                        }
                        else
                        {
                            notificationModel.BodyHtml = string.Format(template.BodyHtmlTemplate, eventData.TitleEn, eventData.CreatedOnDate);
                            notificationModel.BodyPlainText = string.Format(template.BodyPlainTextTemplate, eventData.TitleEn, eventData.CreatedOnDate);
                        }
                        
                        notificationModel.NavigateUrl = "/events/agendas/" + eventData.GlobalEventId;

                        return notificationModel;
                    }
                    break;
            }

            return null;
        }
        private Expression<Func<sm_Notification, bool>> BuildQuery(NotificationQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_Notification>(true);

            if (query.ReceiverUserId.HasValue)
            {
                predicate.And(x => x.ReceiverUserId == query.ReceiverUserId.Value);
            }

            return predicate;
        }

        public async Task<Response> CreateNotification(NotificationCreateModel model)
        {
            try
            {
                var entity = new sm_Notification()
                {
                    ReceiverUserId = model.ReceiverUserId,
                    Type = model.Type,
                    CreatedByUserId = model.CreatedByUserId,
                    JsonData = model.JsonData,
                    JsonDataType = model.JsonDataType,
                    Content = model.Content,
                    Title = model.Title,
                    CreatedOnDate = DateTime.Now
                    
                };
                _dbContext.Add(entity);
                await _dbContext.SaveChangesAsync();
                var usermessage = new UserMessageModel()
                {
                    UserId = entity.ReceiverUserId.Value,
                    Title = entity.Title,
                    Content = entity.Content,
                    CreatedOnDate = entity.CreatedOnDate,
                    Payload = model.JsonData
                };
                var listUser = _dbContext.IdmUser.Where(x => x.Id == entity.ReceiverUserId).ToList();
                await _pushNotificationHandler.PushNotification(usermessage, listUser);
                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse(ex);
            }
        }
    }
}