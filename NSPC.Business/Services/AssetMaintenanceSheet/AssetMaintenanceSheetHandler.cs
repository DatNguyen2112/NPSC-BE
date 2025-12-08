using System.Linq.Expressions;
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.Asset;
using NSPC.Data.Data.Entity.AssetHistory;
using Serilog;

namespace NSPC.Business;

public class AssetMaintenanceSheetHandler : IAssetMaintenanceSheetHandler
{
    private readonly SMDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AssetMaintenanceSheetHandler(
        SMDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IAttachmentHandler attachmentHandler
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<AssetMaintenanceSheetViewModel>> Create(AssetMaintenanceSheetCreateUpdateModel model)
    {
        try
        {
            var (result, asset) = await ValidateCreateUpdateModel<AssetMaintenanceSheetViewModel>(model);

            if (result != null)
            {
                return result;
            }

            // Validate Asset status
            var allowedStatuses = new[]
            {
                AssetStatus.InUse,
                AssetStatus.Damaged,
                AssetStatus.OutOfUse
            };

            if (!allowedStatuses.Contains(asset.Status))
            {
                return Helper.CreateBadRequestResponse<AssetMaintenanceSheetViewModel>(
                    "Chỉ được tạo phiếu bảo trì cho tài sản đang sử dụng, đã hỏng hoặc chưa sử dụng");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var assetMaintenanceSheet = _mapper.Map<sm_AssetMaintenanceSheet>(model);

            asset.Status = AssetStatus.UnderMaintenance;
            assetMaintenanceSheet.Id = Guid.NewGuid();
            assetMaintenanceSheet.Status = MaintenanceStatus.InProgress;
            assetMaintenanceSheet.CreatedOnDate = DateTime.Now;
            assetMaintenanceSheet.CreatedByUserId = currentUser.UserId;
            assetMaintenanceSheet.CreatedByUserName = currentUser.UserName;
            assetMaintenanceSheet.LastModifiedOnDate = assetMaintenanceSheet.CreatedOnDate;
            assetMaintenanceSheet.LastModifiedByUserId = assetMaintenanceSheet.CreatedByUserId;
            assetMaintenanceSheet.LastModifiedByUserName = assetMaintenanceSheet.CreatedByUserName;
            assetMaintenanceSheet.TenantId = currentUser.TenantId;

            var usageHistory = new sm_AssetUsageHistory
            {
                Id = Guid.NewGuid(),
                AssetId = assetMaintenanceSheet.AssetId,
                Operation = AssetBusinessOperation.StartMaintenance,
                AssetStatus = asset.Status,
                ExecutionDate = assetMaintenanceSheet.StartDate ?? assetMaintenanceSheet.CreatedOnDate,
                LocationId = asset.AssetLocationId,
                UserId = asset.UserId,
                Cost = assetMaintenanceSheet.EstimatedCost,
                Description = assetMaintenanceSheet.MaintenanceContent,
                EntityId = assetMaintenanceSheet.Id,
                EntityType = "AssetMaintenanceSheet",
                CreatedOnDate = assetMaintenanceSheet.CreatedOnDate,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                LastModifiedOnDate = assetMaintenanceSheet.CreatedOnDate,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                TenantId = currentUser.TenantId
            };

            _dbContext.sm_AssetMaintenanceSheet.Add(assetMaintenanceSheet);
            _dbContext.sm_AssetUsageHistory.Add(usageHistory);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetMaintenanceSheetViewModel>(assetMaintenanceSheet),
                "Tạo phiếu bảo trì thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<AssetMaintenanceSheetViewModel>(e);
        }
    }

    public async Task<Response<AssetMaintenanceSheetViewModel>> Update(Guid id,
        AssetMaintenanceSheetCreateUpdateModel model)
    {
        try
        {
            var oldSheet = await _dbContext.sm_AssetMaintenanceSheet.FindAsync(id);

            if (oldSheet == null)
            {
                return Helper.CreateBadRequestResponse<AssetMaintenanceSheetViewModel>("Phiếu bảo trì không tồn tại");
            }

            // Check if sheet is completed
            if (oldSheet.Status == MaintenanceStatus.Completed)
            {
                return Helper.CreateBadRequestResponse<AssetMaintenanceSheetViewModel>(
                    "Không được sửa phiếu bảo trì đã hoàn thành");
            }

            var (result, asset) = await ValidateCreateUpdateModel<AssetMaintenanceSheetViewModel>(model);

            if (result != null)
            {
                return result;
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            // Update properties but keep the status
            _mapper.Map(model, oldSheet);
            oldSheet.LastModifiedOnDate = DateTime.Now;
            oldSheet.LastModifiedByUserId = currentUser.UserId;
            oldSheet.LastModifiedByUserName = currentUser.UserName;
            _dbContext.sm_AssetMaintenanceSheet.Update(oldSheet);

            var usageHistory = _dbContext.sm_AssetUsageHistory.FirstOrDefault(x => x.EntityId == id);
            if (usageHistory != null)
            {
                usageHistory.ExecutionDate = oldSheet.StartDate ?? oldSheet.CreatedOnDate;
                usageHistory.Cost = model.EstimatedCost;
                usageHistory.Description = model.MaintenanceContent;
                usageHistory.LastModifiedOnDate = oldSheet.LastModifiedOnDate;
                usageHistory.LastModifiedByUserId = currentUser.UserId;
                usageHistory.LastModifiedByUserName = currentUser.UserName;
                _dbContext.sm_AssetUsageHistory.Update(usageHistory);
            }

            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetMaintenanceSheetViewModel>(oldSheet),
                "Cập nhật phiếu bảo trì thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<AssetMaintenanceSheetViewModel>(e);
        }
    }

    public async Task<Response<AssetMaintenanceSheetViewModel>> CompleteMaintenance(Guid id,
        AssetMaintenanceSheetCompleteModel model)
    {
        try
        {
            var sheet = await _dbContext.sm_AssetMaintenanceSheet
                .Include(x => x.Asset)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sheet == null)
            {
                return Helper.CreateBadRequestResponse<AssetMaintenanceSheetViewModel>("Phiếu bảo trì không tồn tại");
            }

            if (sheet.Status == MaintenanceStatus.Completed)
            {
                return Helper.CreateBadRequestResponse<AssetMaintenanceSheetViewModel>("Phiếu bảo trì đã hoàn thành");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            sheet.Asset.Status = sheet.Asset.UserId == null ? AssetStatus.OutOfUse : AssetStatus.InUse;
            sheet.Status = MaintenanceStatus.Completed;
            sheet.CompleteDate = model.CompletedDate;
            sheet.LastModifiedOnDate = DateTime.Now;
            sheet.LastModifiedByUserId = currentUser.UserId;
            sheet.LastModifiedByUserName = currentUser.UserName;

            var usageHistory = new sm_AssetUsageHistory
            {
                Id = Guid.NewGuid(),
                AssetId = sheet.AssetId,
                Operation = AssetBusinessOperation.CompleteMaintenance,
                AssetStatus = sheet.Asset.Status,
                ExecutionDate = model.CompletedDate,
                LocationId = sheet.Asset.AssetLocationId,
                UserId = sheet.Asset.UserId,
                Cost = sheet.EstimatedCost,
                Description = sheet.MaintenanceContent,
                EntityId = sheet.Id,
                EntityType = "AssetMaintenanceSheet",
                CreatedOnDate = sheet.LastModifiedOnDate ?? DateTime.Now,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                LastModifiedOnDate = sheet.LastModifiedOnDate,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                TenantId = currentUser.TenantId
            };

            _dbContext.sm_AssetUsageHistory.Add(usageHistory);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetMaintenanceSheetViewModel>(sheet),
                "Hoàn thành bảo trì thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            return Helper.CreateExceptionResponse<AssetMaintenanceSheetViewModel>(e);
        }
    }

    public async Task<Response<Pagination<AssetMaintenanceSheetViewModel>>> GetPage(
        AssetMaintenanceSheetQueryModel query)
    {
        try
        {
            var predicate = BuildQuery(query);
            var queryResult = _dbContext.sm_AssetMaintenanceSheet
                .Include(x => x.Asset)
                .Where(predicate);
            var data = await queryResult.GetPageAsync(query);
            var result = _mapper.Map<Pagination<AssetMaintenanceSheetViewModel>>(data);

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            Log.Information("Params: Model: {@query}", query);
            return Helper.CreateExceptionResponse<Pagination<AssetMaintenanceSheetViewModel>>(ex);
        }
    }

    public async Task<Response<Dictionary<string, int>>> CountByStatus(AssetMaintenanceSheetQueryModel query)
    {
        try
        {
            var predicate = BuildQuery(query);
            var result = await _dbContext.sm_AssetMaintenanceSheet
                .Where(predicate)
                .GroupBy(x => x.Status)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count);

            foreach (MaintenanceStatus status in Enum.GetValues(typeof(MaintenanceStatus)))
            {
                if (!result.ContainsKey(status.ToString()))
                {
                    result[status.ToString()] = 0;
                }
            }

            result["Total"] = result.Values.Sum();
            result["OverDue"] = await _dbContext.sm_AssetMaintenanceSheet
                .CountAsync(x => DateTime.Now > x.MaintenancePeriod && x.Status != MaintenanceStatus.Completed);

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            return Helper.CreateExceptionResponse<Dictionary<string, int>>(e);
        }
    }

    public async Task<Response<AssetMaintenanceSheetViewModel>> GetById(Guid id)
    {
        try
        {
            var sheet = await _dbContext.sm_AssetMaintenanceSheet
                .Include(x => x.Asset)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sheet == null)
            {
                return Helper.CreateBadRequestResponse<AssetMaintenanceSheetViewModel>("Phiếu bảo trì không tồn tại");
            }

            var result = _mapper.Map<AssetMaintenanceSheetViewModel>(sheet);

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@id}", id);
            return Helper.CreateExceptionResponse<AssetMaintenanceSheetViewModel>(e);
        }
    }

