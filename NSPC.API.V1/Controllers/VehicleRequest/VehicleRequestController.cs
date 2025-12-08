using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services.VehicleRequest;
using NSPC.Common;

namespace NSPC.API.V1.Controllers.VehicleRequest
{
    /// <summary>
    /// Controller for managing vehicle requests
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/vehicle-request")]
    [ApiExplorerSettings(GroupName = "Quản lý yêu cầu xe")]
    [AuthorizeByToken]
    public class VehicleRequestController : ControllerBase
    {
        private readonly IVehicleRequestHandler _handler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="handler">Vehicle request handler</param>
        public VehicleRequestController(IVehicleRequestHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Tạo yêu cầu xin xe mới
        /// </summary>
        /// <param name="model">Vehicle request data</param>
        /// <returns>Created vehicle request</returns>
        [HttpPost]
        [RightValidate("VEHICLEREQUEST", "ADD")]
        [ProducesResponseType(typeof(Response<VehicleRequestViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] VehicleRequestCreateUpdateModel model)
        {
            var result = await _handler.Create(model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật yêu cầu xin xe
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <param name="model">Updated vehicle request data</param>
        /// <returns>Updated vehicle request</returns>
        [HttpPut, Route("{id:guid}")]
        [RightValidate("VEHICLEREQUEST", "UPDATE")]
        [ProducesResponseType(typeof(Response<VehicleRequestViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] VehicleRequestCreateUpdateModel model)
        {
            var result = await _handler.Update(id, model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách yêu cầu xin xe
        /// </summary>
        /// <param name="size">Page size</param>
        /// <param name="page">Page number</param>
        /// <param name="filter">Filter parameters</param>
        /// <param name="sort">Sort parameters</param>
        /// <returns>Paginated list of vehicle requests</returns>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<VehicleRequestViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPage([FromQuery] int size = 20, [FromQuery] int page = 1,
            [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<VehicleRequestQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết yêu cầu xin xe
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <returns>Vehicle request details</returns>
        [HttpGet, Route("{id:guid}")]
        [ProducesResponseType(typeof(Response<VehicleRequestViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _handler.GetById(id);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa yêu cầu xin xe
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <returns>Response indicating success or failure</returns>
        [HttpDelete, Route("{id:guid}")]
        [RightValidate("VEHICLEREQUEST", "UPDATE")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _handler.Delete(id);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Phê duyệt hoặc từ chối yêu cầu xin xe
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <param name="model">Approval data</param>
        /// <returns>Updated vehicle request</returns>
        [HttpPut, Route("{id:guid}/process-approval")]
        [RightValidate("VEHICLEREQUEST", "APPROVE")]
        [ProducesResponseType(typeof(Response<VehicleRequestViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessApproval(Guid id, [FromBody] VehicleRequestApprovalModel model)
        {
            var result = await _handler.ProcessApproval(id, model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Gửi duyệt yêu cầu xin xe
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <returns>List of conflicting vehicle requests (empty if no conflicts or other errors)</returns>
        [HttpPut, Route("{id:guid}/submit-for-approval")]
        [RightValidate("VEHICLEREQUEST", "SENDAPPROVAL")]
        [ProducesResponseType(typeof(Response<List<VehicleRequestViewModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SubmitForApproval(Guid id)
        {
            var result = await _handler.SubmitForApproval(id);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Gửi yêu cầu ghép xe
        /// </summary>
        /// <param name="draftRequestId">ID của yêu cầu xe nháp muốn ghép</param>
        /// <param name="approvedRequestIds">Danh sách ID các yêu cầu xe đã duyệt/ghép để ghép cùng</param>
        /// <returns>Updated vehicle request</returns>
        [HttpPut, Route("{draftRequestId:guid}/submit-vehicle-sharing")]
        [RightValidate("VEHICLEREQUEST", "SENDAPPROVAL")]
        [ProducesResponseType(typeof(Response<VehicleRequestViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SubmitVehicleSharing(Guid draftRequestId,
            [FromBody] List<Guid> approvedRequestIds)
        {
            var result = await _handler.SubmitVehicleSharing(draftRequestId, approvedRequestIds);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Duyệt yêu cầu ghép xe
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <param name="approvedRequestIds">Danh sách ID các yêu cầu xe đã duyệt/ghép để ghép cùng</param>
        /// <returns>Updated vehicle request</returns>
        [HttpPut, Route("{id:guid}/approve-vehicle-sharing")]
        [RightValidate("VEHICLEREQUEST", "APPROVE")]
        [ProducesResponseType(typeof(Response<VehicleRequestViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveVehicleSharing(Guid id, [FromBody] List<Guid> approvedRequestIds)
        {
            var result = await _handler.ApproveVehicleSharing(id, approvedRequestIds);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xuất ra file pdf yêu cầu xin xe
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id:guid}/pdf")]
        [RightValidate("VEHICLEREQUEST", "PRINT")]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestPdf(Guid id)
        {
            var result = await _handler.GetRequestPdf(id);

            return result.Item1 != null
                ? Helper.TransformData(result.Item1)
                : File(result.Item2, "application/pdf", result.Item3);
        }

        /// <summary>
        /// Lấy cấu hình xuất báo cáo yêu cầu xe
        /// </summary>
        /// <returns>Export configuration</returns>
        [HttpGet, Route("export-config")]
        [RightValidate("VEHICLEREQUEST", "PRINT")]
        [ProducesResponseType(typeof(Response<VehicleRequestExportConfig>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExportConfig()
        {
            var result = await _handler.GetExportConfig();
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật cấu hình xuất báo cáo yêu cầu xe
        /// </summary>
        /// <param name="config">Export configuration data</param>
        /// <returns>Updated export configuration</returns>
        [HttpPut, Route("export-config")]
        [RightValidate("VEHICLEREQUEST", "CONFIGPRINT")]
        [ProducesResponseType(typeof(Response<VehicleRequestExportConfig>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateExportConfig([FromBody] VehicleRequestExportConfig config)
        {
            var result = await _handler.UpdateExportConfig(config);
            return Helper.TransformData(result);
        }
    }
}