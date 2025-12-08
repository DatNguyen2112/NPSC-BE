using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services.TaskUsageHistory;
using NSPC.Common;

namespace NSPC.API.V1.Controllers.TaskUsageHistory
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/task-usage-history")]
    [ApiExplorerSettings(GroupName = "Lịch sử sử dụng công việc dự án")]
    [Authorize]
    [AuthorizeByToken]
    public class TaskUsageHistoryController : ControllerBase
    {
        private readonly ITaskUsageHistoryHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public TaskUsageHistoryController(ITaskUsageHistoryHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Lấy chi tiết 1 lịch sử công việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<TaskUsageHistoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _handler.GetById(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách lịch sử công việc
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<TaskUsageHistoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<TaskUsageHistoryQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

    }
}
