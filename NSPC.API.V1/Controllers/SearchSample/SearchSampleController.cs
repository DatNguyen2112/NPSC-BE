using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business;
using NSPC.Common;

namespace FileManagement.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/search-sample")]
    [ApiExplorerSettings(GroupName = "Search samples")]
    public class SearchSampleController : ControllerBase
    {
        private readonly ISearchSampleHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        public SearchSampleController(ISearchSampleHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Lấy danh sách mẫu tìm kiếm
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Response<Pagination<SearchSampleModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPageAsync([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<SearchSampleQueryModel>(filter);

            if (string.IsNullOrEmpty(sort))
                sort = "-CreatedOnDate";

            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPage(filterObject);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xem chi tiết mẫu tìm kiếm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<SearchSampleModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _handler.GetById(id);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Tạo mẫu tìm kiếm
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response<SearchSampleModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] SearchSampleCreateUpdateModel model)
        {
            var result = await _handler.Create(model);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật mẫu tìm kiếm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Response<SearchSampleModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] SearchSampleCreateUpdateModel model)
        {
            var result = await _handler.Update(id, model);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa mẫu tìm kiếm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _handler.Delete(id);

            return Helper.TransformData(result);
        }
    }
}
