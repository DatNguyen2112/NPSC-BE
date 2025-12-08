using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Common
{
    public class RightValidateAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _right = null;
        public RightValidateAttribute(string group, string action)
        {
            _right = group + "." + action;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            RequestUser currentClaim = Helper.GetRequestInfo(context.HttpContext.Request);
            if (currentClaim.ListRoles.Contains(RoleConstants.AdminRoleCode))
                return;
            if (!currentClaim.ListRights.Contains(_right))
            {
                var result = new ResponseError(HttpStatusCode.Forbidden, "Bạn không có quyền truy cập chức năng này");
                context.Result = TransformData(result);
            }
            return;
        }
    }
}