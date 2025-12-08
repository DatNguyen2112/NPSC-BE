using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business;
using NSPC.Business.Services.ChucVu;
using NSPC.Business.Services.VatTu;
using NSPC.Common;

namespace NSPC.API.V1
{
    // <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/admin/tenants")]
    [ApiExplorerSettings(GroupName = "Quản lý Tenant")]
    public class TenantManagementController : ControllerBase
    {
        private readonly ITenantHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public TenantManagementController(ITenantHandler handler)
        {
            _handler = handler;
        }


        /// <summary>
        /// Tạo tenant
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response<TenantModel>), StatusCodes.Status200OK)]
        [Allow(RoleConstants.SuperAdminRoleCode)]
        public async Task<IActionResult> Create([FromBody] TenantCreateModel model)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.Create(model, user);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật tenant
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(Response<TenantModel>), StatusCodes.Status200OK)]
        [Allow(RoleConstants.SuperAdminRoleCode)]
        public async Task<IActionResult> Update(Guid id, [FromBody] TenantUpdateModel model)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.Update(id, model, user);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy chi tiết 1 tenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<VatTuViewModel>), StatusCodes.Status200OK)]
        [Allow(RoleConstants.SuperAdminRoleCode)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = Helper.GetRequestInfo(Request);

            var result = await _handler.GetById(id, user);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách tenant
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<TenantModel>), StatusCodes.Status200OK)]
        [Allow(RoleConstants.SuperAdminRoleCode)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var user = Helper.GetRequestInfo(Request);
            var filterObject = JsonConvert.DeserializeObject<TenantQueryModel>(filter);
            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _handler.GetPageAsync(filterObject, user);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa 1 tenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(Response<VatTuViewModel>), StatusCodes.Status200OK)]
        [Allow(RoleConstants.SuperAdminRoleCode)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = Helper.GetRequestInfo(Request);

            var result = await _handler.Delete(id, user);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// get tenant by domain
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        [HttpGet, Route("get-by-domain")]
        [ProducesResponseType(typeof(Response<TenantModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByDomain([FromQuery] string domain)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.GetByDomain(domain);
            return Helper.TransformData(result);
        }
    }
}
