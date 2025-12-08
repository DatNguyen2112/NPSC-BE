using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services.TaskNotification;
using NSPC.Common;

namespace NSPC.API.V1.Controllers.TaskNotification
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/task-notification")]
    [ApiExplorerSettings(GroupName = "Thông báo công việc dự án")]
    //[Authorize]
    //[AuthorizeByToken]
    public class TaskNotificationController : ControllerBase
    {
        private readonly ITaskNotificationHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public TaskNotificationController(ITaskNotificationHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Lấy chi tiết 1 thông báo công việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<TaskNotificationViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _handler.GetById(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách thông báo công việc
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<TaskNotificationViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<TaskNotificationQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

        [HttpGet, Route("signal")]
        public async Task Signal(CancellationToken cancellationToken)
        {
            try
            {
                Response.ContentType = "text/event-stream";
                Response.Headers.Add("Cache-Control", "no-cache");
                Response.Headers.Add("Connection", "keep-alive");

                var currentUser = Helper.GetRequestInfo(HttpContext.Request);
                var lastNotificationIds = new HashSet<Guid>();

                while (!cancellationToken.IsCancellationRequested)
                {
                    // Lấy danh sách thông báo mới nhất cho user
                    var filter = new TaskNotificationQueryModel
                    {
                        UserId = currentUser.UserId,
                        Page = 1,
                        Size = 10,
                        Sort = "-CreatedOnDate"
                    };
                    var result = await _handler.GetPage(filter);
                    var notifications = result.Data?.Content ?? new List<TaskNotificationViewModel>();

                    // Kiểm tra có thông báo mới không
                    var newNotifications = notifications.Where(n => !lastNotificationIds.Contains(n.Id)).ToList();
                    if (newNotifications.Any())
                    {
                        // Cập nhật danh sách id đã gửi
                        lastNotificationIds = notifications.Select(n => n.Id).ToHashSet();

                        // Chỉ gửi tín hiệu về client (không gửi json)
                        await Response.WriteAsync($"data: signal\n\n");
                        await Response.Body.FlushAsync();
                    }

                    await Task.Delay(2000, cancellationToken); // Kiểm tra mỗi 2 giây
                }
            }
            catch (TaskCanceledException)
            {
                // Nếu client ngắt kết nối, không làm gì cả
                //Console.WriteLine("Client disconnected");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khác (nếu có)
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Đánh dấu một thông báo là đã đọc
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        [HttpPut, Route("mark-as-read/{notificationId}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            var result = await _handler.MarkAsRead(notificationId);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa tất cả thông báo theo userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete, Route("delete-all-by-user/{userId}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAllByUserId(Guid userId)
        {
            var result = await _handler.DeleteAllByUserId(userId);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Đánh dấu tất cả thông báo là đã đọc theo userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut, Route("mark-all-read-user/{userId}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAllAsReadByUserId(Guid userId)
        {
            var result = await _handler.MarkAllAsReadByUserId(userId);
            return Helper.TransformData(result);
        }

        [HttpGet("count-unread")]
        public async Task<ActionResult<int>> CountUnread()
        {
            var currentUser = Helper.GetRequestInfo(HttpContext.Request);
            var result = await _handler.CountUnread(currentUser.UserId);
            return Helper.TransformData(result);
        }

        [HttpPost("submit-notification")]
        public async Task<IActionResult> SubmitFcmToken([FromBody] SubmitFcmTokenModel model)
        {
            var result = await _handler.SubmitFcmToken(model.Token);
            return Helper.TransformData(result);
        }
    }
}
