using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.AssetCategories;
using Serilog;
using System.Linq.Expressions;

namespace NSPC.Business;

public class AssetTypeHandler : IAssetTypeHandler
{
    private readonly SMDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AssetTypeHandler(
        SMDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<AssetTypeViewModel>> Create(AssetTypeCreateUpdateModel model)
    {
        try
        {
            var validationResult = await ValidateCreateUpdateModel<AssetTypeViewModel>(model);
            if (validationResult != null)
            {
                return validationResult;
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var entity = _mapper.Map<sm_AssetType>(model);

            entity.Id = Guid.NewGuid();
            entity.CreatedByUserId = currentUser.UserId;
            entity.CreatedByUserName = currentUser.UserName;
            entity.CreatedOnDate = DateTime.Now;
            entity.LastModifiedByUserId = currentUser.UserId;
            entity.LastModifiedByUserName = currentUser.UserName;
            entity.LastModifiedOnDate = entity.CreatedOnDate;
            entity.TenantId = currentUser.TenantId;

            _dbContext.sm_AssetType.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Load related AssetGroup for mapping
            await _dbContext.Entry(entity)
                .Reference(x => x.AssetGroup)
                .LoadAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetTypeViewModel>(entity), "Tạo loại tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<AssetTypeViewModel>(e);
        }
    }

    public async Task<Response<AssetTypeViewModel>> Update(Guid id, AssetTypeCreateUpdateModel model)
    {
        try
        {
            var validationResult = await ValidateCreateUpdateModel<AssetTypeViewModel>(model, id);
            if (validationResult != null)
            {
                return validationResult;
            }

            var entity = await _dbContext.sm_AssetType.FindAsync(id);
            if (entity == null)
            {
                return Helper.CreateBadRequestResponse<AssetTypeViewModel>("Loại tài sản không tồn tại");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            entity.Code = model.Code;
            entity.Name = model.Name;
            entity.AssetGroupId = model.AssetGroupId;
            entity.Description = model.Description;
            entity.LastModifiedByUserId = currentUser.UserId;
            entity.LastModifiedByUserName = currentUser.UserName;
            entity.LastModifiedOnDate = DateTime.Now;

            _dbContext.sm_AssetType.Update(entity);
            await _dbContext.SaveChangesAsync();

            var result = await GetById(entity.Id);
            return result;
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<AssetTypeViewModel>(e);
        }
    }

    public async Task<Response<Pagination<AssetTypeViewModel>>> GetPage(AssetTypeQueryModel query)
    {
        try
        {
            var predicate = BuildQuery(query);
            var queryResult = _dbContext.sm_AssetType
                .Include(x => x.AssetGroup)
                .Where(predicate);

            var data = await queryResult.GetPageAsync(query);
            var result = _mapper.Map<Pagination<AssetTypeViewModel>>(data);

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Query: {@query}", query);
            return Helper.CreateExceptionResponse<Pagination<AssetTypeViewModel>>(e);
        }
    }

    public async Task<Response<AssetTypeViewModel>> GetById(Guid id)
    {
        try
        {
            var entity = await _dbContext.sm_AssetType
                .Include(x => x.AssetGroup)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return Helper.CreateBadRequestResponse<AssetTypeViewModel>("Loại tài sản không tồn tại");
            }

            return Helper.CreateSuccessResponse(_mapper.Map<AssetTypeViewModel>(entity));
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Id: {@id}", id);
            return Helper.CreateExceptionResponse<AssetTypeViewModel>(e);
        }
    }

    public async Task<Response> Delete(Guid id)
    {
        try
        {
            var entity = await _dbContext.sm_AssetType.FindAsync(id);
            if (entity == null)
            {
                return Helper.CreateBadRequestResponse("Loại tài sản không tồn tại");
            }

            // Check if any assets are using this type
            var hasAssets = await _dbContext.sm_Asset.AnyAsync(x => x.AssetTypeId == entity.Id);
            if (hasAssets)
            {
                return Helper.CreateBadRequestResponse("Không thể xóa loại tài sản đang được sử dụng");
            }

            _dbContext.sm_AssetType.Remove(entity);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse("Xóa loại tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Id: {@id}", id);
            return Helper.CreateExceptionResponse(e);
        }
    }

    private Expression<Func<sm_AssetType, bool>> BuildQuery(AssetTypeQueryModel query)
    {
        var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
        var predicate = PredicateBuilder.New<sm_AssetType>(true);

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

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            predicate.And(x => x.Code.ToLower().Contains(query.Code.Trim().ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.Name.Trim().ToLower()));
        }

        if (query.AssetGroupId.HasValue)
        {
            predicate.And(x => x.AssetGroupId == query.AssetGroupId);
        }

        return predicate;
    }

    private async Task<Response<T>> ValidateCreateUpdateModel<T>(AssetTypeCreateUpdateModel model, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(model.Code))
        {
            return Helper.CreateBadRequestResponse<T>("Mã loại tài sản không được để trống");
        }

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            return Helper.CreateBadRequestResponse<T>("Tên loại tài sản không được để trống");
        }

        // Kiểm tra nhóm tài sản tồn tại
        var assetGroup = await _dbContext.sm_AssetGroup.FindAsync(model.AssetGroupId);
        if (assetGroup == null)
        {
            return Helper.CreateBadRequestResponse<T>("Nhóm tài sản không tồn tại");
        }

        // Kiểm tra trùng mã
        var existingType = await _dbContext.sm_AssetType
            .FirstOrDefaultAsync(x => x.Code == model.Code);

        if (existingType != null && (id == null || existingType.Id != id))
        {
            return Helper.CreateBadRequestResponse<T>($"Mã loại tài sản {model.Code} đã tồn tại");
        }

        return null;
    }
}