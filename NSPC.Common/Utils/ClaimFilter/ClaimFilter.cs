using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using NSPC.Common;
using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Common
{
    public class AllowAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _roles = null;
        private readonly string _app = null;

        public AllowAttribute(string roles)
        {
            _roles = roles;
        }

        public AllowAttribute(string role1, string role2)
        {
            _roles = role1;
            if (!string.IsNullOrEmpty(role2))
                _roles = _roles + "," + role2;
        }

        public AllowAttribute(string role1, string role2, string role3)
        {
            _roles = role1;
            if (!string.IsNullOrEmpty(role2))
                _roles = _roles + "," + role2;

            if (!string.IsNullOrEmpty(role3))
                _roles = _roles + "," + role3;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            RequestUser currentClaim = Helper.GetRequestInfo(context.HttpContext.Request);

            var currentRole = NSPCConstants.RoleLevelDict.Where(x => currentClaim.ListRoles.Contains(x.Key))
                   .Select(x => x.Key)
                   .ToList();
            if (currentRole != null && currentRole.Count() > 0)
            {
                var listRole = _roles.Split(',').ToList();
                if (listRole.Any(x => currentRole.Any(z => z.Trim() == x.Trim())))
                {
                    return;
                }
            };

            //if (!string.IsNullOrEmpty(_righs))
            //{
            //    var listRight = _righs.Split(',');
            //    if (currentClaim.ListRights.Any(x => listRight.Any(z => z.Trim() == x.Trim())))
            //    {
            //        return;
            //    }
            //}

            //if (!string.IsNullOrEmpty(_app))
            //{
            //    if (currentClaim.ListApps.Any(x => x == _app))
            //    {
            //        return;
            //    }
            //}

            if (string.IsNullOrEmpty(_roles))
            {
                return;
            }

            if (currentClaim.Level == 100)
            {
                return;
            }
            var result = new Response(HttpStatusCode.Forbidden, "Bạn không có quyền truy cập chức năng này.");

            context.Result = Helper.TransformData(result);
        }
    }
}