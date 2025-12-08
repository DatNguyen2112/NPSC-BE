using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business
{
    public interface ITenantHandler
    {
        Task<Response<TenantModel>> Create(TenantCreateModel model, RequestUser requestUser);
        Task<Response<TenantModel>> Update(Guid id, TenantUpdateModel model, RequestUser requestUser);
        Task<Response> Delete(Guid id, RequestUser requestUser);
        Task<Response<TenantModel>> GetById(Guid id, RequestUser requestUser);
        Task<Response<Pagination<TenantModel>>> GetPageAsync(TenantQueryModel query, RequestUser requestUser);
        Task<Response<TenantModel>> RegisterTenant(TenantCreateModel model, RequestUser requestUser);
        Task<Response> CheckDomainExists(string domain);

        // lấy ra dữ liệu của tenant theo domain
        Task<Response<TenantModel>> GetByDomain(string domain);
    }
}
