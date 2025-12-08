using System.Linq.Expressions;
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using NSPC.Common;
using Microsoft.EntityFrameworkCore;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.Asset;
using NSPC.Data.Data.Entity.AssetHistory;
using Serilog;

namespace NSPC.Business.AssetLiquidationSheet;

public class AssetLiquidationSheetHandler : IAssetLiquidationSheetHandler
{
    private readonly SMDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AssetLiquidationSheetHandler(
        SMDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<AssetLiquidationSheetViewModel>> Create(AssetLiquidationSheetCreateUpdateModel model)
    {
        try
        {
            var (response, asset) = await ValidateCreateUpdateModel<AssetLiquidationSheetViewModel>(model);

            if (response != null)
            {
                return response;
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var assetLiquidationSheet = _mapper.Map<sm_AssetLiquidationSheet>(model);

            // Update asset status to Liquidated
            asset.Status = AssetStatus.Liquidated;
            asset.LastModifiedOnDate = DateTime.Now;
            asset.LastModifiedByUserId = currentUser.UserId;
            asset.LastModifiedByUserName = currentUser.UserName;

            // Create liquidation sheet
            assetLiquidationSheet.Id = Guid.NewGuid();
            assetLiquidationSheet.CreatedOnDate = DateTime.Now;
            assetLiquidationSheet.CreatedByUserId = currentUser.UserId;
            assetLiquidationSheet.CreatedByUserName = currentUser.UserName;
            assetLiquidationSheet.LastModifiedOnDate = assetLiquidationSheet.CreatedOnDate;
            assetLiquidationSheet.LastModifiedByUserId = assetLiquidationSheet.CreatedByUserId;
            assetLiquidationSheet.LastModifiedByUserName = assetLiquidationSheet.CreatedByUserName;
            assetLiquidationSheet.TenantId = currentUser.TenantId;

            // Create asset usage history
            var usageHistory = new sm_AssetUsageHistory
            {
                Id = Guid.NewGuid(),
                AssetId = model.AssetId,
                Operation = AssetBusinessOperation.Liquidate,
                AssetStatus = asset.Status,
                ExecutionDate = model.LiquidationDate,
                Description = model.LiquidationReason,
                Cost = model.LiquidationValue,
                EntityId = assetLiquidationSheet.Id,
                EntityType = "AssetLiquidationSheet",
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                CreatedOnDate = DateTime.Now,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                LastModifiedOnDate = DateTime.Now,
                TenantId = currentUser.TenantId
            };

            _dbContext.sm_AssetLiquidationSheet.Add(assetLiquidationSheet);
            _dbContext.sm_AssetUsageHistory.Add(usageHistory);
            _dbContext.sm_Asset.Update(asset);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetLiquidationSheetViewModel>(assetLiquidationSheet),
                "Tạo phiếu thanh lý thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<AssetLiquidationSheetViewModel>(e);
        }
    }

    public async Task<Response<AssetLiquidationSheetViewModel>> Update(Guid id,
        AssetLiquidationSheetCreateUpdateModel model)
    {
        try
        {
            var (response, newAsset) = await ValidateCreateUpdateModel<AssetLiquidationSheetViewModel>(model);

            if (response != null)
            {
                return response;
            }

            var oldSheet = await _dbContext.sm_AssetLiquidationSheet
                .Include(x => x.Asset)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (oldSheet == null)
            {
                return Helper.CreateBadRequestResponse<AssetLiquidationSheetViewModel>("Phiếu thanh lý không tồn tại");
            }

            // Check if asset ID has changed
            if (oldSheet.AssetId != model.AssetId)
            {
                // Validate new asset status - only OutOfUse or Damaged assets can be liquidated
                var allowedStatuses = new[] { AssetStatus.OutOfUse, AssetStatus.Damaged };
                if (!allowedStatuses.Contains(newAsset.Status))
                {
                    return Helper.CreateBadRequestResponse<AssetLiquidationSheetViewModel>(
                        "Chỉ được thanh lý tài sản đã ngừng sử dụng hoặc đã hỏng");
                }

                // Revert old asset status
                var oldAsset = oldSheet.Asset;
                oldAsset.Status = AssetStatus.InUse;

                // Update new asset status
                newAsset.Status = AssetStatus.Liquidated;

                // Update asset usage history
                var usageHistory = await _dbContext.sm_AssetUsageHistory
                    .FirstOrDefaultAsync(x => x.EntityId == oldSheet.Id && x.EntityType == "AssetLiquidationSheet");

                if (usageHistory != null)
                {
                    usageHistory.AssetId = model.AssetId;
                    usageHistory.Description = model.LiquidationReason;
                    usageHistory.Cost = model.LiquidationValue;
                    usageHistory.Operation = AssetBusinessOperation.Liquidate;
                }
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            oldSheet.AssetId = model.AssetId;
            oldSheet.LiquidationDate = model.LiquidationDate;
            oldSheet.DecisionNumber = model.DecisionNumber;
            oldSheet.LiquidatorId = model.LiquidatorId;
            oldSheet.LiquidationValue = model.LiquidationValue;
            oldSheet.LiquidationReason = model.LiquidationReason;
            oldSheet.LastModifiedOnDate = DateTime.Now;
            oldSheet.LastModifiedByUserId = currentUser.UserId;
            oldSheet.LastModifiedByUserName = currentUser.UserName;

            _dbContext.sm_AssetLiquidationSheet.Update(oldSheet);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetLiquidationSheetViewModel>(oldSheet),
                "Cập nhật phiếu thanh lý thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<AssetLiquidationSheetViewModel>(e);
        }
    }

    public async Task<Response<Pagination<AssetLiquidationSheetViewModel>>> GetPage(
        AssetLiquidationSheetQueryModel query)
    {
        try
        {
            var predicate = BuildQuery(query);
            var queryResult = _dbContext.sm_AssetLiquidationSheet
                .Include(x => x.Liquidator)
                .Include(x => x.Asset)
                .Where(predicate);
            var data = await queryResult.GetPageAsync(query);
            var result = _mapper.Map<Pagination<AssetLiquidationSheetViewModel>>(data);

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            Log.Information("Params: Model: {@query}", query);
            return Helper.CreateExceptionResponse<Pagination<AssetLiquidationSheetViewModel>>(ex);
        }
    }

    public async Task<Response<Dictionary<string, int>>> CountByStatus(AssetLiquidationSheetQueryModel query)
    {
        try
        {
            var predicate = BuildQuery(query);
            var result = new Dictionary<string, int>();

            result["Total"] = await _dbContext.sm_AssetLiquidationSheet
                .Where(predicate)
                .CountAsync();

            result["OverDue"] = await _dbContext.sm_AssetLiquidationSheet
                .CountAsync(x => x.LiquidationDate < DateTime.Now);

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            return Helper.CreateExceptionResponse<Dictionary<string, int>>(e);
        }
    }

    public async Task<Response<AssetLiquidationSheetViewModel>> GetById(Guid id)
    {
        try
        {
            var sheet = await _dbContext.sm_AssetLiquidationSheet
                .Include(x => x.Liquidator)
                .Include(x => x.Asset)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sheet == null)
            {
                return Helper.CreateBadRequestResponse<AssetLiquidationSheetViewModel>("Phiếu thanh lý không tồn tại");
            }

            return Helper.CreateSuccessResponse(_mapper.Map<AssetLiquidationSheetViewModel>(sheet));
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@id}", id);
            return Helper.CreateExceptionResponse<AssetLiquidationSheetViewModel>(e);
        }
    }

    public async Task<Response> Delete(Guid id)
    {
        try
        {
            var sheet = await _dbContext.sm_AssetLiquidationSheet.FindAsync(id);

            if (sheet == null)
            {
                return Helper.CreateNotFoundResponse(string.Format("Phiếu thanh lý không tồn tại"));
            }

            _dbContext.sm_AssetLiquidationSheet.Remove(sheet);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse("Xóa thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@id}", id);
            return Helper.CreateExceptionResponse(e);
        }
    }

    private Expression<Func<sm_AssetLiquidationSheet, bool>> BuildQuery(AssetLiquidationSheetQueryModel query)
    {
        var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
        var predicate = PredicateBuilder.New<sm_AssetLiquidationSheet>(true);

        if (currentUser.TenantId != null)
        {
            predicate.And(x => x.TenantId == currentUser.TenantId);
        }

        if (!string.IsNullOrWhiteSpace(query.FullTextSearch))
        {
            predicate.And(x =>
                (x.Asset != null && x.Asset.Code.ToLower().Contains(query.FullTextSearch.Trim().ToLower())) ||
                (x.Asset != null && x.Asset.Name.ToLower().Contains(query.FullTextSearch.Trim().ToLower())));
        }

        if (query.LiquidationDate.Count >= 1 && query.LiquidationDate[0].HasValue)
        {
            predicate.And(x => x.LiquidationDate >= query.LiquidationDate[0].Value);
        }

        if (query.LiquidationDate.Count >= 2 && query.LiquidationDate[1].HasValue)
        {
            predicate.And(x => x.LiquidationDate <= query.LiquidationDate[1].Value);
        }

        if (!string.IsNullOrWhiteSpace(query.DecisionNumber))
        {
            predicate.And(x =>
                x.DecisionNumber != null && x.DecisionNumber.ToLower().Contains(query.DecisionNumber.Trim().ToLower()));
        }

        if (query.LiquidatorId.HasValue)
        {
            predicate.And(x => x.LiquidatorId == query.LiquidatorId.Value);
        }

        return predicate;
    }

    private async Task<(Response<T>, sm_Asset)> ValidateCreateUpdateModel<T>(
        AssetLiquidationSheetCreateUpdateModel model)
    {
        Response<T> response = null;

        // Validate Asset exists
        var asset = await _dbContext.sm_Asset.FindAsync(model.AssetId);
        if (asset == null)
        {
            response = Helper.CreateBadRequestResponse<T>("Tài sản không tồn tại");
            return (response, null);
        }

        // Validate Asset status - only OutOfUse or Damaged assets can be liquidated
        var allowedStatuses = new[] { AssetStatus.OutOfUse, AssetStatus.Damaged };
        if (!allowedStatuses.Contains(asset.Status))
        {
            response = Helper.CreateBadRequestResponse<T>("Chỉ được thanh lý tài sản đã ngừng sử dụng hoặc đã hỏng");
            return (response, null);
        }

        // Validate LiquidationDate is required
        if (model.LiquidationDate == default)
        {
            response = Helper.CreateBadRequestResponse<T>("Ngày thanh lý không được để trống");
            return (response, null);
        }

        // Validate Liquidator exists if provided
        if (model.LiquidatorId.HasValue)
        {
            var liquidator = await _dbContext.IdmUser.FindAsync(model.LiquidatorId.Value);
            if (liquidator == null)
            {
                response = Helper.CreateBadRequestResponse<T>("Người thực hiện không tồn tại");
                return (response, null);
            }
        }

        return (response, asset);
    }
}