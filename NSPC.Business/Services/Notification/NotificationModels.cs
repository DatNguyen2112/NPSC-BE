using NSPC.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NSPC.Business
{
    public class NotificationQueryModel : PaginationRequest
    {
        public Guid? ReceiverUserId { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
    }

    public class NotificationViewModel
    {
        public Int64 Id { get; set; }
        public string Title { get; set; }
        public string BodyHtml { get; set; }
        public string BodyPlainText { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsReceiverRead { get; set; }
        public bool IsReceiverSeen { get; set; }
        public DateTime? ReceiverReadOnDate { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public string Type { get; set; }
        public string NavigateUrl { get; set; }
        public string Content { get; set; }
        public Guid? EntityId { get; set; }
    }

    public class UserNotificationInfoModel
    {
        public int TotalUnread { get; set; }
        public int TotalUnseen { get; set; }
        public List<NotificationViewModel> RecentList { get; set; }

        public UserNotificationInfoModel()
        {
            RecentList = new List<NotificationViewModel>();
        }
    }

    public class ProfileApprovedNotificationModel
    {
        public string Owner { get; set; }
        public string OwnerDemandCode { get; set; }
        public Guid? PackageId { get; set; }
        public Guid ProfileId { get; set; }
        public string ProfileType { get; set; }
        public DateTime ApprovedDate { get; set; }
    }

    public class ProposalReceivedNotificationModel
    {
        public Guid ProfileId { get; set; }
        public string ProfileType { get; set; }
        public string ProfileOwner { get; set; }
        public string ProfileOwnerDemandCode { get; set; }
        public Guid ThreadId { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }

    public class MessageReceivedNotificationModel
    {
        public Guid ThreadId { get; set; }
        public string Content { get; set; }
        public string SenderName { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }

    public class GlobalEventNotificationModel
    {
        public Guid? GlobalEventId { get; set; }
        public string TitleEn { get; set; }
        public string TitleVn { get; set; }
        public DateTime? CreatedOnDate { get; set; }
    }

    public class NotificationCreateModel
    {
        public Guid? ReceiverUserId { get; set; }
        public string Type { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public string JsonData { get; set; }
        public string JsonDataType { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
    }

    public class JsonModel
    {
        public Guid? id { get; set; }
        public string statusCode { get; set; }
        public string type { get; set; }
    }
}