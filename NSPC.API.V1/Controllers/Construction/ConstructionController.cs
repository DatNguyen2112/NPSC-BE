using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services;
using NSPC.Business.Services.ConstructionWeekReport;
using NSPC.Business.Services.Contract;
using NSPC.Business.Services.ExecutionTeams;
using NSPC.Common;
using NSPC.Data;

namespace NSPC.API.V1.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/construction")]
    [ApiExplorerSettings(GroupName = "Quản lý công trình/dự án")]
    public class ConstructionController : ControllerBase
    {
        private readonly IConstructionHandler _handler;
        private readonly IConstructionWeekReportHandler _handlerWeekReport;

        public ConstructionController(IConstructionHandler handler, IConstructionWeekReportHandler handlerWeekReport)
        {
            _handler = handler;
            _handlerWeekReport = handlerWeekReport;
        }

        /// <summary>
        /// Tạo công trình/dự án mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        // [Authorize]
        [RightValidate("CONSTRUCTION", RightActionConstants.ADD)]
        [ProducesResponseType(typeof(Response<ConstructionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] ConstructionCreateUpdateModel model)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.Create(model, user);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật công trình/dự án
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        // [Authorize]
        [RightValidate("CONSTRUCTION", RightActionConstants.UPDATE)]
        [ProducesResponseType(typeof(Response<ConstructionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ConstructionCreateUpdateModel model)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.Update(id, model, user);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách công trình dự án
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        // [Authorize]
        [RightValidateGroup("CONSTRUCTION.VIEWALL", "CONSTRUCTION.VIEWBYINDIVIDUAL", "CONSTRUCTION.VIEWBYTEAM", "CONSTRUCTION.VIEWBYDEPARTMENT")]
        [ProducesResponseType(typeof(ResponsePagination<ConstructionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<ConstructionQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách tổ thực hiện theo constructionId
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("execution-teams")]
        // [Authorize]
        [RightValidateGroup("CONSTRUCTION.VIEWALL", "CONSTRUCTION.VIEWBYINDIVIDUAL", "CONSTRUCTION.VIEWBYTEAM", "VIEWBYDEPARTMENT")]
        [ProducesResponseType(typeof(ResponsePagination<ExecutionTeamsViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExecutionTeams([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<ExecutionTeamsQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetExecutionTeamsInConstruction(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Chi tiết công trình/dự án
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [RightValidateGroup("CONSTRUCTION.VIEWALL", "CONSTRUCTION.VIEWBYINDIVIDUAL", "CONSTRUCTION.VIEWBYTEAM", "VIEWBYDEPARTMENT")]
        [ProducesResponseType(typeof(Response<ConstructionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.GetById(id, user);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa công trình dự án
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        // [Authorize]
        [RightValidate("CONSTRUCTION", RightActionConstants.DELETE)]
        [ProducesResponseType(typeof(Response<ConstructionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.Delete(id, user);

            return Helper.TransformData(result);
        }

        #region Dashboard dự án
        /// <summary>
        /// Dashboard dự án
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("construction-analyze-all")]
        // [Authorize]
        [RightValidate("DASHBOARD", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(ResponsePagination<ConstructionAnalyzeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AnalyzeAllConstruction([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<ConstructionQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.ConstructionSummaryAll(filterObject);
            return Helper.TransformData(result);
        }
        #endregion

        #region Dashboard doanh thu
        /// <summary>
        /// Dashboard doanh thu
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("contract-analyze-all")]
        // [Authorize]
        [RightValidate("DASHBOARD", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(ResponsePagination<ConstructionAnalyzeByPriorityOrInvestor>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AnalyzeContractAll([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<AnalyzeContractQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.ContractSummaryAll(filterObject);
            return Helper.TransformData(result);
        }
        #endregion

        /// <summary>
        /// Export excel to list by query parameters
        /// </summary>
        /// <param name="type"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [EnableCors("AllowOrigin")]
        [HttpPost("excel/export")]
        // [Authorize]
        [RightValidate("CONSTRUCTION", ConstructionConstants.ConstructionPermissionCode.EXPORTIMPORTFILE)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportExcelFile(
            [FromQuery] int size = 20,
            [FromQuery] int page = 1,
            [FromQuery] string filter = "{}",
            [FromQuery] string sort = ""
        )
        {
            var filterObject = JsonConvert.DeserializeObject<ConstructionQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.ExportExcelFile(filterObject);

            return result.Item1 != null
                ? Helper.TransformData(result.Item1)
                : File(result.Item2, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.Item3);
        }

        /// <summary>
        /// import từ excel
        /// </summary>
        /// <returns></returns>
        [EnableCors("AllowOrigin")]
        [HttpPost, Route("excel/import")]
        // [Authorize]
        [RightValidate("CONSTRUCTION", ConstructionConstants.ConstructionPermissionCode.EXPORTIMPORTFILE)]
        [ProducesResponseType(typeof(Response<List<ConstructionViewModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ImportExcelFile([FromForm] IFormFile file, [FromForm] bool overwrite = false)
        {
            var result = await _handler.ImportExcelFile(file, overwrite);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Đảo ngược trạng thái hoàn thành cho giai đoạn dự án
        /// </summary>
        /// <param name="constructionId"></param>
        /// <param name="templateStageId"></param>
        /// <returns></returns>
        [HttpPut("template-stage/{constructionId}/{templateStageId}/isDone")]
        [Authorize]
        [ProducesResponseType(typeof(Response<ConstructionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ToggleTemplateStageIsDone(Guid constructionId, Guid templateStageId)
        {
            var currentUser = Helper.GetRequestInfo(HttpContext.Request);
            var result = await _handler.ToggleTemplateStageIsDone(constructionId, templateStageId, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật danh sách giai đoạn cho công trình/dự án
        /// </summary>
        /// <param name="constructionId"></param>
        /// <param name="templateStages"></param>
        /// <returns></returns>
        [HttpPut("template-stages/{constructionId}")]
        [ProducesResponseType(typeof(Response<ConstructionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTemplateStages(Guid constructionId, [FromBody] List<jsonb_TemplateStage> templateStages)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.UpdateTemplateStages(constructionId, templateStages, user);
            return Helper.TransformData(result);
        }
        
        /// <summary>
        /// Cảnh báo quá tải nhân sự (>= 2 dự án)
        /// </summary>
        /// <returns></returns>
        [HttpPost("overload-warning")]
        [ProducesResponseType(typeof(Response<OverloadEmployeeModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> OverloadWarning([FromBody] CheckOverloadModel model)
        {
            // var user = Helper.GetRequestInfo(Request);
            var result = await _handler.CheckOverloadedEmployeesAsync(model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Get template stages with isDone status for a construction
        /// </summary>
        /// <param name="constructionId"></param>
        /// <returns></returns>
        [HttpGet("template-stages/{constructionId}/is-done-status")]
        [Authorize]
        [ProducesResponseType(typeof(Response<List<jsonb_TemplateStage>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTemplateStagesWithIsDoneStatus(Guid constructionId)
        {
            var result = await _handler.GetTemplateStagesWithIsDoneStatus(constructionId);
            return Helper.TransformData(result);
        }

    }
}
