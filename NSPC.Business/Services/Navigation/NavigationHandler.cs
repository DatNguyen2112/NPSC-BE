using AutoMapper;
using NSPC.Common;
using NSPC.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NSPC.Data.Data;

namespace NSPC.Business
{
    public class NavigationHandler : INavigationHandler
    {
        private readonly SMDbContext _context;
        private readonly IMapper _mapper;

        public NavigationHandler(SMDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response> GetByUserIdAsync(Guid userId)
        {
            try
            {
                var userRoles = await _context.IdmUser.Where(x => x.Id == userId).Select(x => x.RoleListCode).FirstOrDefaultAsync();
                var query = (from navrole in _context.BsdNavigationMapRole 
                             
                             join nav in _context.BsdNavigation on navrole.NavigationId equals nav.Id
                             join role in _context.IdmRole on navrole.RoleId equals role.Id
                             where nav.Status == true && userRoles.Contains(role.Code)
                             select nav).Distinct().OrderBy(r => r.Order);

                var sRights = await query.ToListAsync();
                var rRights = new List<NavigationModel>();

                foreach (var sRight in sRights)
                    if (sRight.ParentId == null)
                        rRights.Add(GetNav(sRights, sRight));
                return new Response<List<NavigationModel>>(rRights);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@params}", userId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetByUserIdAsync(Guid userId, int? type)
        {
            try
            {
                var userRoles = await _context.IdmUser.Where(x => x.Id == userId).Select(x => x.RoleListCode).FirstOrDefaultAsync();
                var query = (from navrole in _context.BsdNavigationMapRole

                             join nav in _context.BsdNavigation on navrole.NavigationId equals nav.Id
                             join role in _context.IdmRole on navrole.RoleId equals role.Id
                             where nav.Status == true && userRoles.Contains(role.Code) && nav.Type == type
                             select nav).Distinct().OrderBy(r => r.Order);

                var sRights = await query.ToListAsync();
                var rRights = new List<NavigationModel>();

                foreach (var sRight in sRights)
                    if (sRight.ParentId == null)
                        rRights.Add(GetNav(sRights, sRight));
                return new Response<List<NavigationModel>>(rRights);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}, Types: {@type}", userId, type);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetTreeAsync(bool isGetRoles)
        {
            try
            {
                var rRights = new List<NavigationModel>();
                var sRights = await _context.BsdNavigation.AsQueryable().OrderBy(s => s.Order).ToListAsync();
                if (sRights != null && sRights.Count > 0)
                {
                    foreach (var sRight in sRights)
                        if (sRight.ParentId == null)
                            rRights.Add(await GetNavAsync(sRights, sRight, isGetRoles));
                    return new Response<List<NavigationModel>>(rRights);
                }

                return new Response<List<NavigationModel>>(null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: IsGetRoles: {@params}", isGetRoles);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> CreateAsync(NavigationCreateModel request, Guid userId)
        {
            try
            {
                var id = Guid.NewGuid();
                if (_context.BsdNavigation.Any(x => x.UrlRewrite == request.UrlRewrite))
                    return Helper.CreateBadRequestResponse("Link đã tồn tại");
                var model = new BsdNavigation
                {
                    Id = id,
                    Name = request.Name,
                    Code = request.Code,
                    Status = request.Status,
                    UrlRewrite = request.UrlRewrite,
                    IconClass = request.IconClass,
                    Order = request.Order,
                    SubUrl = request.SubUrl,
                    Type = request.Type,
                    IdPath = request.ParentModel != null
                        ? (request.ParentModel.IdPath == null ? id + "/" : request.ParentModel.IdPath + id + "/")
                        : id + "/",
                    Path = request.ParentModel != null
                        ? (request.ParentModel.Path == null
                            ? request.Name + "/"
                            : request.ParentModel.Path + request.Name + "/")
                        : request.Name + "/",
                    Level = request.ParentModel?.Level + 1 ?? 0,
                    QueryParams = request.QueryParams
                };

                if (request.ParentModel != null) model.ParentId = request.ParentModel.Id;

                //if (!string.IsNullOrEmpty(model.UrlRewrite) && model.UrlRewrite != "#/")
                //    if (CheckUrlRewrite(model.UrlRewrite, null))
                //        return new ResponseError(HttpStatusCode.BadRequest,
                //            $"Đường dẫn: {model.UrlRewrite} đã tồn tại");

                if (!string.IsNullOrEmpty(model.Code))
                    if (Checkcode(model.Code, null))
                        return new ResponseError(HttpStatusCode.BadRequest, $"Mã: {model.Code} đã tồn tại");

                _context.BsdNavigation.Add(model);

                var parentItem = await _context.BsdNavigation.AsQueryable().Where(s => s.Id == model.ParentId).FirstOrDefaultAsync();

                if (parentItem != null)
                {
                    parentItem.HasChild = true;
                    _context.BsdNavigation.Update(parentItem);
                }

                if (await _context.SaveChangesAsync() > 0)
                {
                    if (request.RoleList != null && request.RoleList.Any())
                    {
                        await UpdateRoleInNavigations(model.Id, request.RoleList.ToList());
                    }
                    return new Response<NavigationModel>(new NavigationModel
                    {
                        Id = model.Id,
                        Name = model.Name,
                        ParentId = model.ParentId,
                        Code = model.Code,
                        UrlRewrite = model.UrlRewrite,
                        Order = model.Order,
                        Status = model.Status,
                        SubUrl = model.SubUrl,
                        QueryParams = model.QueryParams
                    });
                }

                Log.Error("The sql statement is not executed!");
                return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Request: {@request}, UserIds: {@userId}", request, userId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdateAsync(Guid navigationId, NavigationUpdateModel request, Guid userId)
        {
            try
            {
                var navDbset = _context.BsdNavigation;
                var currentNav = await navDbset.Where(x => x.Id == navigationId).FirstOrDefaultAsync();
                if (currentNav == null)
                {
                    Log.Error($"{navigationId} is not found");
                    return new ResponseError(HttpStatusCode.BadRequest, "Id không tồn tại");
                }

                var childList = await GetListChildNav(navigationId);
                //kiem tra nav cha khong dk la chinh no hoac con no
                childList.Add(navigationId);
                foreach (var id in childList)
                    if (request.ParentModel != null)
                        if (id == request.ParentModel.Id)
                            return new ResponseError(HttpStatusCode.BadRequest,
                                "Không được chọn điều hướng cha là điều hướng con của điều hướng hiện tại");
                if (!string.IsNullOrEmpty(request.Code))
                    if (Checkcode(request.Code, navigationId))
                        return new ResponseError(HttpStatusCode.BadRequest, $"Mã: {request.Code} đã tồn tại");
                currentNav.Name = request.Name;
                currentNav.UrlRewrite = request.UrlRewrite;
                currentNav.IconClass = request.IconClass;
                currentNav.Status = request.Status;
                currentNav.Order = request.Order;
                currentNav.Code = request.Code;
                currentNav.SubUrl = request.SubUrl;
                currentNav.QueryParams = request.QueryParams;
                if (request.ParentModel == null)
                {
                    currentNav.ParentId = null;
                    currentNav.IdPath = currentNav.Id + "/";
                    currentNav.Path = request.Name + "/";
                    currentNav.Level = 0;
                }
                else
                {
                    var parentNavId = request.ParentModel.Id;
                    if (parentNavId != currentNav.ParentId)
                    {
                        var parentItem = await navDbset.Where(s => s.Id == parentNavId).FirstOrDefaultAsync();
                        if (parentItem != null)
                        {
                            parentItem.HasChild = true;
                            navDbset.Update(parentItem);
                            currentNav.ParentId = request.ParentModel.Id;
                            currentNav.IdPath = request.ParentModel.IdPath + currentNav.Id + "/";
                            currentNav.Path = request.ParentModel.Path + request.Name + "/";
                            currentNav.Level = request.ParentModel.Level + 1;
                        }
                    }
                }

                navDbset.Update(currentNav);
                await UpdateNavChilds(currentNav, navDbset);
                //DELETE role old parent
                var navRoles = _context.BsdNavigationMapRole;

                //FromSubNavigation: từ các quyền lấy từ nav con => tìm các con
                var updateNavRoles = await navRoles
                    .Where(x => x.NavigationId == navigationId && x.FromSubNavigation != null)
                    .Select(x => x.FromSubNavigation).Distinct().ToListAsync();

                //lấy các quyền của nó cho cha nó để xóa đi
                var deleteNavRoles = await navRoles.Where(x => x.FromSubNavigation == navigationId).ToListAsync();

                if (updateNavRoles.Any())
                {
                    var listParent = await GetListParentNav(navigationId);
                    if (listParent.Count > 0)
                        foreach (var temp in updateNavRoles)
                            foreach (var parentId in listParent)
                            {
                                var uNavRoles2 = await navRoles.Where(x => x.FromSubNavigation == temp && x.NavigationId == parentId).ToListAsync();
                                deleteNavRoles = deleteNavRoles.Concat(uNavRoles2).ToList();
                            }
;
                }

                navRoles.RemoveRange(deleteNavRoles);

                if (await _context.SaveChangesAsync() >= 1)
                {
                    await MoveRole(currentNav.Id);
                    if (request.RoleList != null && request.RoleList.Any())
                    {
                        await UpdateRoleInNavigations(navigationId, request.RoleList.ToList());
                    }
                    return new ResponseUpdate(currentNav.Id);
                }

                Log.Error("The sql statement is not executed!");
                return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: NavigationId: {@navigationId}, Requests: {@request}, UserIds: {@userId}", navigationId, request, userId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> DeleteAsync(Guid navigationId)
        {
            try
            {
                var datas = _context.BsdNavigation;
                var dNav = await datas.Where(x => x.Id == navigationId).FirstOrDefaultAsync();
                if (dNav != null)
                {
                    //delete Role_Navigation
                    var navRoles = _context.BsdNavigationMapRole;
                    var listDeleteNavRoles = await navRoles.Where(s => s.NavigationId == dNav.Id || s.FromSubNavigation == dNav.Id).ToListAsync();
                    navRoles.RemoveRange(listDeleteNavRoles);
                    datas.Remove(dNav);
                }

                if (await _context.SaveChangesAsync() >= 1)
                    if (dNav != null)
                        return new ResponseDelete(dNav.Id, dNav.Name);

                Log.Error("The sql statement is not executed!");
                return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: NavigationId: {@params}", navigationId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdateRoleInNavigations(Guid navigationId, List<Guid> listRoleId)
        {
            try
            {
                // Lay ve nhom nguoi dung
                var sNav = await _context.BsdNavigation.Where(sp => sp.Id == navigationId).FirstOrDefaultAsync();
                if (sNav == null)
                {
                    Log.Error($"{navigationId} is not found");
                    return new ResponseError(HttpStatusCode.BadRequest, "Id không tồn tại");
                }

                var cNavRoleDbset = _context.BsdNavigationMapRole;
                //get all old roles of nav, tách những role bị trùng
                var oldRoles = await cNavRoleDbset.Where(o => o.NavigationId == navigationId).Select(s => s.RoleId).Distinct().ToListAsync();
                //new roles list
                var newRoles = listRoleId;
                //list all roles not changed
                var temporaryRoles = new List<Guid>();
                foreach (var oItem in oldRoles)
                    foreach (var nRoleId in newRoles)
                        if (oItem == nRoleId)
                        {
                            temporaryRoles.Add(oItem);
                            break;
                        }

                if (temporaryRoles.Count == newRoles.Count && newRoles.Count == oldRoles.Count)
                    return new ResponseUpdate(navigationId);
                //Remove roles unchanged
                foreach (var temp in temporaryRoles)
                {
                    oldRoles.Remove(temp);
                    newRoles.Remove(temp);
                }

                //delete old roles and add new roles
                foreach (var oItem in oldRoles)
                {
                    //vì ở trên select distinct nên phải get lại
                    var listNavRole = await cNavRoleDbset.Where(g => g.NavigationId == navigationId && g.RoleId == oItem).ToListAsync();

                    foreach (var item in listNavRole)
                        // xoa tat ca row an theo Fromsub = navId => các quyền cha kế thừa từ con
                        // xoa row co nav = fromsub =>
                        // xoa row fromsub = fromsub, xoa row
                        if (!string.IsNullOrEmpty(item.FromSubNavigation.ToString()))
                        {
                            var navRoles = await cNavRoleDbset.Where(rr =>
                                                            rr.FromSubNavigation == item.NavigationId && rr.RoleId == item.RoleId ||
                                                            rr.NavigationId == item.FromSubNavigation && rr.RoleId == item.RoleId ||
                                                            rr.FromSubNavigation == item.FromSubNavigation && rr.RoleId == item.RoleId ||
                                                            rr.Id == item.Id).ToListAsync();

                            cNavRoleDbset.RemoveRange(navRoles);
                        }
                        else
                        {
                            var navRoles = await cNavRoleDbset.Where(rr =>
                                                           rr.NavigationId == item.NavigationId && rr.RoleId == item.RoleId ||
                                                           rr.Id == item.Id).ToListAsync();

                            cNavRoleDbset.RemoveRange(navRoles);
                        }
                }

                if (newRoles.Count > 0)
                {
                    var listId = await GetListParentNav(navigationId);
                    foreach (var roleId in newRoles)
                    {
                        foreach (var item in listId)
                        {
                            var navR = new BsdNavigationMapRole
                            {
                                NavigationId = item,
                                RoleId = roleId,
                                FromSubNavigation = navigationId
                            };
                            cNavRoleDbset.Add(navR);
                        }

                        var navRole = new BsdNavigationMapRole
                        {
                            RoleId = roleId,
                            NavigationId = navigationId,
                        };
                        cNavRoleDbset.Add(navRole);
                    }
                }

                // Commit tat ca lenh
                if (await _context.SaveChangesAsync() >= 1) return new ResponseUpdate(navigationId);

                Log.Error("The sql statement is not executed!");
                return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: NavigationId: {@navigationOd}, ListRoleIds: {@listRoleId}", navigationId, listRoleId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        private async Task<NavigationModel> GetNavAsync(List<BsdNavigation> navs, BsdNavigation nav, bool isGetRoles)
        {
            var rNav = new NavigationModel
            {
                Id = nav.Id,
                ParentId = nav.ParentId,
                IconClass = nav.IconClass,
                Code = nav.Code,
                Name = nav.Name,
                Order = nav.Order,
                Status = nav.Status,
                UrlRewrite = nav.UrlRewrite,
                IdPath = nav.IdPath,
                Path = nav.Path,
                Level = nav.Level,
                SubUrl = nav.SubUrl,
                Type = nav.Type,
                SubChild = new List<NavigationModel>(),
                RoleList = new List<Guid>(),
                QueryParams = nav.QueryParams
            };

            if (isGetRoles)
            {
                var navR = await _context.BsdNavigationMapRole.Where(s => s.NavigationId == nav.Id).Select(s => s.RoleId).Distinct().ToListAsync();
                var rL = new List<Guid>();
                foreach (var item in navR) rL.Add(item);
                rNav.RoleList = rL;
            }

            foreach (var tNav in navs)
                if (tNav.ParentId == nav.Id)
                    rNav.SubChild.Add(await GetNavAsync(navs, tNav, isGetRoles));

            return rNav;
        }

        private async Task UpdateNavChilds(BsdNavigation parentNav, DbSet<BsdNavigation> navDbset)
        {
            try
            {
                var parentNavId = parentNav.Id;
                // 1. Lay ve danh sach tat ca cac nav con
                var childNavs = await navDbset.Where(x => x.ParentId == parentNavId).ToListAsync();

                foreach (var childNav in childNavs)
                {
                    childNav.ParentId = parentNav.Id;
                    childNav.Level = parentNav.Level + 1;
                    childNav.Path = parentNav.Path + childNav.Name + "/";
                    childNav.IdPath = parentNav.IdPath + childNav.Id + "/";
                    childNav.LastModifiedOnDate = DateTime.Now;

                    navDbset.Update(childNav);
                    // 2.1 Cap nhat duong dan tat ca nav con
                    await UpdateNavChilds(childNav, navDbset);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                throw;
            }
        }

        private async Task MoveRole(Guid navigationId)
        {
            try
            {
                var cNavRoleRepo = _context.BsdNavigationMapRole;
                var listParentsId = await GetListParentNav(navigationId);
                var listRole = await cNavRoleRepo.Where(u => u.NavigationId == navigationId).ToListAsync();
                foreach (var navRole in listRole)
                    foreach (var item in listParentsId)
                    {
                        var navR = new BsdNavigationMapRole
                        {
                            RoleId = navRole.RoleId,
                            NavigationId = item,
                            FromSubNavigation = navRole.FromSubNavigation ?? navRole.NavigationId
                        };
                        cNavRoleRepo.Add(navR);
                    }

                Log.Debug(await _context.SaveChangesAsync() >= 1 ? "Move role success!" : "Move role Error!");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
            }
        }

        private bool CheckUrlRewrite(string url, Guid? navigationId)
        {
            try
            {
                var data = from nav in _context.BsdNavigation
                           where
                               nav.UrlRewrite.ToLower() == url.ToLower()
                           select nav;
                if (navigationId != null)
                    data = from dt in data where dt.Id != navigationId select dt;
                if (data.Any())
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return false;
            }
        }

        private bool Checkcode(string code, Guid? navigationId)
        {
            try
            {
                var data = from nav in _context.BsdNavigation
                           where
                               nav.Code.ToLower() == code.ToLower()
                           select nav;
                if (navigationId != null)
                    data = from dt in data where dt.Id != navigationId select dt;
                if (data.Any())
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return false;
            }
        }

        private async Task<List<Guid>> GetListParentNav(Guid? navId)
        {
            var navIds = new List<Guid>();
            // Lay ve nhom nguoi dung
            var sNav = await _context.BsdNavigation.Where(sp => sp.Id == navId).FirstOrDefaultAsync();

            if (sNav?.ParentId != null)
            {
                navIds.Add(sNav.ParentId.Value);
                var navParentId = await GetListParentNav(sNav.ParentId);
                if (navParentId.Count > 0)
                    foreach (var item in navParentId)
                        navIds.Add(item);
            }

            return navIds;
        }

        private async Task<List<Guid>> GetListChildNav(Guid? navId)
        {
            var navIds = new List<Guid>();
            // Lay ve tat ca dieu huong con cua dh hien tai
            var sNav = await _context.BsdNavigation.Where(sp => sp.ParentId == navId).ToListAsync();

            if (sNav != null && sNav.Any())
                foreach (var item in sNav)
                {
                    navIds.Add(item.Id);
                    var navChildId = await GetListChildNav(item.Id);
                    if (navChildId.Count > 0)
                        foreach (var request in navChildId)
                            navIds.Add(request);
                }

            return navIds;
        }

        private static NavigationModel GetNav(List<BsdNavigation> navs, BsdNavigation nav)
        {
            var rNav = new NavigationModel
            {
                Id = nav.Id,
                ParentId = nav.ParentId,
                Code = nav.Code,
                Name = nav.Name,
                UrlRewrite = nav.UrlRewrite,
                Type = nav.Type,
                IconClass = nav.IconClass,
                SubChild = new List<NavigationModel>(),
                QueryParams = nav.QueryParams
            };
            //rNav.RoleList = Nav.NavigationRole.Select(s => s.RoleID.ToString()).ToArray();
            if (nav.HasChild)
                foreach (var tNav in navs)
                    if (tNav.ParentId == nav.Id)
                        rNav.SubChild.Add(GetNav(navs, tNav));
            return rNav;
        }
    }
}