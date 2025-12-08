using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services.Contract;
using NSPC.Common;

namespace NSPC.API.V1.Controllers.Contract
{
    /// <summary>
    /// Controller for managing contracts
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [Route("api/v{api-version:apiVersion}/contracts")]
    [ApiExplorerSettings(GroupName = "Quản lý hợp đồng")]
    public class ContractController : ControllerBase
    {
        private readonly IContractHandler _handler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="handler">Contract handler</param>
        public ContractController(IContractHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Thêm mới hợp đồng/phụ lục
        /// </summary>
        /// <param name="model">Contract data</param>
        /// <returns>Created contract</returns>
        [HttpPost]
        [RightValidate("CONTRACT", "ADD")]
        [ProducesResponseType(typeof(Response<ContractViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] ContractCreateUpdateModel model)
        {
            var result = await _handler.Create(model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách hợp đồng/phụ lục phân trang
        /// </summary>
        /// <param name="size">Page size</param>
        /// <param name="page">Page number</param>
        /// <param name="filter">Filter parameters</param>
        /// <param name="sort">Sort parameters</param>
        /// <returns>Paginated list of contracts</returns>
        [HttpGet, Route("")]
        [RightValidate("CONTRACT", "VIEW")]
        [ProducesResponseType(typeof(ResponsePagination<ContractViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPage(
            [FromQuery] int size = 20,
            [FromQuery] int page = 1,
            [FromQuery] string filter = "{}",
            [FromQuery] string sort = ""
        )
        {
            var filterObject = JsonConvert.DeserializeObject<ContractQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy số lượng hợp đồng theo từng trạng thái
        /// </summary>
        /// <param name="size">Page size</param>
        /// <param name="page">Page number</param>
        /// <param name="filter">Filter parameters</param>
        /// <param name="sort">Sort parameters</param>
        /// <returns>Paginated list of contracts</returns>
        [HttpGet, Route("count-by-status")]
        [RightValidate("CONTRACT", "VIEW")]
        [ProducesResponseType(typeof(Response<Dictionary<string, int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CountByStatus(
            [FromQuery] int size = 20,
            [FromQuery] int page = 1,
            [FromQuery] string filter = "{}",
            [FromQuery] string sort = ""
        )
        {
            var filterObject = JsonConvert.DeserializeObject<ContractQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.CountByStatus(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xem chi tiết hợp đồng/phụ lục
        /// </summary>
        /// <param name="id">Contract ID</param>
        /// <returns>Contract details</returns>
        [HttpGet, Route("{id:guid}")]
        [RightValidate("CONTRACT", "VIEW")]
        [ProducesResponseType(typeof(Response<ContractDetailViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _handler.GetById(id);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xuất file excel
        /// </summary>
        /// <param name="size">Page size</param>
        /// <param name="page">Page number</param>
        /// <param name="filter">Filter parameters</param>
        /// <param name="sort">Sort parameters</param>
        /// <returns>Paginated list of contracts</returns>
        [HttpGet, Route("export-excel")]
        [RightValidate("CONTRACT", "EXPORTIMPORTFILE")]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportExcelFile(
            [FromQuery] int size = 20,
            [FromQuery] int page = 1,
            [FromQuery] string filter = "{}",
            [FromQuery] string sort = ""
        )
        {
            var filterObject = JsonConvert.DeserializeObject<ContractQueryModel>(filter);

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
        /// Nhập file excel
        /// </summary>
        /// <param name="file">Contracts file</param>
        /// <param name="overwrite">Overwrite existing contracts</param>
        /// <returns>Contract details</returns>
        [HttpPost, Route("import-excel")]
        [RightValidate("CONTRACT", "EXPORTIMPORTFILE")]
        [ProducesResponseType(typeof(Response<ContractDetailViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ImportExcelFile([FromForm] IFormFile file, [FromForm] bool overwrite = false)
        {
            var result = await _handler.ImportExcelFile(file, overwrite);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Chỉnh sửa hợp đồng/phụ lục
        /// </summary>
        /// <param name="id">Contract ID</param>
        /// <param name="model">Updated contract data</param>
        /// <returns>Updated contract</returns>
        [HttpPut, Route("{id:guid}")]
        [RightValidate("CONTRACT", "UPDATE")]
        [ProducesResponseType(typeof(Response<ContractViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ContractCreateUpdateModel model)
        {
            var result = await _handler.Update(id, model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa hợp đồng/phụ lục
        /// </summary>
        /// <param name="id">Contract ID</param>
        /// <returns>Response indicating success or failure</returns>
        [HttpDelete, Route("{id:guid}")]
        [RightValidate("CONTRACT", "DELETE")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _handler.Delete(id);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách báo cáo công nợ phân trang
        /// </summary>
        /// <param name="size">Page size</param>
        /// <param name="page">Page number</param>
        /// <param name="filter">Filter parameters</param>
        /// <param name="sort">Sort parameters</param>
        /// <returns>Paginated list of contracts</returns>
        [HttpGet, Route("debt-report")]
        [RightValidate("DEBT", "VIEW")]
        [ProducesResponseType(typeof(ResponsePagination<DebtReportViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPageDebtReport([FromQuery] int size = 20, [FromQuery] int page = 1,
            [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<DebtReportQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPageDebtReport(filterObject);
            return Helper.TransformData(result);
        }
    }
}