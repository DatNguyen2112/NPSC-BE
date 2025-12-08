using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services.ChucVu;
using NSPC.Business.Services.PhongBan;
using NSPC.Common;

namespace NSPC.API.V1.Controllers.ChucVu
{
    // <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/chuc-vu")]
    [ApiExplorerSettings(GroupName = "Quản lý chức vụ")]
    public class ChucVuController: ControllerBase
    {
        private readonly IChucVuHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public ChucVuController(IChucVuHandler handler)
        {
            _handler = handler;
        }


        /// <summary>
        /// Tạo chức vụ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response<ChucVuViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] ChucVuCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);

            var result = await _handler.Create(model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật chức vụ
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(Response<ChucVuViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ChucVuCreateUpdateModel model)
        {
            var currentUser = Helper.GetRequestInfo(Request);

            var result = await _handler.Update(id, model, currentUser);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết 1 chức vụ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<ChucVuViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _handler.GetById(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách chức vụ
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<ChucVuViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<ChucVuQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa 1 chức vụ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(Response<ChucVuViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _handler.Delete(id);

            return Helper.TransformData(result);
        }
    }
}
