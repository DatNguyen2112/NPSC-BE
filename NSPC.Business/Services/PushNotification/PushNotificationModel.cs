using System;
using System.Collections.Generic;

namespace NSPC.Business
{
    #region notification
    public enum NotificationType
    {
        /// <summary>
        /// Thông báo thay đổi trạng thái hóa đơn
        /// </summary>
        InvoiceStatusChange,
        /// <summary>
        /// Thông báo thay đổi trạng thái người dùng
        /// </summary>
        UserStatusChange,
        /// <summary>
        /// Thông báo thay đổi trạng thái phát
        /// </summary>
        DeliveryStatusChange,
    }

    public enum PushNotificationStatus
    {
        /// <summary>
        /// Gửi thông báo thất bại
        /// </summary>
        Fail,
        /// <summary>
        /// Gửi thông báo thành công
        /// </summary>
        Success
    }

    public enum CollectResultStatus
    {
        /// <summary>
        /// Dữ liệu được bưu điện Hà Nội tiếp nhận theo ca
        /// </summary>       
        Received = 4,
        /// <summary>
        /// Dữ liệu đã được bưu cục tiếp nhận, in và giao cho bưu tá đi thu gom
        /// </summary>
        Operating = 6,
        /// <summary>
        /// Dữ liệu được bưu điện Hà Nội tiếp nhận và chuyển xuống bưu cục trực thuộc
        /// </summary>      
        Forward = 10,
        /// <summary>
        /// Đến lấy nhưng chưa có hàng lần 1
        /// </summary>
        EmptyStockOneTime = 13,
        /// <summary>
        /// Đến lấy nhưng chưa có hàng lần 2
        /// </summary>
        EmptyStockTwoTimes = 14,

        //Đến lấy nhưng chưa có hàng lần 3
        EmptyStockThreeTimes = 15,
        /// <summary>
        /// Đến lấy nhưng chưa có hàng trên 3 lần
        /// </summary>
        EmptyStockMoreThanThreeTimes = 16,
        /// <summary>
        /// Đơn hàng được các bưu cục chuyển trả bdhn để điều tin thu gom sang bưu cục khác
        /// </summary>
        WrongWay = 12,
        /// <summary>
        /// Đơn hàng hủy không thu gom nữa
        /// </summary>
        Cancel = 8,
        /// <summary>
        /// Đơn hàng đã được thu gom thành công và phát hành lên mạng lưới chuyển phát
        /// </summary>
        Lock = 1,
        /// <summary>
        /// Gửi yêu cầu hủy thu gom
        /// </summary>
        RequestCancel = 21
    }

    public class Config
    {
        public const int STATUS_INPROCESS = 2;
        public const int STATUS_FINISH = 1;
        public const int STATUS_WAIT = 0;
        public const int STATUS_FAIL = 9;

        //Xoa trang thai ban tin o Schedule
        public const int STATUS_DELETE = 99;

        public const string CODE_DONE = "00";
        public const string CODE_UPDATE = "01";
        public const string CODE_ERR = "99";

        public const string COMM_TYPE_OTT = "OTT";
        public const string COMM_TYPE_EMAIL = "EMAIL";
        public const string COMM_TYPE_SMS = "SMS";

        public const int SIZE_TOP_NEWS = 10;

        public const string HO_BR_CODE = "110000";
        public const string ChannelID = "NET";

        public const int INTERVAL_IMMEDIATE_TIME_IN_SECONDS = 30;
        public const string INTERVAL_IMMEDIATE_TIME_IN_SECONDS_KEY = "INTERVAL_IMMEDIATE_TIME_IN_SECONDS_KEY";
        public const int INTERVAL_SCHEDULE_TIME_IN_SECONDS = 30;
        public const string INTERVAL_SCHEDULE_TIME_IN_SECONDS_KEY = "INTERVAL_SCHEDULE_TIME_IN_SECONDS_KEY";

        public const string MAIL_SERVER_IP = "MAIL_SERVER_IP";
        public const string MAIL_SERVER_USER = "MAIL_SERVER_USER";
        public const string MAIL_SERVER_PASS = "MAIL_SERVER_PASS";
        public const string MAIL_SERVER_FROM = "MAIL_SERVER_FROM";
        public const string MAIL_SERVER_DOMAIN = "MAIL_SERVER_DOMAIN";
        public const string MAIL_SERVER_PORT = "MAIL_SERVER_PORT";
        public const string SMS_PARTNER = "SMS_PARTNER";
        public const string SHARED_KEY = "SHARED_KEY";
    }
    #endregion
    public class UserMessageBaseModel
    {
        public Guid Id { get; set; }
    }

    public class UserMessageModel : UserMessageBaseModel
    {
        public Guid UserId { get; set; }
        public Guid? CampaignId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public NotificationType NotificationType { get; set; }
        public string Payload { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
    }


}