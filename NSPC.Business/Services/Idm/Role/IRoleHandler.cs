using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface IRoleHandler
    {
        Task<Response> CreateAsync(RoleCreateModel model, Guid? appId, Guid? actorId);

        Task<Response> UpdateAsync(Guid id, RoleUpdateModel model, Guid? appId, Guid? actorId);

        Task<Response> DeleteAsync(Guid id);

        Task<Response> DeleteRangeAsync(List<Guid> listId);

        Task<Response> FindAsync(Guid id);

        Task<Response> GetDetail(Guid id, Guid applicationId);

        Task<Response> GetPageAsync(RoleQueryModel query);

        Task<Response> GetAllAsync(RoleQueryModel query);

        Task<Response> GetAllAsync();

        Response GetAll();

        #region using dbContext
        Task<Response> GetPageAsync2(RoleQueryModel query);
        #endregion using dbContext;
    }
}