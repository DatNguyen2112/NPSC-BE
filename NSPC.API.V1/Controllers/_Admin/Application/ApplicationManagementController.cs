using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business;
using NSPC.Common;

namespace NSPC.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/admin/applications")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Admin Application")]
    public class ApplicationManagementController : ControllerBase
    {
        private IApplicationHandler _handler;
        private ParameterCollection _parameterCollection;
        public ApplicationManagementController(IApplicationHandler handler, ParameterCollection parameterCollection)
        {
            _handler = handler;
            _parameterCollection = parameterCollection;
        }

        /// <summary>
        /// Lấy danh sách ứng dụng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet, Authorize, Route("")]
        [ProducesDefaultResponseType(typeof(ResponsePagination<ApplicationModel>))]
        public async Task<IActionResult> GetFilter([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}")
        {
            var userInfo = Helper.GetRequestInfo(Request);

            var filterObject = JsonConvert.DeserializeObject<ApplicationQueryModel>(filter);
            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _handler.GetPageAsync(filterObject, userInfo);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Update thông tin ứng dụng
        /// </summary>
        /// <returns></returns>
        [HttpPut, Authorize, Route("{id}")]
        [ProducesDefaultResponseType(typeof(Response<ApplicationModel>))]
        public async Task<IActionResult> Update(Guid id, ApplicationCreateUpdateModel model)
        {
            var userInfo = Helper.GetRequestInfo(Request);

            var result = await _handler.Update(id, model, userInfo);

            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Tạo mới ứng dụng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Authorize, Route("")]
        [ProducesDefaultResponseType(typeof(Response<ApplicationModel>))]
        public async Task<IActionResult> Create( ApplicationCreateUpdateModel model)
        {
            var userInfo = Helper.GetRequestInfo(Request);

            var result = await _handler.Create(model, userInfo);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Tạo mới ứng dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Authorize, Route("{id}")]
        [ProducesDefaultResponseType(typeof(Response<ApplicationModel>))]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userInfo = Helper.GetRequestInfo(Request);

            var result = await _handler.Delete(id, userInfo);

            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Tạo mới ứng dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Authorize, Route("{id}")]
        [ProducesDefaultResponseType(typeof(Response<ApplicationModel>))]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userInfo = Helper.GetRequestInfo(Request);

            var result = await _handler.GetById(id, userInfo);

            // Hander response
            return Helper.TransformData(result);
        }
    }
}
