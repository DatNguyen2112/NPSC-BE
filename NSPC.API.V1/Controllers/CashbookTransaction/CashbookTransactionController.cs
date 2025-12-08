using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Configuration;
using NSPC.Business.Services;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.InventoryNote;
using NSPC.Business.Services.Quotation;
using NSPC.Common;

namespace NSPC.API.V1.Controllers.CashbookTransaction
{
    // <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/cashbook_transaction")]
    [ApiExplorerSettings(GroupName = "Quản lý thu chi")]
    public class CashbookTransactionController : ControllerBase
    {
        private readonly ICashbookTransactionHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public CashbookTransactionController(ICashbookTransactionHandler handler)
        {
            _handler = handler;
        }


        /// <summary>
        /// Tạo thu chi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response<CashbookTransactionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CashbookTransactionCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.Create(model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Tạo nhiều phiếu thu/chi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("create-multiple")]
        [ProducesResponseType(typeof(Response<CashbookTransactionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateMultiple([FromBody] CashbookTransactionCreateUpdateMutipleModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.CreateMultiple(model, currentUser);
            return Helper.TransformData(result);
        }


        /// <summary>
        /// Lấy tổng thu chi, số dư đầu/cuối kỳ
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("transaction-summary")]
        [ProducesResponseType(typeof(Response<CashbookTransactionViewModel>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetSummaryTransaction([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<CashbookTransactionQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetTransactionSummary(filterObject);
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Lấy chi tiết 1 thu chi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<CashbookTransactionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _handler.GetById(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách thu chi
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<CashbookTransactionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<CashbookTransactionQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật phiếu thu/chi
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(Response<CashbookTransactionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] CashbookTransactionCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);
            var result = await _handler.Update(id, model, currentUser);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Hủy 1 hoặc nhiều phiếu thu/chi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize, HttpPut, Route("cancel_voucher")]
        //[Allow(RoleConstants.AdminRoleCode)]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeactiveCashbookTransactionAsync([FromQuery] List<Guid> ids)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _handler.DeactiveCashbookTransactionsAsync(ids);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Thanh toán phiếu thu/chi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize, HttpPut, Route("payment_invoice")]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<CashbookTransactionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PayInvoice(Guid id)
        {
            // Get Token Info
            var currentUser = Helper.GetRequestInfo(Request);

            var result = await _handler.PayInvoiceAsync(id, currentUser);

            // Hander response
            return Helper.TransformData(result);
        }


        /// <summary>
        /// Export excel list cashbook transaction by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// [EnableCors("AllowOrigin")]
        [HttpPost("export-excel-list/{type}")]
        [Authorize]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportExcelListCashbookTransaction(string type)
        {
            var result = _handler.ExportExcelListCashbookTransaction(type).Result.Data;
            byte[] file;

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream(result, FileMode.Open))
                {
                    await fs.CopyToAsync(ms);
                }
                file = ms.ToArray();
            }
            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(result));
        }

        /// <summary>
        /// Export excel list cashbook transaction by type and query parameters for current page
        /// </summary>
        /// <param name="type"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [EnableCors("AllowOrigin")]
        [HttpPost("export-excel-list-current-page/{type}")]
        [Authorize]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportExcelListCashbookTransactionCurrentPage(string type, [FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<CashbookTransactionQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.ExportExcelListCashbookTransactionCurrentPage(type, filterObject);
            if (result.Data == null)
            {
                return NotFound(new { message = "Không tìm thấy dữ liệu để xuất." });
            }

            byte[] file;

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream(result.Data, FileMode.Open))
                {
                    await fs.CopyToAsync(ms);
                }
                file = ms.ToArray();
            }

            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(result.Data));
        }

        /// <summary>
        /// Xuất excel danh sách phiếu sổ quỹ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [EnableCors("AllowOrigin")]
        [HttpPost("export-excel-list-vouchers")]
        [Authorize]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportExcelListVouchersCurrentPage([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<CashbookTransactionQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.ExportExcelListVouchersPage(filterObject);
            if (result.Data == null)
            {
                return NotFound(new { message = "Không tìm thấy dữ liệu để xuất." });
            }

            byte[] file;

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream(result.Data, FileMode.Open))
                {
                    await fs.CopyToAsync(ms);
                }
                file = ms.ToArray();
            }

            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(result.Data));
        }

        /// <summary>
        /// Báo cáo dòng tiền
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("cash-flow-report")]
        [ProducesResponseType(typeof(Response<CashFlowReportViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCashFlowReportAsync([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<CashbookTransactionQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetCashFlowReportAsync(filterObject);
            return Helper.TransformData(result);
        }
        
        /// <summary>
        /// Dashboard không filter
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("dashboard-no-filter")]
        [ProducesResponseType(typeof(Response<CashbookDashboardViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardWithNoFilter([FromQuery] CashbookDashboardFilterModel filterObject)
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ"
            };
            var filter = JsonConvert.DeserializeObject<CashbookDashboardFilterDatetimeModel>(filterObject.Filter ?? string.Empty, settings);
            var result = await _handler.DashboardWithNoFilter(filter);
            return Helper.TransformData(result);
        }
        
        /// <summary>
        /// Dashboard tổng kèm filter
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("dashboard-total-filter")]
        [ProducesResponseType(typeof(Response<CashbookDashboardViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardTotalFilter([FromQuery] CashbookDashboardFilterModel filterObject)
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ"
            };
            var filter = JsonConvert.DeserializeObject<CashbookDashboardFilterDatetimeModel>(filterObject.Filter ?? string.Empty, settings);
            var result = await _handler.DashboardWithTotalFilter(filter);
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Dashboard thu và chi
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("receipts-expenditures-dashboard")]
        [ProducesResponseType(typeof(Response<CashbookDashboardViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransactionDashboard([FromQuery] CashbookDashboardFilterModel filterObject)
        {
            var result = await _handler.ReceiptsDashboardFilter(filterObject);
            return Helper.TransformData(result);
        }


        /// <summary>
        /// Xuất báo cáo dòng tiền ra file excel
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [EnableCors("AllowOrigin")]
        [HttpPost("cash-flow-report/export")]
        [Authorize]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportExcelCashFlowReportAsync([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<CashbookTransactionQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.ExportExcelCashFlowReportAsync(filterObject);
            if (result.Data == null)
            {
                return NotFound(new { message = "Không tìm thấy dữ liệu để xuất." });
            }

            byte[] file;

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream(result.Data, FileMode.Open))
                {
                    await fs.CopyToAsync(ms);
                }
                file = ms.ToArray();
            }

            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(result.Data));
        }
    }
}
