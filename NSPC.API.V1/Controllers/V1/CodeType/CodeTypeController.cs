using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSPC.Business;
using NSPC.Common;

namespace NSPC.Api
{
    [Route("api/v1/code-type")]
    [ApiExplorerSettings(GroupName = "CodeType")]
    [ApiController]
    public class CodeTypeController : ControllerBase
    {
        private readonly ICodeTypeHandler _repository;
        private readonly IConfiguration _configuration;

        public CodeTypeController(ICodeTypeHandler repository,
                                             IConfiguration configuration)
        {
            _configuration = configuration;
            _repository = repository;
        }

        /// <summary>
        /// Lấy danh sách Code Type
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var languageCode = Request.Headers[ClaimConstants.LANGUAGE].FirstOrDefault();

            var filterObject = JsonConvert.DeserializeObject<CodeTypeQueryModel>(filter);
            filterObject.Sort = string.IsNullOrWhiteSpace(sort) ? "+Order" : sort + "," + "+Order";

            filterObject.Size = size;
            filterObject.Page = page;
            filterObject.Language = LanguageConstants.Default;// languageCode != null ? languageCode.ToLower() : LanguageConstants.Default;

            var result = await _repository.GetListPageAsync(filterObject);
            return Helper.TransformData(result);
        }

        private async Task<Response> GetCodeTypeFilter(string type, int size, int page, string filter, string sort, string format)
        {
            var languageCode = Request.Headers[ClaimConstants.LANGUAGE].FirstOrDefault();
            var filterObject = JsonConvert.DeserializeObject<CodeTypeQueryModel>(filter);
            filterObject.Sort = string.IsNullOrWhiteSpace(sort) ? "+Order" : sort + "," + "+Order";
            filterObject.Size = size;
            filterObject.Page = page;
            filterObject.Type = type;
            filterObject.Language = LanguageConstants.Default; // languageCode != null ? languageCode.ToLower() : LanguageConstants.Default;
            filterObject.Format = format;
            var result = await _repository.GetListPageAsync(filterObject);
            return result;
        }

        // /// <summary>
        // /// Lấy danh sách Bằng cấp chuyên môn
        // /// </summary>
        // /// <param name="size"></param>
        // /// <param name="page"></param>
        // /// <param name="filter"></param>
        // /// <param name="sort"></param>
        // /// <returns></returns>
        // [HttpGet, AllowAnonymous]
        // [Route("medical-degrees")]
        // [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        // public async Task<IActionResult> GetMedicalDegrees(int size = 40, int page = 1, string filter = "{}", string sort = "+Order")
        // {
        //     return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.MedicalDegree, size, page, filter, sort, string.Empty));
        // }
        //
        //
        // /// <summary>
        // /// Lấy danh sách Nhóm thủ thuật
        // /// </summary>
        // /// <param name="size"></param>
        // /// <param name="page"></param>
        // /// <param name="filter"></param>
        // /// <param name="sort"></param>
        // /// <returns></returns>
        // [HttpGet, AllowAnonymous]
        // [Route("medical-procedure-groups")]
        // [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        // public async Task<IActionResult> GetMedicalproceduregroups(int size = 20, int page = 1, string filter = "{}", string sort = "+Order")
        // {
        //     return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.MedicalProcedureGroup, size, page, filter, sort, string.Empty));
        // }
        //
        // /// <summary>
        // /// Lấy danh sách Độ khó thủ thuật
        // /// </summary>
        // /// <param name="size"></param>
        // /// <param name="page"></param>
        // /// <param name="filter"></param>
        // /// <param name="sort"></param>
        // /// <returns></returns>
        // [HttpGet, AllowAnonymous]
        // [Route("medical-procedure-difficulties")]
        // [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        // public async Task<IActionResult> GetMedicalProceduresDifficulties(int size = 20, int page = 1, string filter = "{}", string sort = "+Order")
        // {
        //     return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.MedicalProcedureDifficulty, size, page, filter, sort, string.Empty));
        // }
        //
        // /// <summary>
        // /// Lấy danh sách Độ khó thủ thuật
        // /// </summary>
        // /// <param name="size"></param>
        // /// <param name="page"></param>
        // /// <param name="filter"></param>
        // /// <param name="sort"></param>
        // /// <returns></returns>
        // [HttpGet, AllowAnonymous]
        // [Route("number-of-teeths")]
        // [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        // public async Task<IActionResult> GetNumberOfTeeth(int size = 20, int page = 1, string filter = "{}", string sort = "+Order")
        // {
        //     return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.NumberOfTeeth, size, page, filter, sort, string.Empty));
        // }

