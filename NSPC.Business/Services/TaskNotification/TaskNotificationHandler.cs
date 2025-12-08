using AutoMapper;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
namespace NSPC.Business.Services.TaskNotification
{
    public class TaskNotificationHandler : ITaskNotificationHandler
    {
        public static string RenderNotificationContent(TaskNotificationViewModel notification)
        {
            switch (notification.NotificationStatus)
            {
                case "Mentioned":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã nhắc đến bạn trong bình luận của công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span>";
                case "Joined":
                    if (notification.ApprovalType == "Approver")
                        return $"Bạn đã tham gia vào công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span> với vai trò Người phê duyệt";
                    else
                        return $"Bạn đã tham gia vào công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span> với vai trò Nhân sự thực hiện";
                case "Left":
                    if (notification.ApprovalType == "Approver")
                        return $"Bạn đã ra khỏi công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span> với vai trò Người phê duyệt";
                    else
                        return $"Bạn đã ra khỏi công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span> với vai trò Nhân sự thực hiện";
                case "WarningSoonExpire":
                    return $"Công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span> sẽ hết hạn vào ngày <span style=\"font-weight:600\">{notification?.Task?.EndDateTime?.ToString("dd/MM/yyyy")}</span>";
                case "Due":
                    return $"Công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span> đã đến hạn hoàn thành";
                case "Overdue":
                    return $"Công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span> đã quá hạn hoàn thành";
                case "StatusInProgress":
                    return $"Công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span> đã chuyển sang trạng thái Đang thực hiện";
                case "StatusFailed":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã đánh dấu <span style=\"color:#FF4D4F\">Không đạt</span> cho công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span>";
                case "StatusPassed":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã đánh dấu <span style=\"color:#52C41A\">Đạt</span> cho công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span>";
                case "PendingApproval":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã gửi duyệt công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span>";
                case "NextTaskCompletion":
                    return $"Công việc <span style=\"font-weight:600\">{notification.AdditionalData[0]}</span> đã hoàn thành. Vui lòng thực hiện công việc <span style=\"font-weight:600\">{notification.Task?.Name}</span>";
                case "VehicleRequestSendApproval":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã gửi duyệt yêu cầu xin xe đến <span style=\"font-weight:600\">{notification.AdditionalData[0]}</span>";
                case "VehicleRequestWaitingForSharing":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã gửi duyệt yêu cầu ghép xe đến <span style=\"font-weight:600\">{notification.AdditionalData[0]}</span>";
                case "VehicleRequestApprove":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã <span style=\"color: green\">phê duyệt</span> yêu cầu xin xe đến <span style=\"font-weight:600\">{notification.AdditionalData[0]}</span>";
                case "VehicleRequestShared":
                    var requestCodes = notification.AdditionalData.Skip(2).ToList();
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã xác nhận ghép xe với yêu cầu <span style=\"font-weight:600\">{string.Join(", ", requestCodes)}</span>";
                case "VehicleRequestReject":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã <span style=\"color: red\">từ chối</span> yêu cầu xin xe đến <span style=\"font-weight:600\">{notification.AdditionalData[0]}</span>";
                case "CreateIssue":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã tạo vướng mắc dự án <span style=\"font-weight:600\">{notification.AdditionalData[1]}</span>";
                case "CancelIssue":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã <span style=\"color: red\">huỷ</span> vướng mắc dự án <span style=\"font-weight:600\">{notification.AdditionalData[1]}</span>";
                case "ResolveIssue":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã <span style=\"color: green\">xử lý & đóng</span> vướng mắc dự án <span style=\"font-weight:600\">{notification.AdditionalData[1]}</span>";
                case "ReOpenIssue":
                    return $"<span style=\"font-weight:600\">{notification.CreatedByUserName}</span> đã mở lại vướng mắc dự án <span style=\"font-weight:600\">{notification.AdditionalData[1]}</span>";
                default:
                    return string.Empty;
            }
        }

        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        private readonly IMemoryCache _memoryCache;

        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false
                }
            }
        };
        public TaskNotificationHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<Response<TaskNotificationViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_TaskNotification
                                    .AsNoTracking()
                                    .Include(x => x.Task)
                                    .Include(x => x.idm_User)
                                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<TaskNotificationViewModel>("Lịch sử công việc không tồn tại trong hệ thống.");

                var result = _mapper.Map<TaskNotificationViewModel>(entity);
                return new Response<TaskNotificationViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<TaskNotificationViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<TaskNotificationViewModel>>> GetPage(TaskNotificationQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_TaskNotification
                    .AsNoTracking()
                    .Include(x => x.Task)
                    .Include(x => x.idm_User)
                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<TaskNotificationViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<TaskNotificationViewModel>>(ex);
            }
        }
        private Expression<Func<sm_TaskNotification, bool>> BuildQuery(TaskNotificationQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_TaskNotification>(true);
            //if (!string.IsNullOrEmpty(query.FullTextSearch))
            //    predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.UserId != Guid.Empty)
                predicate.And(s => s.UserId == query.UserId);

            return predicate;
        }
        public async Task<Response> MarkAsRead(Guid notificationId)
        {
            try
            {
                var entity = await _dbContext.sm_TaskNotification
                    .FirstOrDefaultAsync(x => x.Id == notificationId);

                if (entity == null)
                    return Helper.CreateNotFoundResponse<bool>("Thông báo không tòn tại.");

                if (!entity.IsRead)
                {
                    entity.IsRead = true;
                    _dbContext.sm_TaskNotification.Update(entity);
                    await _dbContext.SaveChangesAsync();
                }

                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: NotificationId: {@notificationId}", notificationId);
                return Helper.CreateExceptionResponse<bool>(ex);
            }
        }

        public async Task<Response> DeleteAllByUserId(Guid userId)
        {
            try
            {
                var notifications = await _dbContext.sm_TaskNotification
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                if (notifications == null || notifications.Count == 0)
                    return Helper.CreateNotFoundResponse<bool>("Không tìm thấy thông báo nào để xóa.");

                _dbContext.sm_TaskNotification.RemoveRange(notifications);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse("Đã xóa tất cả thông báo thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}", userId);
                return Helper.CreateExceptionResponse<bool>(ex);
            }
        }
        public async Task<Response> MarkAllAsReadByUserId(Guid userId)
        {
            try
            {
                var notifications = await _dbContext.sm_TaskNotification
                    .Where(x => x.UserId == userId && !x.IsRead)
                    .ToListAsync();

                if (notifications == null || notifications.Count == 0)
                    return Helper.CreateNotFoundResponse<bool>("Không có thông báo chưa đọc nào để cập nhật.");

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }

                _dbContext.sm_TaskNotification.UpdateRange(notifications);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse("Đã đánh dấu tất cả thông báo là đã đọc.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}", userId);
                return Helper.CreateExceptionResponse<bool>(ex);
            }
        }

        public async Task<Response<int>> CountUnread(Guid userId)
        {
            var count = await _dbContext.sm_TaskNotification
                .Where(x => x.UserId == userId && !x.IsRead)
                .CountAsync();

            return Helper.CreateSuccessResponse(count);
        }

        public async Task<Response> CreatePushNotification(TaskNotificationViewModel notification)
        {
            if (notification.UserId == Guid.Empty)
                return Helper.CreateBadRequestResponse("UserId không hợp lệ.");

            var key = $"{notification.UserId}-fcm";
            var tokensMap = new Dictionary<string, string>();
            var p = _memoryCache.Get<HashSet<string>>(key);
            if (p != null)
            {
                foreach (var token in p)
                {
                    tokensMap.Add(token, key);
                }
            }

            var extrasData = new Dictionary<string, string>
            {
                { "content", notification.Content },
                { "id", notification.Id.ToString() },
                //{ "orderId", notification.OrderId?.ToString() ?? "" },
            };

            var messages = tokensMap.Keys
                .Select(token => new Message
                {
                    Data = extrasData,
                    Notification = new Notification
                    {
                        //Title = notification.NotificationStatus,
                        Body = Regex.Replace(notification.Content, "<.*?>", string.Empty)
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps
                        {
                            Badge = 1,
                            Sound = "default",
                            AlertString = Regex.Replace(notification.Content, "<.*?>", string.Empty)
                        },
                    },
                    Android = new AndroidConfig
                    {
                        Notification = new AndroidNotification
                        {
                            Sound = "default",
                            //Title = notification.NotificationStatus,
                            Body = Regex.Replace(notification.Content, "<.*?>", string.Empty)
                        }
                    },
                    Token = token
                })
                .ToList();

            if (!messages.Any())
            {
                return Helper.CreateSuccessResponse();
            }

            var filepath = Directory.GetCurrentDirectory() + "/Resources/FCM/Uberental.json";

            if (!File.Exists(filepath))
            {
                Log.Error(filepath + "is not exist");
                return Helper.CreateExceptionResponse("Credential file not exist");
            }

            var firebaseApp = FirebaseApp.GetInstance("xntv-npsc") ?? FirebaseApp.Create(new AppOptions()
            {
                Credential =
                    GoogleCredential.FromFile(filepath)
            }, "xntv-npsc");
            var batchResponse = await FirebaseMessaging.GetMessaging(firebaseApp).SendEachAsync(messages);

            // Xoá những token bị revoke (response error là NotFound)
            for (var i = 0; i < batchResponse.Responses.Count; i++)
            {
                var response = batchResponse.Responses[i];

                if (response.IsSuccess) continue;

                if (response.Exception.ErrorCode == ErrorCode.NotFound)
                {
                    var invalidToken = messages[i].Token;
                    var cacheKey = tokensMap[invalidToken];
                    if (_memoryCache.TryGetValue<HashSet<string>>(cacheKey, out var tokenList))
                    {
                        tokenList?.Remove(invalidToken);
                        if (tokenList is { Count: 0 })
                        {
                            _memoryCache.Remove(cacheKey);
                        }
                        else
                        {
                            _memoryCache.Set(cacheKey, tokenList);
                        }
                    }
                }
                else
                {
                    Log.Error(response.Exception, "");
                }
            }

            return Helper.CreateSuccessResponse();
        }

        public async Task<Response> SubmitFcmToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Helper.CreateBadRequestResponse("Token không được trống");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var key = $"{currentUser.UserId}-fcm";
            if (!_memoryCache.TryGetValue<HashSet<string>>(key, out var tokenList))
            {
                tokenList = new HashSet<string>();
            }

            tokenList.Add(token.Trim());
            //var cacheEntryOptions = new MemoryCacheEntryOptions
            //{
            //    AbsoluteExpiration = currentUser.ExpireAt
            //};
            //_memoryCache.Set(key, tokenList, cacheEntryOptions);
            _memoryCache.Set(key, tokenList);
            return Helper.CreateSuccessResponse();
        }
    }
}
