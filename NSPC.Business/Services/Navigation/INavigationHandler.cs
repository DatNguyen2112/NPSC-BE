using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface INavigationHandler
    {
        Task<Response> GetByUserIdAsync(Guid userId);

        Task<Response> GetByUserIdAsync(Guid userId, int? type);

        Task<Response> GetTreeAsync(bool isGetRoles);

        Task<Response> CreateAsync(NavigationCreateModel request, Guid userId);

        Task<Response> UpdateAsync(Guid navigationId, NavigationUpdateModel request, Guid userId);

        Task<Response> DeleteAsync(Guid navigationId);

        Task<Response> UpdateRoleInNavigations(Guid navigationId, List<Guid> listRoleId);
    }
}