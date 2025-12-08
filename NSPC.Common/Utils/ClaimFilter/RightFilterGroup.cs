using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Common
{
    public class RightValidateGroupAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly List<string> _rights;

        /// <summary>
        /// Initializes with a list of rights in the format "group.action".
        /// </summary>
        /// <param name="rights">A list of rights in the format "group.action".</param>
        public RightValidateGroupAttribute(params string[] rights)
        {
            // Store the rights as a list of strings, where each string is "group.action".
            _rights = rights.ToList();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var currentClaim = Helper.GetRequestInfo(context.HttpContext.Request);

            // Nếu user là Admin, bỏ qua kiểm tra quyền
            if (currentClaim.ListRoles.Contains(RoleConstants.AdminRoleCode))
                return;

            // Kiểm tra quyền trong danh sách
            foreach (var right in _rights)
            {
                // Nếu nó chứa thi dung vong lap
                if (currentClaim.ListRights.Contains(right))
                {
                    return;
                }
            }
            
            // neu co thi tra message error
            var result = new ResponseError(HttpStatusCode.Forbidden, $"Bạn không có quyền truy cập chức năng");
            context.Result = TransformData(result);
        }
    }
}