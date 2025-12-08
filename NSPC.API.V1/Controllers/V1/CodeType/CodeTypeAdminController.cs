using NSPC.Business;
using NSPC.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSPC.Api
{
    [Route("api/v1/admin/code-types")]
    [ApiExplorerSettings(GroupName = "CodeType Management")]
    [ApiController]
    public class CodeTypeAdminController : ControllerBase
    {
        private readonly ICodeTypeHandler _repository;
        private readonly IConfiguration _configuration;

        public CodeTypeAdminController(ICodeTypeHandler repository, IConfiguration configuration)
        {
            _configuration = configuration;
            _repository = repository;
        }

        /// <summary>
        /// Tạo Code Type
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        [RightValidate("CODETYPE", RightActionConstants.ADD)]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CodeTypeCreateUpdateModel request)
        {
            if (request == null)
                return new BadRequestResult();

            return Helper.TransformData(await _repository.CreateCodeType(request));
        }

        /// <summary>
        /// Lấy thông tin chi tiết Code type
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns></returns>
        [Authorize, HttpGet("{id}")]
        [RightValidate("CODETYPE", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _repository.GetById(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách Code Type
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [Authorize, HttpGet]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var languageCode = Request.Headers[ClaimConstants.LANGUAGE].FirstOrDefault();

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            var filterObject = JsonConvert.DeserializeObject<CodeTypeQueryModel>(filter);
            filterObject.Sort = sort != null ? sort : filterObject.Sort;

            filterObject.Size = size;
            filterObject.Page = page;
            filterObject.Language = languageCode != null ? languageCode.ToLower() : LanguageConstants.English;

            var result = await _repository.GetAdminPageAsync(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật thông tin Code type
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id">Id bản ghi</param>
        /// <returns></returns>
        [Authorize, HttpPut("{id}")]
        [RightValidate("CODETYPE", RightActionConstants.UPDATE)]
        [ProducesResponseType(typeof(Response<CodeTypeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] CodeTypeCreateUpdateModel request, Guid id)
        {
            if (request == null)
                return new BadRequestResult();

            var searchResults = await _repository.UpdateCodeType(request, id);

            return Helper.TransformData(searchResults);
        }

        /// <summary>
        /// Xóa Code Type
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns></returns>
        [Authorize, HttpDelete("{id}")]
        [RightValidate("CODETYPE", RightActionConstants.DELETE)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _repository.Delete(id);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách các Type của Code Type
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("/api/v1/admin/code-type/types")]
        public IActionResult GetCodeTypeTypes()
        {
            var result = new Pagination<CodeTypeTypeListModel>();
            result.Content = new List<CodeTypeTypeListModel>()
            {
              new CodeTypeTypeListModel { Code = CodeTypeConstants.Kho, Title = "Danh sách Kho" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.Don_Vi_Tinh, Title = "Đơn vị tính" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.Purpose_Receipt, Title = "Mục đích thu" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.Expenditure_Purpose, Title = "Mục đích chi" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.Payment_Method, Title = "Phương thức thanh toán" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.PaymentGroup, Title = "Nhóm người nộp/nhận" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.ActionCode, Title = "Thao tác" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.ReturnedReasonCode, Title = "Lý do trả hàng" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.InventoryImportType, Title = "Loại phiếu nhập kho" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.InventoryExportType, Title = "Loại phiếu xuất kho" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.CustomerGroup, Title = "Nhóm khách hàng" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.SupplierGroup, Title = "Nhóm nhà cung cấp" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.VATList, Title = "Danh sách thuế VAT" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.InventoryCheckNoteReason, Title = "Lý do kiểm hàng" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.CustomerType, Title = "Loại khách hàng" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.CustomerSource, Title = "Nguồn khách hàng" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.AssetType, Title = "Loại tài sản" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.AssetGroup, Title = "Nhóm tài sản" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.TaskStatus, Title = "Trạng thái công việc" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.TaskType, Title = "Loại công việc" },
              
              // EVN
              new CodeTypeTypeListModel { Code = CodeTypeConstants.VoltageType, Title = "Loại cấp điện áp" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.OwnerType, Title = "Loại chủ đầu tư" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.ConsultService, Title = "Loại dịch vụ tư vấn" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.ProcessTemplate, Title = "Mẫu quy trình" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.Investor, Title = "Chủ đầu tư" },
              new CodeTypeTypeListModel { Code = CodeTypeConstants.OrganizationStructure, Title = "Phòng ban" },
            };
            return Helper.TransformData(new Common.ResponsePagination<CodeTypeTypeListModel>(result));
        }
        
        [HttpGet, Route("/api/v1/admin/code-type/chart-of-accounts")]
        public IActionResult GetChartOfAccountTypes()
        {
            var result = new Pagination<CodeTypeTypeListModel>();
            result.Content = new List<CodeTypeTypeListModel>()
            {
                new CodeTypeTypeListModel { Code = CodeTypeConstants.Purpose_Receipt, Title = "Mục đích thu" },
                new CodeTypeTypeListModel { Code = CodeTypeConstants.Expenditure_Purpose, Title = "Mục đích chi" },
                //new CodeTypeTypeListModel { Code = CodeTypeConstants.REVENUE, Title = "Doanh thu" },
                //new CodeTypeTypeListModel { Code = CodeTypeConstants.EXPENSE, Title = "Chi phí" },
                //new CodeTypeTypeListModel { Code = CodeTypeConstants.ASSET, Title = "Tài sản" },
                //new CodeTypeTypeListModel { Code = CodeTypeConstants.LIABILITIES, Title = "Nợ" },
                //new CodeTypeTypeListModel { Code = CodeTypeConstants.EQUITY, Title = "Vốn chủ sở hữu" },
            };
            return Helper.TransformData(new Common.ResponsePagination<CodeTypeTypeListModel>(result));
        }
    }
}