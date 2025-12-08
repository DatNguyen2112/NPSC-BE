using System.Linq.Expressions;
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.AssetCategories;
using Serilog;

namespace NSPC.Business;

public class AssetGroupHandler : IAssetGroupHandler
{
    private readonly SMDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AssetGroupHandler(
        SMDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<AssetGroupViewModel>> Create(AssetGroupCreateUpdateModel model)
    {
        try
        {
            var validationResult = await ValidateCreateUpdateModel<AssetGroupViewModel>(model);
            if (validationResult != null)
            {
                return validationResult;
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var entity = _mapper.Map<sm_AssetGroup>(model);

            entity.Id = Guid.NewGuid();
            entity.CreatedByUserId = currentUser.UserId;
            entity.CreatedByUserName = currentUser.UserName;
            entity.CreatedOnDate = DateTime.Now;
            entity.LastModifiedByUserId = currentUser.UserId;
            entity.LastModifiedByUserName = currentUser.UserName;
            entity.LastModifiedOnDate = entity.CreatedOnDate;
            entity.TenantId = currentUser.TenantId;

            _dbContext.sm_AssetGroup.Add(entity);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetGroupViewModel>(entity),
                "Tạo nhóm tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<AssetGroupViewModel>(e);
        }
    }

    public async Task<Response<AssetGroupViewModel>> Update(Guid id, AssetGroupCreateUpdateModel model)
    {
        try
        {
            var validationResult = await ValidateCreateUpdateModel<AssetGroupViewModel>(model, id);
            if (validationResult != null)
            {
                return validationResult;
            }

            var entity = await _dbContext.sm_AssetGroup.FindAsync(id);
            if (entity == null)
            {
                return Helper.CreateBadRequestResponse<AssetGroupViewModel>("Nhóm tài sản không tồn tại");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            entity.Code = model.Code;
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.LastModifiedByUserId = currentUser.UserId;
            entity.LastModifiedByUserName = currentUser.UserName;
            entity.LastModifiedOnDate = DateTime.Now;

            _dbContext.sm_AssetGroup.Update(entity);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetGroupViewModel>(entity),
                "Cập nhật nhóm tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<AssetGroupViewModel>(e);
        }
    }

    public async Task<Response<Pagination<AssetGroupViewModel>>> GetPage(AssetGroupQueryModel query)
    {
        try
        {
            var predicate = BuildQuery(query);
            var queryResult = _dbContext.sm_AssetGroup.Where(predicate);
            var data = await queryResult.GetPageAsync(query);
            var result = _mapper.Map<Pagination<AssetGroupViewModel>>(data);

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Query: {@query}", query);
            return Helper.CreateExceptionResponse<Pagination<AssetGroupViewModel>>(e);
        }
    }

    public async Task<Response<AssetGroupViewModel>> GetById(Guid id)
    {
        try
        {
            var entity = await _dbContext.sm_AssetGroup.FindAsync(id);
            if (entity == null)
            {
                return Helper.CreateBadRequestResponse<AssetGroupViewModel>("Nhóm tài sản không tồn tại");
            }

            return Helper.CreateSuccessResponse(_mapper.Map<AssetGroupViewModel>(entity));
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Id: {@id}", id);
            return Helper.CreateExceptionResponse<AssetGroupViewModel>(e);
        }
    }

    public async Task<Response> Delete(Guid id)
    {
        try
        {
            var entity = await _dbContext.sm_AssetGroup.FindAsync(id);
            if (entity == null)
            {
                return Helper.CreateBadRequestResponse("Nhóm tài sản không tồn tại");
            }

            var hasAssetTypes = await _dbContext.sm_AssetType.AnyAsync(x => x.AssetGroupId == id);
            if (hasAssetTypes)
            {
                return Helper.CreateBadRequestResponse("Không thể xóa nhóm tài sản đã có loại tài sản");
            }

            _dbContext.sm_AssetGroup.Remove(entity);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse("Xóa nhóm tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Id: {@id}", id);
            return Helper.CreateExceptionResponse(e);
        }
    }

    private Expression<Func<sm_AssetGroup, bool>> BuildQuery(AssetGroupQueryModel query)
    {
        var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
        var predicate = PredicateBuilder.New<sm_AssetGroup>(true);

        if (currentUser.TenantId != null)
        {
            predicate.And(x => x.TenantId == currentUser.TenantId);
        }

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            predicate.And(x => x.Code.ToLower().Contains(query.Code.Trim().ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.Name.Trim().ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(query.FullTextSearch))
        {
            predicate.And(x =>
                x.Code.ToLower().Contains(query.FullTextSearch.Trim().ToLower()) ||
                x.Name.ToLower().Contains(query.FullTextSearch.Trim().ToLower()));
        }

        return predicate;
    }

    private async Task<Response<T>> ValidateCreateUpdateModel<T>(AssetGroupCreateUpdateModel model, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(model.Code))
        {
            return Helper.CreateBadRequestResponse<T>("Mã nhóm tài sản không được để trống");
        }

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            return Helper.CreateBadRequestResponse<T>("Tên nhóm tài sản không được để trống");
        }

        // Kiểm tra trùng mã
        var existingGroup = await _dbContext.sm_AssetGroup
            .FirstOrDefaultAsync(x => x.Code == model.Code);

        if (existingGroup != null && (id == null || existingGroup.Id != id))
        {
            return Helper.CreateBadRequestResponse<T>($"Mã nhóm tài sản {model.Code} đã tồn tại");
        }

        return null;
    }
}