        /// <summary>
        /// Lấy danh sách giới tính
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("gender")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGenderCodeTypeTypes()
        {
            var result = new Pagination<CodeTypeListModel>();
            result.Content = new List<CodeTypeListModel>()
            {
              new CodeTypeListModel { Code = GenderConstants.FEMALE, Title = "Nữ" },
              new CodeTypeListModel { Code = GenderConstants.MALE, Title = "Nam" },
            };
            return Helper.TransformData(new Common.ResponsePagination<CodeTypeListModel>(result));
        }
        /// <summary>
        /// Lấy danh sách loại khách hàng
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("loai-khach-hang")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetResidenceInfo(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.Loai_khach_hang, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách nhóm người nộp
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("payment-group")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentGroup(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.PaymentGroup, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách Kho
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("kho")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMimeType(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.Kho, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách Đơn vị tính
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("don-vi-tinh")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDonViTinh(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.Don_Vi_Tinh, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách loại phiếu thu
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("purpose_receipts")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPurposeReceipt(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.Purpose_Receipt, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách loại phiếu chi
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("expenditure_purposes")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExpenditurePurpose(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.Expenditure_Purpose, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách phương thức thanh toán
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("payment_methods")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentMethod(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.Payment_Method, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách loại phiếu nhập kho
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("inventory-import-types")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInventoryImportType(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.InventoryImportType, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách loại phiếu xuất kho
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("inventory-export-types")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInventoryExportType(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.InventoryExportType, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách lý do trả hàng
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("returned_reason")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReasonCode(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.ReturnedReasonCode, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách nhóm khách hàng
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("customer-group")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomerGroup(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.CustomerGroup, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách nhóm nhà cung cấp
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("supplier-group")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSupplierGroup(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.SupplierGroup, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách Thuế VAT
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("vat-list")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVatList(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.VATList, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách lý do kiểm hàng
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("inventory-check-note-reason")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInventoryCheckNoteReason(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.InventoryCheckNoteReason, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách loại tài sản
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("asset-type")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAssetTypeCodes(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.AssetType, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách nhóm tài sản
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("asset-group")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAssetGroupCodes(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.AssetGroup, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách Loại khách hàng
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("customer-type")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListCustomerType(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.CustomerType, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách Nguồn khách hàng
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("customer-source")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListCustomerSource(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.CustomerSource, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách phần trăm thuế GTGT
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("vat-gtgt")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListVatGTGT(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.VatGTGT, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách trạng thái công việc
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("task-status")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTaskStatus(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.TaskStatus, size, page, filter, sort, string.Empty));
        }

        /// <summary>
        /// Lấy danh sách loại công việc
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("task-type")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTaskType(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.TaskType, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách loại cấp điện áp
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("voltage-type")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVoltageType(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.VoltageType, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách loại chủ đầu tư
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("owner-type")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOwnerType(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.OwnerType, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách loại dịch vụ tư vấn
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("consult-service")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConsultService(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.ConsultService, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách phòng ban
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("organization-structure")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrganizationStructure(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.OrganizationStructure, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách mẫu quy trình
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("process-template")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProcessTemplate(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.ProcessTemplate, size, page, filter, sort, string.Empty));
        }
        
        /// <summary>
        /// Lấy danh sách chủ đầu tư
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("investor")]
        [ProducesResponseType(typeof(ResponsePagination<CodeTypeListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInvestor(int size = 20, int page = 1, string filter = "{}", string sort = "")
        {
            return Helper.TransformData(await GetCodeTypeFilter(CodeTypeConstants.Investor, size, page, filter, sort, string.Empty));
        }
    }
}