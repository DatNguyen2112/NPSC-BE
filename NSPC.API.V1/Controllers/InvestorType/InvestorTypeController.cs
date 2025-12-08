using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services;
using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using NSPC.Business.Services.Investor;
using NSPC.Business.Services.InvestorType;

namespace NSPC.API.V1.Controllers.InvestorType
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/investor-type")]
    [ApiExplorerSettings(GroupName = "Danh mục loại chủ đầu tư")]
    public class InvestorTypeController : ControllerBase
    {
        private readonly InterfaceInvestorTypeHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public InvestorTypeController(InterfaceInvestorTypeHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Tạo danh mục loại chủ đầu tư
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [RightValidate("CODETYPE", RightActionConstants.ADD)]
        [ProducesResponseType(typeof(Response<InvestorTypeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] InvestorTypeCreateModel model)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.CreateAsync(model, user);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật danh mục loại chủ đầu tư
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        [RightValidate("CODETYPE", RightActionConstants.UPDATE)]
        [ProducesResponseType(typeof(Response<InvestorTypeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] InvestorTypeCreateModel model)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.UpdateAsync(id, model, user);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết danh mục loại chủ đầu tư
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [RightValidate("CODETYPE", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(Response<InvestorTypeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.GetById(id, user);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách danh mục loại chủ đầu tư
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [RightValidate("CODETYPE", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(ResponsePagination<InvestorTypeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<InvestorTypeQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPageAsync(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa danh mục loại chủ đầu tư
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [RightValidate("CODETYPE", RightActionConstants.DELETE)]
        [ProducesResponseType(typeof(Response<InvestorTypeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.DeleteAsync(id, user);
            return Helper.TransformData(result);
        }
    }
}
