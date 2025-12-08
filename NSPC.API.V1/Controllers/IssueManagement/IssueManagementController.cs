using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business;
using NSPC.Business.Services;
using NSPC.Business.Services.Contract;
using NSPC.Business.Services.EInvoice;
using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.API.V1.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Route("api/v{api-version:apiVersion}/issue-management")]
    [ApiExplorerSettings(GroupName = "Quản lý vướng mắc")]
    public class IssueManagementController : ControllerBase
    {
        private readonly IIssueManagementHandler _handler;

        public IssueManagementController(IIssueManagementHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Thêm mới vướng mắc
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [RightValidate("CONSTRUCTION", "ADDISSUE")]
        [ProducesResponseType(typeof(Response<IssueManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] IssueManagementCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);

            var result = await _handler.Create(model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách vướng mắc
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<IssueManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPageAsync([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<IssueManagementQuery>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xem chi tiết vướng mắc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<IssueManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.GetById(id, currentUser);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy số lượng vướng mắc theo trạng thái
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("countIssueByStatus")]
        [ProducesResponseType(typeof(Response<IssueManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIssueByStatus(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.CountByStatus(currentUser);

            return Helper.TransformData(result);
        }


        /// <summary>
        /// Xem lịch sử vướng mắc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("activity-log")]
        [ProducesResponseType(typeof(ResponsePagination<IssueActivityLogViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivityLog(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.GetActivityLogById(id, currentUser);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Chỉnh sửa vướng mắc
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        [RightValidate("ISSUE", "UPDATEISSUE")]
        [ProducesResponseType(typeof(Response<IssueManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] IssueManagementCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);

            var result = await _handler.Update(id, model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Huỷ vướng mắc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("deactivate-issue")]
        [RightValidate("ISSUE", "PROCESSISSUE")]
        [ProducesResponseType(typeof(Response<IssueManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeactiveIssueAsync(Guid id, string reasonCancel)
        {
            var result = await _handler.DeactiveIssueAsync(id,reasonCancel);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Mở lại vướng mắc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("reopen-issue")]
        [RightValidate("ISSUE", "REOPENISSUE")]
        [ProducesResponseType(typeof(Response<IssueManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReopenIssueAsync(Guid id, string reasonOpen)
        {
            var result = await _handler.ReopenIssueAsync(id, reasonOpen);

            return Helper.TransformData(result);
        }
        /// <summary>
        /// Xử lý vướng mắc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("resolve-issue")]
        [RightValidate("ISSUE", "PROCESSISSUE")]
        [ProducesResponseType(typeof(Response<IssueManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResolveIssue(Guid id, ResolveModel model)
        {
            var result = await _handler.ResolveIssueAsync(id, model);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xoá vướng mắc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [RightValidate("ISSUE", "DELETEISSUE")]
        [ProducesResponseType(typeof(Response<IssueManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteIssueAsync(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.Delete(id, currentUser);

            return Helper.TransformData(result);
        }
    }
}
