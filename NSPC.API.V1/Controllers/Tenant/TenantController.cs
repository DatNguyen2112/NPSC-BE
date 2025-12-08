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
    [Route("api/v{api-version:apiVersion}/tenants")]
    [ApiExplorerSettings(GroupName = "Tenant")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public TenantController(ITenantHandler handler)
        {
            _handler = handler;
        }


        /// <summary>
        /// Đăng ký tenant mới 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("register")]
        [ProducesResponseType(typeof(Response<TenantModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] TenantCreateModel model)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.RegisterTenant(model, user);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Check domain exists
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        [HttpGet, Route("check-subdomain")]
        [ProducesResponseType(typeof(Response<TenantModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckDomain([FromQuery] string subDomain)
        {
            var user = Helper.GetRequestInfo(Request);
            var result = await _handler.CheckDomainExists(subDomain);
            return Helper.TransformData(result);
        }
    }
}
