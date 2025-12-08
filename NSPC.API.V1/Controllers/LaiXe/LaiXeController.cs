using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services.QuanLyLaiXe;
using NSPC.Common;

namespace NSPC.API.V1.Controllers.LaiXe
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/quan-ly-lai-xe")]
    [ApiExplorerSettings(GroupName = "Quản lý lái xe")]
    [Authorize]
    [AuthorizeByToken]
    public class PhuongTienController : ControllerBase
    {
        private readonly ILaiXeHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public PhuongTienController(ILaiXeHandler handler)
        {
            _handler = handler;
        }


        /// <summary>
        /// Thêm mới lái xe
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [RightValidate("CODETYPE", RightActionConstants.ADD)]
        [ProducesResponseType(typeof(Response<LaiXeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] LaiXeCreateUpdateModel model)
        {
            var result = await _handler.Create(model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật tài xế
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        [RightValidate("CODETYPE", RightActionConstants.UPDATE)]
        [ProducesResponseType(typeof(Response<LaiXeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] LaiXeCreateUpdateModel model)
        {
            var result = await _handler.Update(id, model);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết 1 tài xế
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [RightValidate("CODETYPE", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(Response<LaiXeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _handler.GetById(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách tài xế
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [RightValidate("CODETYPE", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(ResponsePagination<LaiXeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<LaiXeQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa 1 tài xế
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [RightValidate("CODETYPE", RightActionConstants.DELETE)]
        [ProducesResponseType(typeof(Response<LaiXeViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _handler.Delete(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa danh sách tài xế
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete, Route("")]
        [RightValidate("CODETYPE", RightActionConstants.DELETE)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMany([FromQuery] List<Guid> ids)
        {
            var result = await _handler.DeleteMany(ids);

            return Helper.TransformData(result);
        }
    }
}
