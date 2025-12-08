using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services.ProjectTemplate;
using NSPC.Common;

namespace NSPC.API.V1.Controllers.ProjectTemplate
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/project-template")]
    [ApiExplorerSettings(GroupName = "Quản lý template dự án")]
    [Authorize]
    [AuthorizeByToken]
    public class ProjectTemplateController : ControllerBase
    {
        private readonly IProjectTemplateHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public ProjectTemplateController(IProjectTemplateHandler handler)
        {
            _handler = handler;
        }


        /// <summary>
        /// Thêm mới template dự án
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response<ProjectTemplateViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] ProjectTemplateCreateUpdateModel model)
        {
            var result = await _handler.Create(model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật template dự án
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(Response<ProjectTemplateViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProjectTemplateCreateUpdateModel model)
        {
            var result = await _handler.Update(id, model);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết 1 template dự án
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<ProjectTemplateViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _handler.GetById(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách template dự án
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<ProjectTemplateViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<ProjectTemplateQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa 1 template dự án
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(Response<ProjectTemplateViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _handler.Delete(id);

            return Helper.TransformData(result);
        }
    }
}
