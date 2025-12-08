using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business;
using NSPC.Common;
using System.CodeDom;

namespace NSPC.API
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/idm/rights")]
    [ApiExplorerSettings(GroupName = "Admin Right")]
    [AuthorizeByToken]
    [Authorize]
    public class RightController : ControllerBase
    {
        private readonly IRightHandler _rightHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rightHandler"></param>
        public RightController(IRightHandler rightHandler)
        {
            _rightHandler = rightHandler;
        }


        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        [HttpPost, Route("")]
        [ProducesResponseType(typeof(Response<RightViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] RightCreateUpdateModel model)
        {
            var result = await _rightHandler.Create(model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model">Dữ liệu</param>
        [HttpPut, Route("")]
        [ProducesResponseType(typeof(Response<RightViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] RightCreateUpdateModel model)
        {
            var result = await _rightHandler.Update(id, model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy tất cả
        /// </summary>
        [HttpGet, Route("all")]
        [ProducesResponseType(typeof(Response<List<RightViewModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rightHandler.GetAll();
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về theo bộ loc
        /// </summary>
        /// <param name="page">Số thứ tự trang tìm kiếm</param>
        /// <param name="size">Số bản ghi giới hạn một trang</param>
        /// <param name="filter">Thông tin lọc nâng cao (Object Json)</param>
        /// <param name="sort">Thông tin sắp xếp (Array Json)</param>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<RoleModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilterAsync([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<RightQueryModel>(filter);
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _rightHandler.GetPage(filterObject);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete, Route("")]
        [ProducesResponseType(typeof(Response<RightViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _rightHandler.Delete(id);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết
        /// </summary>
        /// <param name="id"></param>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<RightViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _rightHandler.GetById(id);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Seed right
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="groupName"></param>
        [HttpPost, Route("seed")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> SeedRight([FromQuery] string groupCode, string groupName)
        {
            var result = await _rightHandler.SeedRight(groupCode, groupName);
            return Helper.TransformData(result);
        }
    }
}
