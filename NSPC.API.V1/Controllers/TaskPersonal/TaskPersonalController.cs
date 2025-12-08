using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services;
using NSPC.Business.Services.WorkItem;
using NSPC.Common;
using TaskQueryModel = NSPC.Business.Services.WorkItem.TaskQueryModel;

namespace NSPC.API.V1.Controllers.WorkItem
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/task-personal")]
    [ApiExplorerSettings(GroupName = "Quản lý công việc cá nhân")]
    [Authorize]
    [AuthorizeByToken]
    public class TaskPersonalController : ControllerBase
    {
        private readonly ITaskPersonalHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public TaskPersonalController(ITaskPersonalHandler handler)
        {
            _handler = handler;
        }


        /// <summary>
        /// Thêm mới công việc dự án
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        // [RightValidate("CONSTRUCTION", "ADDTASK")]
        [ProducesResponseType(typeof(Response<TaskPersonalViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] TaskPersonalCreateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.Create(model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật công việc dự án
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        // [RightValidate("CONSTRUCTION", "UPDATETASK")]
        [ProducesResponseType(typeof(Response<TaskPersonalViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] TaskPersonalCreateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.Update(id, model, currentUser);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết 1 công việc dự án
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        // [RightValidateGroup("CONSTRUCTION.VIEWALL", "CONSTRUCTION.VIEWBYINDIVIDUAL", "CONSTRUCTION.VIEWBYTEAM", "VIEWBYDEPARTMENT")]
        [ProducesResponseType(typeof(Response<TaskPersonalViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _handler.GetById(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách công việc dự án
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        // [RightValidateGroup("CONSTRUCTION.VIEWALL", "CONSTRUCTION.VIEWBYINDIVIDUAL", "CONSTRUCTION.VIEWBYTEAM", "VIEWBYDEPARTMENT")]
        [ProducesResponseType(typeof(ResponsePagination<TaskPersonalViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<TaskPersonalQueryModel>(filter);

            //sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa 1 công việc dự án
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(Response<TaskPersonalViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _handler.Delete(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa nhiều công việc dự án
        /// </summary>
        /// <param name="ids">Danh sách Id công việc cần xóa</param>
        /// <returns></returns>
        [HttpDelete, Route("many")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMany([FromQuery] List<Guid> ids)
        {
            var result = await _handler.DeleteMany(ids);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật trạng thái công việc dự án
        /// </summary>
        /// <param name="id">Id công việc</param>
        /// <param name="status">Trạng thái mới (tên enum, ví dụ: "InProgress")</param>
        /// <returns></returns>
        [HttpPut("{id}/status/{status}")]
        // [RightValidate("CONSTRUCTION", "SENDAPPROVALTASK")]
        [ProducesResponseType(typeof(Response<TaskPersonalViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            var result = await _handler.UpdateStatus(id, status);
            return Helper.TransformData(result);
        }
        
        /// <summary>
        /// Cập nhật trạng thái hàng loạt
        /// </summary>
        [HttpPut("status-many")]
        [ProducesResponseType(typeof(Response<List<TaskPersonalViewModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatusMany([FromBody] UpdateStatusManyModel model)
        {
            var result = await _handler.UpdateStatusMany(model.Ids, model.Status);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách công việc dự án theo ConstructionId
        /// </summary>
        /// <param name="constructionId">Id công trình/dự án</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        // [HttpGet("by-construction/{constructionId}")]
        // // [RightValidateGroup("CONSTRUCTION.VIEWALL", "CONSTRUCTION.VIEWBYINDIVIDUAL", "CONSTRUCTION.VIEWBYTEAM", "VIEWBYDEPARTMENT")]
        // [ProducesResponseType(typeof(ResponsePagination<TaskViewModel>), StatusCodes.Status200OK)]
        // public async Task<IActionResult> GetByConstructionId(
        //     Guid constructionId,
        //     [FromQuery] int size = 20,
        //     [FromQuery] int page = 1,
        //     [FromQuery] string filter = "{}",
        //     [FromQuery] string sort = "")
        // {
        //     var filterObject = JsonConvert.DeserializeObject<TaskQueryModel>(filter);
        //     //sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
        //     filterObject.Sort = sort ?? filterObject.Sort;
        //     filterObject.Size = size;
        //     filterObject.Page = page;
        //
        //     var result = await _handler.GetPageByConstructionId(constructionId, filterObject);
        //     return Helper.TransformData(result);
        // }
        
        // /// <summary>
        // /// Thống kê công việc theo giai đoạn
        // /// </summary>
        // /// <param name="constructionId">Id công trình/dự án</param>
        // /// <param name="size"></param>
        // /// <param name="page"></param>
        // /// <param name="filter"></param>
        // /// <param name="sort"></param>
        // /// <returns></returns>
        // [HttpGet("analyze-each-stage/{idTemplateStage}/{constructionId}")]
        // [RightValidateGroup("CONSTRUCTION.VIEWALL", "CONSTRUCTION.VIEWBYINDIVIDUAL", "CONSTRUCTION.VIEWBYTEAM", "VIEWBYDEPARTMENT")]
        // [ProducesResponseType(typeof(ResponsePagination<TaskOverviewEachStage>), StatusCodes.Status200OK)]
        // public async Task<IActionResult> AnalyzeEachStage(
        //     Guid idTemplateStage, Guid constructionId)
        // {
        //     var result = await _handler.GetAnalyzeByEachStage(idTemplateStage, constructionId);
        //     return Helper.TransformData(result);
        // }
        //
        // /// <summary>
        // /// Cập nhật trạng thái cho nhiều công việc dự án
        // /// </summary>
        // /// <param name="model">Model chứa danh sách Id công việc, trạng thái mới và mô tả</param>
        // /// <returns></returns>
        // [HttpPut("status-many")]
        // [ProducesResponseType(typeof(Response<List<TaskViewModel>>), StatusCodes.Status200OK)]
        // public async Task<IActionResult> UpdateStatusMany([FromBody] UpdateStatusManyModel model)
        // {
        //     var result = await _handler.UpdateStatusMany(model.Ids, model.Status, model.Description);
        //     return Helper.TransformData(result);
        // }
        //
        // /// <summary>
        // /// Lấy giá trị PriorityOrder lớn nhất của các task thuộc dự án (constructionId) và giai đoạn mẫu (idTemplateStage)
        // /// </summary>
        // /// <param name="constructionId">Id công trình/dự án</param>
        // /// <param name="idTemplateStage">Id giai đoạn mẫu</param>
        // /// <returns>Giá trị PriorityOrder lớn nhất</returns>
        // [HttpGet("max-priority-order/{constructionId}/{idTemplateStage}")]
        // [ProducesResponseType(typeof(Response<int>), StatusCodes.Status200OK)]
        // public async Task<IActionResult> GetMaxPriorityOrderByConstructionIdAndTemplateStage(Guid constructionId, Guid idTemplateStage)
        // {
        //     var result = await _handler.GetMaxPriorityOrderByConstructionIdAndTemplateStage(constructionId, idTemplateStage);
        //     return Helper.TransformData(result);
        // }
        //
        /// <summary>
        /// Lấy tổng hợp trạng thái công việc dự án (có phân trang/filter/sort như GetPage)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet("status-summary")]
        [ProducesResponseType(typeof(Response<TaskStatusSummaryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTaskStatusSummary(
            [FromQuery] int size = 20,
            [FromQuery] int page = 1,
            [FromQuery] string filter = "{}",
            [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<TaskPersonalQueryModel>(filter);
            filterObject.Sort = sort ?? filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;
        
            var result = await _handler.GetTaskStatusSummary(filterObject);
            return Helper.TransformData(result);
        }
        //
        // /// <summary>
        // /// Lấy tổng quan công việc dự án (có phân trang/filter/sort như GetPage)
        // /// </summary>
        // /// <param name="size"></param>
        // /// <param name="page"></param>
        // /// <param name="filter"></param>
        // /// <param name="sort"></param>
        // /// <returns></returns>
        // [HttpGet("overview-summary")]
        // [ProducesResponseType(typeof(Response<TaskOverviewSummaryViewModel>), StatusCodes.Status200OK)]
        // public async Task<IActionResult> GetTaskOverviewSummary(
        //     [FromQuery] int size = 20,
        //     [FromQuery] int page = 1,
        //     [FromQuery] string filter = "{}",
        //     [FromQuery] string sort = "")
        // {
        //     var filterObject = JsonConvert.DeserializeObject<TaskQueryModel>(filter);
        //     filterObject.Sort = sort ?? filterObject.Sort;
        //     filterObject.Size = size;
        //     filterObject.Page = page;
        //
        //     var result = await _handler.GetTaskOverviewSummary(filterObject);
        //     return Helper.TransformData(result);
        // }
    }
}
