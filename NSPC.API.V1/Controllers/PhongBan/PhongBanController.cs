using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business;
using NSPC.Business.Services.PhongBan;
using NSPC.Common;

namespace NSPC.API.V1.Controllers.PhongBan
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/phong-ban")]
    [ApiExplorerSettings(GroupName = "Quản lý phòng ban")]
    public class PhongBanController : ControllerBase
    {
        private readonly IPhongBanHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public PhongBanController(IPhongBanHandler handler)
        {
            _handler = handler;
        }


        /// <summary>
        /// Tạo phòng ban
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response<PhongBanViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] PhongBanCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);

            var result = await _handler.Create(model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật phòng ban
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(Response<PhongBanViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] PhongBanCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);

            var result = await _handler.Update(id, model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết 1 phòng ban
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<PhongBanViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _handler.GetById(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách phòng ban
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<PhongBanViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<PhongBanQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa 1 phòng ban
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(Response<PhongBanViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _handler.Delete(id);

            return Helper.TransformData(result);
        }
    }
}
