using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public class RightMapRoleHandler : IRightMapRoleHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;

        public RightMapRoleHandler(SMDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        public async Task<Response<List<RightMapRoleViewModel>>> GetRightsInRole(Guid roleId)
        {
            try
            {
                if (!_dbContext.IdmRole.Any(x => x.Id == roleId))
                    return Helper.CreateNotFoundResponse<List<RightMapRoleViewModel>>("Role không tồn tại");

                var rightMapRole = await _dbContext.IdmRightMapRole.AsNoTracking().Include(x => x.Right)
                    .Where(x => x.RoleId == roleId).ToListAsync();

                var result = _mapper.Map<List<RightMapRoleViewModel>>(rightMapRole);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<List<RightMapRoleViewModel>>(ex);
            }
        }

        public async Task<Response> AddRightToRole(Guid roleId, List<Guid> rightIds)
        {
            try
            {
                var role = await _dbContext.IdmRole.AsNoTracking().FirstOrDefaultAsync(x => x.Id == roleId);
                if (role == null)
                    return Helper.CreateNotFoundResponse("Role không tồn tại");

                var rightList = await _dbContext.IdmRight.AsNoTracking().Where(x => rightIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var item in rightList)
                {
                    var rightMapRole = new IdmRightMapRole
                    {
                        RoleId = roleId,
                        RoleCode = role.Code,
                        RightId = item.Id,
                        RightCode = item.Code
                    };
                    _dbContext.Add(rightMapRole);
                }

                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse("Thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse(ex);
            }
        }

        public async Task<Response> RemoveRightFromRole(Guid roleId, List<Guid> rightIds)
        {
            try
            {
                if (!_dbContext.IdmRole.Any(x => x.Id == roleId))
                    return Helper.CreateNotFoundResponse("Role không tồn tại");
                var rightMapRole = await _dbContext.IdmRightMapRole
                    .Where(x => x.RoleId == roleId && rightIds.Contains(x.RightId.Value)).ToListAsync();
                if (rightMapRole.Any())
                {
                    _dbContext.RemoveRange(rightMapRole);
                    await _dbContext.SaveChangesAsync();
                }

                return Helper.CreateSuccessResponse("Thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse(ex);
            }
        }

        public async Task<Response<Dictionary<string, List<string>>>> GetRightMapConfig(string groupCode)
        {
            try
            {
                var rightMapConfig = await _dbContext.IdmRightMapRole
                    .AsNoTracking()
                    .Where(x => x.Right.GroupCode == groupCode)
                    .ToListAsync();

                var result = rightMapConfig
                    .GroupBy(x => x.RoleCode)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.RightCode).ToList());
                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<Dictionary<string, List<string>>>(ex);
            }
        }

        public Response<RightMapConfigViewModel> GetRightMapConfigByCode(string groupCode)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var result = new RightMapConfigViewModel();
                result.GroupCode = groupCode;
                result.RightCodes = currentUser.ListRights
                    .Where(x => x.StartsWith(groupCode + "."))
                    .Select(x => x.Split('.').Last())
                    .ToList();

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<RightMapConfigViewModel>(ex);
            }
        }

        public Response<List<RightMapConfigViewModel>> GetRightMapConfigByListCode(List<string> groupCodes)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var result = new List<RightMapConfigViewModel>();
                foreach (var groupCode in groupCodes)
                {
                    result.Add(new RightMapConfigViewModel
                    {
                        GroupCode = groupCode,
                        RightCodes = currentUser.ListRights
                            .Where(x => x.StartsWith(groupCode + "."))
                            .Select(x => x.Split('.').Last())
                            .ToList()
                    });
                }

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<List<RightMapConfigViewModel>>(ex);
            }
        }

        public async Task<Response> ConfigRightMap(string groupCode,
            List<RightMapRoleAssignModel> rightMapRoleAssignModels)
        {
            try
            {
                var rights = await _dbContext.IdmRight
                    .Where(x => x.GroupCode == groupCode)
                    .ToDictionaryAsync(x => x.Id, x => x.Code);
                var roleIds = rightMapRoleAssignModels.Select(x => x.RoleId).ToList();
                var roleCodeMap = await _dbContext.IdmRole
                    .Where(x => roleIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.Code);
                var rightMapRoles = await _dbContext.IdmRightMapRole
                    .Where(x => roleIds.Contains(x.RoleId.Value) && x.Right.GroupCode == groupCode)
                    .ToListAsync();
                var rolesToInvalidateCache = new HashSet<string>();

                foreach (var rightMapRoleAssignModel in rightMapRoleAssignModels)
                {
                    var singleRoleRights = rightMapRoles
                        .Where(x => x.RoleId == rightMapRoleAssignModel.RoleId)
                        .ToList();
                    var currentRightIds = singleRoleRights
                        .Select(x => x.RightId!.Value)
                        .ToList();
                    var addingRightIds = rightMapRoleAssignModel.RightIds
                        .Where(x => !currentRightIds.Contains(x))
                        .ToList();
                    var removingRights = singleRoleRights
                        .Where(x => !rightMapRoleAssignModel.RightIds.Contains(x.RightId!.Value))
                        .ToList();

                    if (addingRightIds.Count == 0 && removingRights.Count == 0)
                    {
                        continue;
                    }

                    foreach (var addingRightId in addingRightIds)
                    {
                        _dbContext.IdmRightMapRole.Add(new IdmRightMapRole()
                        {
                            RightId = addingRightId,
                            RightCode = rights[addingRightId],
                            RoleId = rightMapRoleAssignModel.RoleId,
                            RoleCode = roleCodeMap[rightMapRoleAssignModel.RoleId]
                        });
                    }

                    _dbContext.IdmRightMapRole.RemoveRange(removingRights);
                    rolesToInvalidateCache.Add(roleCodeMap[rightMapRoleAssignModel.RoleId]);
                }

                await _dbContext.SaveChangesAsync();

                var userIdToInvalidateCache = await _dbContext.IdmUser
                    .Where(x => rolesToInvalidateCache.ToList().Any(y => x.RoleListCode.Contains(y)))
                    .Select(x => x.Id)
                    .ToListAsync();
                foreach (var userId in userIdToInvalidateCache)
                {
                    _memoryCache.Remove(userId);
                }

                return Helper.CreateSuccessResponse("Thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse(ex);
            }
        }

        public async Task<Response<bool>> PermissionCheck(List<string> roleCodeList, string group, string rightCode)
        {
            try
            {
                var rightMapRole = await _dbContext.IdmRightMapRole
                    .AsNoTracking()
                    .Include(x => x.Right)
                    .Where(x => roleCodeList.Contains(x.RoleCode) && x.RightCode == rightCode &&
                                x.Right.GroupCode == group)
                    .ToListAsync();
                if (rightMapRole.Any())
                    return Helper.CreateSuccessResponse(true);
                else return Helper.CreateSuccessResponse(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<bool>(ex);
            }
        }
    }
}