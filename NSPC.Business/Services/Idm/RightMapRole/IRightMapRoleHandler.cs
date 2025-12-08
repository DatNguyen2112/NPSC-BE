using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface IRightMapRoleHandler
    {
        Task<Response<List<RightMapRoleViewModel>>> GetRightsInRole(Guid roleId);
        Task<Response> AddRightToRole(Guid roleId, List<Guid> rightIds);
        Task<Response> RemoveRightFromRole(Guid roleId, List<Guid> rightIds);
        Task<Response<bool>> PermissionCheck(List<string> roleCodeList, string group, string rightCode);
        Task<Response<Dictionary<string, List<string>>>> GetRightMapConfig(string groupCode);
        Task<Response> ConfigRightMap(string groupCode, List<RightMapRoleAssignModel> rightMapRoleAssignModels);
        Response<RightMapConfigViewModel> GetRightMapConfigByCode(string groupCode);
        Response<List<RightMapConfigViewModel>> GetRightMapConfigByListCode(List<string> groupCodes);
    }
}