using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Common
{
    public class AuthorizeByTokenAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        // private readonly IMemoryCache _memoryCache;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            
            var checkToken = Utils.GetConfig("Authentication:TokenSettings:Enable");

            if(checkToken == "false")
            {
                return;
            }

            RequestUser currentClaim = Helper.GetRequestInfo(context.HttpContext.Request);
            var token = context.HttpContext.Request.Headers["Authorization"].ToString();
            var _memoryCache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;

            var userInfo = _memoryCache.Get<UserAuthInfo>(currentClaim.UserId);
            if (userInfo == null || userInfo.Token == null || userInfo.Token != token)
            {
                var result = new ResponseError(HttpStatusCode.Unauthorized, "Phiên đăng nhập hết hạn");
                context.Result = TransformData(result);
                return;
            }
            return;
        }
    }



    public class UserAuthInfo
    {
        public string Token { get; set; }
    }
}