    public async Task<Response> Delete(Guid id)
    {
        try
        {
            var asset = await _dbContext.sm_AssetMaintenanceSheet.FindAsync(id);

            if (asset == null)
            {
                return Helper.CreateNotFoundResponse(string.Format("Phiếu bảo trì không tồn tại"));
            }

            _dbContext.sm_AssetMaintenanceSheet.Remove(asset);
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

    private Expression<Func<sm_AssetMaintenanceSheet, bool>> BuildQuery(AssetMaintenanceSheetQueryModel query)
    {
        var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
        var predicate = PredicateBuilder.New<sm_AssetMaintenanceSheet>(true);

        if (currentUser.TenantId != null)
        {
            predicate.And(x => x.TenantId == currentUser.TenantId);
        }

        if (!string.IsNullOrWhiteSpace(query.FullTextSearch))
        {
            predicate.And(x =>
                x.Asset.Name.ToLower().Contains(query.FullTextSearch.Trim().ToLower()) ||
                x.Asset.Code.ToLower().Contains(query.FullTextSearch.Trim().ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(query.AssetName))
        {
            predicate.And(x => x.Asset.Name.ToLower().Contains(query.AssetName.Trim().ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(query.AssetCode))
        {
            predicate.And(x => x.Asset.Code.ToLower().Contains(query.AssetCode.Trim().ToLower()));
        }

        if (query.MaintenancePeriod.Count >= 1 && query.MaintenancePeriod[0].HasValue)
        {
            predicate.And(x => x.MaintenancePeriod >= query.MaintenancePeriod[0].Value);
        }

        if (query.MaintenancePeriod.Count >= 2 && query.MaintenancePeriod[1].HasValue)
        {
            predicate.And(x => x.MaintenancePeriod <= query.MaintenancePeriod[1].Value);
        }

        if (!string.IsNullOrWhiteSpace(query.MaintenanceType))
        {
            if (Enum.TryParse<MaintenanceType>(query.MaintenanceType, out var type))
            {
                predicate.And(x => x.MaintenanceType == type);
            }
            else
            {
                predicate.And(x => false);
            }
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (Enum.TryParse<MaintenanceStatus>(query.Status, out var status))
            {
                predicate.And(x => x.Status == status);
            }
            else
            {
                predicate.And(x => false);
            }
        }

        return predicate;
    }

    private async Task<(Response<T>, sm_Asset)> ValidateCreateUpdateModel<T>(
        AssetMaintenanceSheetCreateUpdateModel model)
    {
        Response<T> response = null;

        // Validate Asset exists
        var asset = await _dbContext.sm_Asset.FindAsync(model.AssetId);
        if (asset == null)
        {
            response = Helper.CreateBadRequestResponse<T>("Tài sản không tồn tại");
        }

        // Validate MaintenanceType
        if (response == null && string.IsNullOrWhiteSpace(model.MaintenanceType))
        {
            response = Helper.CreateBadRequestResponse<T>("Loại bảo trì không được để trống");
        }

        if (response == null && !Enum.TryParse<MaintenanceType>(model.MaintenanceType, out var maintenanceType))
        {
            response = Helper.CreateBadRequestResponse<T>("Loại bảo trì không hợp lệ");
            return (response, null);
        }

        // Validate MaintenanceLocation
        if (response == null && string.IsNullOrWhiteSpace(model.MaintenanceLocation))
        {
            response = Helper.CreateBadRequestResponse<T>("Địa điểm bảo trì không được để trống");
        }

        if (response == null &&
            !Enum.TryParse<MaintenanceLocation>(model.MaintenanceLocation, out var maintenanceLocation))
        {
            response = Helper.CreateBadRequestResponse<T>("Địa điểm bảo trì không hợp lệ");
            return (response, null);
        }

        // Validate MaintenancePeriod
        if (response == null && model.StartDate.HasValue && model.MaintenancePeriod < model.StartDate.Value)
        {
            response = Helper.CreateBadRequestResponse<T>("Hạn bảo trì phải sau ngày bắt đầu");
        }

        // Validate Performer exists if provided
        if (response == null && model.PerformerId.HasValue)
        {
            var performer = await _dbContext.IdmUser.FindAsync(model.PerformerId.Value);
            if (performer == null)
            {
                response = Helper.CreateBadRequestResponse<T>("Người thực hiện không tồn tại");
                return (response, null);
            }
        }

        return (response, asset);
    }
}