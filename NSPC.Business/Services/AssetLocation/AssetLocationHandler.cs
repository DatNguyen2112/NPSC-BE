using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.AssetLocation;
using Serilog;
using System.Linq.Expressions;
using NSPC.Data.Data.Entity.PhongBan;

namespace NSPC.Business;

public class AssetLocationHandler : IAssetLocationHandler
{
    private readonly SMDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AssetLocationHandler(
        SMDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<AssetLocationViewModel>> Create(AssetLocationCreateUpdateModel model)
    {
        try
        {
            var (validationResult, parent, managementUnit) =
                await ValidateCreateUpdateModel<AssetLocationViewModel>(model);

            if (validationResult != null)
            {
                return validationResult;
            }

            // Check if code already exists
            var existingCode = await _dbContext.sm_AssetLocation
                .AnyAsync(x => x.Code == model.Code);

            if (existingCode)
            {
                return Helper.CreateBadRequestResponse<AssetLocationViewModel>("Mã vị trí đã tồn tại");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var location = _mapper.Map<sm_AssetLocation>(model);

            location.Id = Guid.NewGuid();
            location.ParentId = parent?.Id;
            location.Parent = parent;
            location.CreatedOnDate = DateTime.Now;
            location.CreatedByUserId = currentUser.UserId;
            location.CreatedByUserName = currentUser.UserName;
            location.LastModifiedOnDate = location.CreatedOnDate;
            location.LastModifiedByUserId = location.CreatedByUserId;
            location.LastModifiedByUserName = location.CreatedByUserName;

            _dbContext.sm_AssetLocation.Add(location);
            await _dbContext.SaveChangesAsync();

            var result = _mapper.Map<AssetLocationViewModel>(location);

            return Helper.CreateSuccessResponse(result, "Tạo thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<AssetLocationViewModel>(e);
        }
    }

    public async Task<Response<AssetLocationViewModel>> Update(Guid id, AssetLocationCreateUpdateModel model)
    {
        try
        {
            var (validationResult, parent, managementUnit) =
                await ValidateCreateUpdateModel<AssetLocationViewModel>(model);

            if (validationResult != null)
            {
                return validationResult;
            }

            var location = await _dbContext.sm_AssetLocation
                .Include(x => x.Parent)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (location == null)
            {
                return Helper.CreateBadRequestResponse<AssetLocationViewModel>("Vị trí tài sản không tồn tại");
            }

            // Check if code already exists (excluding current location)
            var existingCode = await _dbContext.sm_AssetLocation
                .AnyAsync(x => x.Code == model.Code && x.Id != id);

            if (existingCode)
            {
                return Helper.CreateBadRequestResponse<AssetLocationViewModel>("Mã vị trí đã tồn tại");
            }

            // Check for circular reference
            if (model.ParentId.HasValue && await HasCircularReference(id, model.ParentId.Value))
            {
                return Helper.CreateBadRequestResponse<AssetLocationViewModel>(
                    "Không thể chọn vị trí con làm vị trí cha");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            _mapper.Map(model, location);
            location.ParentId = parent?.Id;
            location.Parent = parent;
            location.LastModifiedOnDate = DateTime.Now;
            location.LastModifiedByUserId = currentUser.UserId;
            location.LastModifiedByUserName = currentUser.UserName;

            _dbContext.sm_AssetLocation.Update(location);
            await _dbContext.SaveChangesAsync();

            var updatedLocation = _mapper.Map<AssetLocationViewModel>(location);

            return Helper.CreateSuccessResponse(updatedLocation, "Cập nhật thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<AssetLocationViewModel>(e);
        }
    }

    public async Task<Response<Pagination<AssetLocationViewModel>>> GetPage(AssetLocationQueryModel query)
    {
        try
        {
            var predicate = BuildQuery(query);
            var queryResult = _dbContext.sm_AssetLocation
                .Include(x => x.Parent)
                .Include(x => x.Children)
                .Where(predicate);

            var data = await queryResult.GetPageAsync(query);
            var result = _mapper.Map<Pagination<AssetLocationViewModel>>(data);

            // Build tree structure within pagination
            var rootItems = result.Content.Where(x => !x.ParentId.HasValue).ToList();
            foreach (var root in rootItems)
            {
                BuildLocationTree(root, result.Content);
            }

            result.Content = rootItems;

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Query: {@query}", query);
            return Helper.CreateExceptionResponse<Pagination<AssetLocationViewModel>>(e);
        }
    }

    public async Task<Response<AssetLocationViewModel>> GetById(Guid id)
    {
        try
        {
            var location = await _dbContext.sm_AssetLocation
                .Include(x => x.Parent)
                .Include(x => x.Children)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (location == null)
            {
                return Helper.CreateBadRequestResponse<AssetLocationViewModel>("Vị trí tài sản không tồn tại");
            }

            var result = _mapper.Map<AssetLocationViewModel>(location);

            // Build tree structure for children
            BuildLocationTree(result, await _dbContext.sm_AssetLocation
                .Select(x => _mapper.Map<AssetLocationViewModel>(x))
                .ToListAsync());

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Id: {@id}", id);
            return Helper.CreateExceptionResponse<AssetLocationViewModel>(e);
        }
    }

    public async Task<Response> Delete(Guid id)
    {
        try
        {
            var location = await _dbContext.sm_AssetLocation
                .Include(x => x.Children)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (location == null)
            {
                return Helper.CreateNotFoundResponse("Vị trí tài sản không tồn tại");
            }

            if (location.Children?.Any() == true)
            {
                return Helper.CreateBadRequestResponse("Không thể xóa vị trí đang có vị trí con");
            }

            // Check if location is being used by any assets
            var hasAssets = await _dbContext.sm_Asset
                .AnyAsync(x => x.AssetLocationId == id);

            if (hasAssets)
            {
                return Helper.CreateBadRequestResponse("Không thể xóa vị trí đang được sử dụng bởi tài sản");
            }

            _dbContext.sm_AssetLocation.Remove(location);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse("Xóa vị trí tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Id: {@id}", id);
            return Helper.CreateExceptionResponse(e);
        }
    }

    private async Task<(Response<T>, sm_AssetLocation, mk_PhongBan)> ValidateCreateUpdateModel<T>(
        AssetLocationCreateUpdateModel model)
    {
        Response<T> response = null;

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            response = Helper.CreateBadRequestResponse<T>("Tên vị trí không được để trống");
        }

        if (response == null && string.IsNullOrWhiteSpace(model.Code))
        {
            response = Helper.CreateBadRequestResponse<T>("Mã vị trí không được để trống");
        }

        sm_AssetLocation parent = null;
        mk_PhongBan managementUnit = null;

        if (response == null && model.ParentId.HasValue)
        {
            parent = await _dbContext.sm_AssetLocation
                .FirstOrDefaultAsync(x => x.Id == model.ParentId);

            if (parent == null)
            {
                response = Helper.CreateBadRequestResponse<T>("Vị trí cha không tồn tại");
            }
        }

        return (response, parent, managementUnit);
    }

    private Expression<Func<sm_AssetLocation, bool>> BuildQuery(AssetLocationQueryModel query)
    {
        var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
        var predicate = PredicateBuilder.New<sm_AssetLocation>(true);

        if (currentUser.TenantId != null)
        {
            predicate.And(x => x.TenantId == currentUser.TenantId);
        }

        if (!string.IsNullOrWhiteSpace(query.FullTextSearch))
        {
            predicate.And(x =>
                x.Name.ToLower().Contains(query.FullTextSearch.Trim().ToLower()) ||
                x.Code.ToLower().Contains(query.FullTextSearch.Trim().ToLower()));
        }

        if (query.ParentId.HasValue)
        {
            predicate.And(x => x.ParentId == query.ParentId);
        }

        return predicate;
    }

    private void BuildLocationTree(AssetLocationViewModel parent, List<AssetLocationViewModel> allLocations)
    {
        parent.Children = allLocations
            .Where(x => x.ParentId == parent.Id)
            .ToList();

        foreach (var child in parent.Children)
        {
            BuildLocationTree(child, allLocations);
        }
    }

    private async Task<bool> HasCircularReference(Guid currentId, Guid newParentId)
    {
        var parent = await _dbContext.sm_AssetLocation.FindAsync(newParentId);

        while (parent != null)
        {
            if (parent.Id == currentId)
            {
                return true;
            }

            parent = parent.ParentId.HasValue
                ? await _dbContext.sm_AssetLocation.FindAsync(parent.ParentId)
                : null;
        }

        return false;
    }
}