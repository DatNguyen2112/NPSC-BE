using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services;
using NSPC.Business.Services.ConstructionWeekReport;
using NSPC.Common;
using Microsoft.AspNetCore.Cors;

namespace NSPC.API.V1.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/week-report")]
    [ApiExplorerSettings(GroupName = "Báo cáo tuần")]
    public class ConstructionWeekReportController : ControllerBase
    {
        private readonly IConstructionWeekReportHandler  _handlerWeekReport;

        public ConstructionWeekReportController(IConstructionWeekReportHandler handlerWeekReport)
        {
            _handlerWeekReport = handlerWeekReport;
        }
        
        /// <summary>
        /// Tạo báo cáo tuần trong dự án
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response<ConstructionWeekReportViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] ConstructionWeekReportCreateModel model)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handlerWeekReport.Create(model, user);
            return Helper.TransformData(result);
        }
        
        /// <summary>
        /// Cập nhật báo cáo công việc
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(Response<ConstructionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ConstructionWeekReportCreateModel model)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handlerWeekReport.Update(id, model, user);
            return Helper.TransformData(result);
        }
        
        /// <summary>
        /// Chi tiết báo cáo công việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<ConstructionWeekReportViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handlerWeekReport.GetById(id, user);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa báo cáo công việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(Response<ConstructionWeekReportViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handlerWeekReport.Delete(id, user);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách báo cáo tuần trong dự án
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Response<Pagination<ConstructionWeekReportViewModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilterConstructionWeekReport([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<ConstructionWeekReportQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handlerWeekReport.GetPage(filterObject);
            return Helper.TransformData(result);
        }
        
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
            var filterObject = JsonConvert.DeserializeObject<ConstructionWeekReportQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handlerWeekReport.ExportExcelFile(filterObject);

            return result.Item1 != null
                ? Helper.TransformData(result.Item1)
                : File(result.Item2, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.Item3);
        }
    }
}
