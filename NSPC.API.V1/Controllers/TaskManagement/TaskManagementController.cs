using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business;
using NSPC.Common;

namespace NSPC.API.V1
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Route("api/v{api-version:apiVersion}/task-management")]
    [ApiExplorerSettings(GroupName = "Quản lý công việc")]
    
    public class TaskManagementController : ControllerBase
    {
        private readonly ITaskManagementHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public TaskManagementController(ITaskManagementHandler handler)
        {
            _handler = handler;    
        }

        #region Task Management

        /// <summary>
        /// Danh sách công việc
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(Response<TaskManagementViewModel>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetPageTask([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var filterObject = JsonConvert.DeserializeObject<TaskManagementQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPageTask(filterObject, currentUser);
            return Helper.TransformData(result);
        }


        /// <summary>
        /// Thống kê số lượng công việc theo tiêu chí 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("summary")]
        [ProducesResponseType(typeof(Response<TaskSummanryModel>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetSummaryTask([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var filterObject = JsonConvert.DeserializeObject<TaskManagementQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetSummaryTask(filterObject, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Tạo công việc
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Response<TaskManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] TaskManagementCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.CreateTask(model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Tạo nhiều công việc
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("multiple")]
        [Authorize]
        [ProducesResponseType(typeof(Response<List<TaskManagementViewModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateMultiTask([FromBody] List<TaskManagementCreateUpdateModel> model)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.CreateMultiTask(model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật công việc
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(Response<TaskManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskManagementCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);

            var result = await _handler.UpdateTask(id, model, currentUser);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết 1 công việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<TaskManagementViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdTask(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.GetByIdTask(id, currentUser);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa công việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.DeleteTask(id, currentUser);

            return Helper.TransformData(result);
        }

        #endregion

        #region Task Management Assignee

        /// <summary>
        /// Danh sách giao việc
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("assignee")]
        [ProducesResponseType(typeof(Response<TaskManagementAssigneeViewModel>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetPageTaskAssignee([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var filterObject = JsonConvert.DeserializeObject<TaskManagementAssigneeQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPageTaskAssignee(filterObject, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Tạo giao việc
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("assignee")]
        [Authorize]
        [ProducesResponseType(typeof(Response<TaskManagementAssigneeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTaskAssignee([FromBody] TaskManagementAssigneeCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.CreateTaskAssignee(model, currentUser);
            return Helper.TransformData(result);
        }

        ///// <summary>
        ///// Cập nhật giao việc
        ///// </summary>
        ///// <param name="model"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpPut, Route("assignee/{id}")]
        //[ProducesResponseType(typeof(Response<TaskManagementAssigneeViewModel>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> UpdateTaskAssignee(Guid id, [FromBody] TaskManagementAssigneeCreateUpdateModel model)
        //{
        //    var currentUser = Helper.GetRequestInfo(Request);

        //    var result = await _handler.UpdateTaskAssignee(id, model, currentUser);

        //    return Helper.TransformData(result);
        //}

        /// <summary>
        /// Lấy chi tiết 1 giao việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("assignee/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<TaskManagementAssigneeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.GetByIdTaskAssignee(id, currentUser);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa 1 giao việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("assignee/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTaskAssignee(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.DeleteTaskAssignee(id, currentUser);

            return Helper.TransformData(result);
        }
        #endregion

        #region Task Management Comment

        /// <summary>
        /// Danh sách bình luận công việc
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("comment")]
        [ProducesResponseType(typeof(Response<TaskManagementCommentViewModel>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetPageTaskComment([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var filterObject = JsonConvert.DeserializeObject<TaskManagementCommentQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPageTaskComment(filterObject, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Tạo bình luận công việc
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("comment")]
        [Authorize]
        [ProducesResponseType(typeof(Response<TaskManagementCommentViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTaskComment([FromBody] TaskManagementCommentCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.CreateTaskComment(model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật bình luận công việc
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("comment/{id}")]
        [ProducesResponseType(typeof(Response<TaskManagementCommentViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTaskComment(Guid id, [FromBody] TaskManagementCommentCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);

            var result = await _handler.UpdateTaskComment(id, model, currentUser);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết 1 bình luận công việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("comment/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<TaskManagementCommentViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdTaskComment(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.GetByIdTaskComment(id, currentUser);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa 1 bình luận công việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("comment/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTaskComment(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.DeleteTaskComment(id, currentUser);

            return Helper.TransformData(result);
        }
        #endregion

        #region Task Management History

        /// <summary>
        /// Danh sách lịch sử công việc
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("history")]
        [ProducesResponseType(typeof(Response<TaskManagementHistoryViewModel>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetPageTaskHistory([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var filterObject = JsonConvert.DeserializeObject<TaskManagementHistoryQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPageTaskHistory(filterObject, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Tạo lịch sử công việc
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("history")]
        [Authorize]
        [ProducesResponseType(typeof(Response<TaskManagementHistoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTaskHistory([FromBody] TaskManagementHistoryCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.CreateTaskHistory(model, currentUser);
            return Helper.TransformData(result);
        }

        ///// <summary>
        ///// Cập nhật lịch sử công việc
        ///// </summary>
        ///// <param name="model"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpPut, Route("{id}")]
        //[ProducesResponseType(typeof(Response<TaskManagementViewModel>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskManagementCreateUpdateModel model)
        //{
        //    var currentUser = Helper.GetRequestInfo(Request);

        //    var result = await _handler.UpdateTask(id, model, currentUser);

        //    return Helper.TransformData(result);
        //}

        /// <summary>
        /// Lấy chi tiết 1 công việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("history/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<TaskManagementHistoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdTaskHistory(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.GetByIdTaskHistory(id, currentUser);

            return Helper.TransformData(result);
        }
        #endregion

        #region Task Management Milestone

        /// <summary>
        /// Danh sách milestones
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("milestone")]
        [ProducesResponseType(typeof(Response<TaskManagementMileStoneViewModel>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetPageTaskMileStone([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var filterObject = JsonConvert.DeserializeObject<TaskManagementMileStoneQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPageTaskMileStone(filterObject, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Tạo milestone
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("milestone")]
        [Authorize]
        [ProducesResponseType(typeof(Response<TaskManagementMileStoneViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] TaskManagementMileStoneCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.CreateTaskMileStone(model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật milestone
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("milestone/{id}")]
        [ProducesResponseType(typeof(Response<TaskManagementMileStoneViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTaskMileStone(Guid id, [FromBody] TaskManagementMileStoneCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);

            var result = await _handler.UpdateTaskMileStone(id, model, currentUser);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết 1 milestone
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("milestone/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<TaskManagementMileStoneViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdTaskMileStone(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.GetByIdTaskMileStone(id, currentUser);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa milestone
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("milestone/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTaskMileStone(Guid id)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.DeleteTaskMileStone(id, currentUser);

            return Helper.TransformData(result);
        }

        #endregion

    }
}
