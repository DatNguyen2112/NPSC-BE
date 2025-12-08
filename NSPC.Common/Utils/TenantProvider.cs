using Microsoft.AspNetCore.Http;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Common
{
    public interface ITenantProvider
    {
        Guid? GetTenantId();
    }

    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
           _httpContextAccessor = httpContextAccessor;
        }
        public Guid? GetTenantId()
        {
            var requestUser = Helper.GetRequestInfo(_httpContextAccessor);
            Guid? tenantId = null;
            if (requestUser != null)
                tenantId = requestUser.TenantId;

            return tenantId;
        }
    }
}
