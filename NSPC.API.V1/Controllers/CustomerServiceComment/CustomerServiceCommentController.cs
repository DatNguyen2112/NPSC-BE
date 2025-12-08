using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business;
using NSPC.Business.Services;
using NSPC.Common;

namespace NSPC.API.V1.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/customer-service-comment")]
    [ApiExplorerSettings(GroupName = "Quản lý tương tác")]
    public class CustomerServiceCommentController: ControllerBase
    {
        private readonly ICustomerServiceCommentHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public CustomerServiceCommentController(ICustomerServiceCommentHandler handler)
        {
            _handler = handler;
        }
        
        /// <summary>
        /// Tạo bình luận mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response<CustomerServiceCommentViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CustomerServiceCommentCreateModel model)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.Create(model, user);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Sửa nội dung bình luận
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(Response<CustomerServiceCommentViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] CustomerServiceCommentCreateModel model)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.Update(id, model, user);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách bình luận
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<CustomerServiceCommentViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<CustomerServiceQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPageAsync(filterObject);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa bình luận
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(Response<CustomerServiceCommentViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.Delete(id, user);

            return Helper.TransformData(result);
        }
    }
}
