using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.AssetAllocation;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.Asset;
using NSPC.Data.Data.Entity.AssetAllocation;
using NSPC.Data.Data.Entity.AssetHistory;
using NSPC.Data.Data.Entity.AssetIncident;
using Serilog;
using System.Linq.Expressions;

namespace NSPC.Business;

public class AssetHandler : IAssetHandler
{
    private readonly List<AssetStatus> _statusList = new()
    {
        AssetStatus.InUse,
        AssetStatus.OutOfUse,
        AssetStatus.UnderMaintenance,
        AssetStatus.Liquidated,
        AssetStatus.Damaged,
        AssetStatus.Lost,
        AssetStatus.Destroyed
    };

    /// <summary>
    /// Lấy danh sách các thao tác được phép thực hiện dựa trên trạng thái hiện tại của tài sản
    /// </summary>
    /// <param name="status">Trạng thái hiện tại của tài sản</param>
    /// <returns>Danh sách các thao tác được phép thực hiện dạng string</returns>
    public static List<string> GetAllowedOperations(AssetStatus status)
    {
        var allowedOperations = new List<string>();

        switch (status)
        {
            case AssetStatus.OutOfUse: // Chưa sử dụng
                allowedOperations.Add(AssetBusinessOperation.Allocate.ToString()); // Cấp phát
                allowedOperations.Add(AssetBusinessOperation.StartMaintenance.ToString()); // Bảo trì
                allowedOperations.Add(AssetBusinessOperation.Liquidate.ToString()); // Thanh lý
                allowedOperations.Add(AssetBusinessOperation.ReportDestroyed.ToString()); // Báo huỷ
                allowedOperations.Add(AssetBusinessOperation.ReportLost.ToString()); // Đánh dấu đã mất
                allowedOperations.Add(AssetBusinessOperation.ReportDamaged.ToString()); // Báo hỏng
                break;

            case AssetStatus.InUse: // Đang sử dụng
                allowedOperations.Add(AssetBusinessOperation.Revoke.ToString()); // Thu hồi
                allowedOperations.Add(AssetBusinessOperation.Transfer.ToString()); // Điều chuyển
                allowedOperations.Add(AssetBusinessOperation.StartMaintenance.ToString()); // Bảo trì
                allowedOperations.Add(AssetBusinessOperation.ReportLost.ToString()); // Đánh dấu đã mất
                allowedOperations.Add(AssetBusinessOperation.ReportDamaged.ToString()); // Báo hỏng
                break;

            case AssetStatus.UnderMaintenance: // Đang bảo trì
                allowedOperations.Add(AssetBusinessOperation.CompleteMaintenance.ToString()); // Hoàn thành bảo trì
                break;

            case AssetStatus.Damaged: // Đã hỏng
                allowedOperations.Add(AssetBusinessOperation.StartMaintenance.ToString()); // Bảo trì
                allowedOperations.Add(AssetBusinessOperation.Liquidate.ToString()); // Thanh lý
                allowedOperations.Add(AssetBusinessOperation.ReportDestroyed.ToString()); // Báo huỷ
                allowedOperations.Add(AssetBusinessOperation.ReportLost.ToString()); // Đánh dấu đã mất
                break;

            // Các trạng thái khác không có thao tác được phép
            case AssetStatus.Liquidated: // Đã thanh lý
            case AssetStatus.Lost: // Đã mất
            case AssetStatus.Destroyed: // Đã huỷ
            default:
                break;
        }

        return allowedOperations;
    }

    private readonly SMDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAttachmentHandler _attachmentHandler;
    private readonly IEmailService _emailService;

    public AssetHandler(
        SMDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IAttachmentHandler attachmentHandler,
        IEmailService emailService
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _attachmentHandler = attachmentHandler;
        _emailService = emailService;
    }

    public async Task<Response<List<AssetViewModel>>> Create(AssetCreateUpdateModel model)
    {
        try
        {
            var result = await ValidateCreateUpdateModel<List<AssetViewModel>>(model);

            if (result != null)
            {
                return result;
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var newAssets = new List<sm_Asset>();
            var newUsageHistories = new List<sm_AssetUsageHistory>();

            for (int i = 0; i < model.Quantity; i++)
            {
                var asset = _mapper.Map<sm_Asset>(model);

                asset.Id = Guid.NewGuid();
                asset.Status = AssetStatus.OutOfUse;
                asset.CreatedOnDate = DateTime.Now;
                asset.CreatedByUserId = currentUser.UserId;
                asset.CreatedByUserName = currentUser.UserName;
                asset.LastModifiedOnDate = asset.CreatedOnDate;
                asset.LastModifiedByUserId = asset.CreatedByUserId;
                asset.LastModifiedByUserName = asset.CreatedByUserName;
                asset.TenantId = currentUser.TenantId;
                asset.Images = await ProcessAttachment(asset, model.Images);
                asset.Documents = await ProcessAttachment(asset, model.Documents);
                newAssets.Add(asset);
                newUsageHistories.Add(new sm_AssetUsageHistory
                {
                    Id = Guid.NewGuid(),
                    AssetId = asset.Id,
                    Operation = AssetBusinessOperation.AddNew,
                    AssetStatus = asset.Status,
                    ExecutionDate = asset.CreatedOnDate,
                    LocationId = asset.AssetLocationId,
                    UserId = null,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    TenantId = currentUser.TenantId
                });
            }

            _dbContext.sm_Asset.AddRange(newAssets);
            _dbContext.sm_AssetUsageHistory.AddRange(newUsageHistories);
            await _dbContext.SaveChangesAsync();

            // await _dbContext.Entry(asset)
            //     .Reference(x => x.AssetType)
            //     .LoadAsync();
            // await _dbContext.Entry(asset)
            //     .Reference(x => x.AssetLocation)
            //     .LoadAsync();
            // await _dbContext.Entry(asset)
            //     .Reference(x => x.MeasureUnit)
            //     .LoadAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<List<AssetViewModel>>(newAssets), "Tạo tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<List<AssetViewModel>>(e);
        }
    }

    public async Task<Response<AssetViewModel>> Update(Guid id, AssetCreateUpdateModel model)
    {
        try
        {
            var result = await ValidateCreateUpdateModel<AssetViewModel>(model);

            if (result != null)
            {
                return result;
            }

            var oldAsset = await _dbContext.sm_Asset.FindAsync(id);

            if (oldAsset == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không tồn tại");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var oldLocation = oldAsset.AssetLocationId;

            _mapper.Map(model, oldAsset);
            oldAsset.AssetLocationId = oldLocation;
            oldAsset.LastModifiedOnDate = DateTime.Now;
            oldAsset.LastModifiedByUserId = currentUser.UserId;
            oldAsset.LastModifiedByUserName = currentUser.UserName;
            oldAsset.Images = await ProcessAttachment(oldAsset, model.Images);
            oldAsset.Documents = await ProcessAttachment(oldAsset, model.Documents);

            _dbContext.sm_Asset.Update(oldAsset);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetViewModel>(oldAsset), "Cập nhật tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<AssetViewModel>(e);
        }
    }

    public async Task<Response<Pagination<AssetViewModel>>> GetPage(AssetQueryModel query)
    {
        try
        {
            var predicate = BuildQuery(query);
            var queryResult = _dbContext.sm_Asset
                .Include(x => x.AssetType)
                .Include(x => x.AssetLocation)
                .Include(x => x.User)
                .Include(x => x.MeasureUnit)
                .Where(predicate);
            var data = await queryResult.GetPageAsync(query);
            var result = _mapper.Map<Pagination<AssetViewModel>>(data);

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            Log.Information("Params: Model: {@query}", query);
            return Helper.CreateExceptionResponse<Pagination<AssetViewModel>>(ex);
        }
    }

    public async Task<Response<Dictionary<string, int>>> CountByStatus(AssetQueryModel query)
    {
        var predicate = BuildQuery(query);
        var result = await _dbContext.sm_Asset
            .Where(predicate)
            .GroupBy(x => x.Status)
            .Select(x => new { x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count);

        result["Total"] = result.Values.Sum();
        foreach (var status in _statusList.Select(x => x.ToString()).Where(status => !result.ContainsKey(status)))
        {
            result[status] = 0;
        }

        return Helper.CreateSuccessResponse(result);
    }

    public async Task<Response<AssetDetailViewModel>> GetById(Guid id)
    {
        try
        {
            var asset = await _dbContext.sm_Asset
                .IgnoreQueryFilters()
                .Include(x => x.AssetType)
                .Include(x => x.AssetLocation)
                .Include(x => x.User)
                .Include(x => x.MeasureUnit)
                .FirstOrDefaultAsync(x => x.Id == id);

            await _dbContext.sm_AssetUsageHistory
                .IgnoreQueryFilters()
                .Include(x => x.Location)
                .Include(x => x.User)
                .Include(x => x.CreatedByUser)
                .OrderByDescending(x => x.CreatedOnDate)
                .LoadAsync();

            if (asset == null)
            {
                return Helper.CreateBadRequestResponse<AssetDetailViewModel>("Tài sản không tồn tại");
            }

            var result = _mapper.Map<AssetDetailViewModel>(asset);

            if (result.OriginalPrice != 0 && result.DepreciationValue.HasValue && result.DepreciationValue.Value != 0 &&
                result.DepreciationTime.HasValue && result.DepreciationTime.Value != 0)
            {
                var dayOfUnit = result.DepreciationUnit == DepreciationUnit.Month.ToString() ? 30 : 365;
                var depreciationDay = dayOfUnit * result.DepreciationTime;
                var depreciationPerDay = result.DepreciationValue!.Value / depreciationDay;
                var maxDay = (int)Math.Floor(result.OriginalPrice / depreciationPerDay.Value);
                var ascendingHistories = result.AssetUsageHistories
                    .OrderBy(x => x.CreatedOnDate)
                    .ToList();
                var inUseDateRange = new List<(DateTime, DateTime)>();

                for (var i = 0; i < ascendingHistories.Count; i++)
                {
                    var history = ascendingHistories[i];

                    if (history.AssetStatus != AssetStatus.InUse.ToString())
                    {
                        continue;
                    }

                    var startDate = history.ExecutionDate.Date;
                    var endDate = DateTime.Now.Date;
                    var isContinue = false;

                    if (i < ascendingHistories.Count - 1)
                    {
                        endDate = ascendingHistories[i + 1].ExecutionDate.Date;
                    }

                    if (result.DepreciationStartDate.HasValue)
                    {
                        if (endDate < result.DepreciationStartDate.Value)
                        {
                            continue;
                        }

                        if (startDate < result.DepreciationStartDate.Value)
                        {
                            startDate = result.DepreciationStartDate.Value;
                        }
                    }

                    if (inUseDateRange.Count > 0)
                    {
                        if (inUseDateRange[^1].Item2 == startDate)
                        {
                            inUseDateRange[^1] = (inUseDateRange[^1].Item1, endDate);
                            isContinue = true;
                        }
                    }

                    if (!isContinue)
                    {
                        inUseDateRange.Add((startDate, endDate));
                    }
                }

                var totalDay = inUseDateRange.Sum(x => (x.Item2 - x.Item1).Days + 1);
                var actualDay = Math.Min(maxDay, totalDay);
                result.AccumulatedDepreciation = Math.Ceiling(actualDay * depreciationPerDay.Value);
                result.RemainingValue = result.OriginalPrice - result.AccumulatedDepreciation;
            }

            if (asset.Status == AssetStatus.InUse)
            {
                var allocation = await _dbContext.sm_AssetAllocation
                    .Include(x => x.FromLocation)
                    .Include(x => x.ToLocation)
                    .Include(x => x.FromUser)
                    .Include(x => x.ToUser)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .FirstOrDefaultAsync(x => x.AssetId == asset.Id);

                result.Allocation = _mapper.Map<AssetAllocationViewModel>(allocation);
            }

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@id}", id);
            return Helper.CreateExceptionResponse<AssetDetailViewModel>(e);
        }
    }

    public async Task<Response> Delete(Guid id)
    {
        try
        {
            var asset = await _dbContext.sm_Asset.FindAsync(id);

            if (asset == null)
            {
                return Helper.CreateNotFoundResponse(string.Format("Tài sản không tồn tại"));
            }

            if (asset.Status == AssetStatus.InUse)
            {
                return Helper.CreateBadRequestResponse("Không thể xóa tài sản đang sử dụng");
            }

            _dbContext.Remove(asset);
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

    private async Task<List<jsonb_Attachment>> ProcessAttachment(sm_Asset asset, List<jsonb_Attachment> attachments)
    {
        try
        {
            var attachmentIdList = attachments.Select(x => x.Id).ToList();

            if (attachmentIdList.Count == 0)
            {
                return new List<jsonb_Attachment>();
            }

            var allAttachments = await _dbContext.erp_Attachment
                .Where(x => attachmentIdList.Contains(x.Id))
                .ToListAsync();

            foreach (var att in allAttachments)
            {
                att.EntityId = asset.Id;
                att.EntityType = attachments.FirstOrDefault(x => x.Id == att.Id)?.DocType;
                att.Description = attachments.FirstOrDefault(x => x.Id == att.Id)?.Description;

                var moveFileResult = _attachmentHandler.MoveEntityAttachment(att.DocType, att.EntityType,
                    asset.Id, att.FilePath, asset.CreatedOnDate);
                if (moveFileResult.IsSuccess)
                {
                    att.FilePath = moveFileResult.Data;
                }
            }

            await _dbContext.SaveChangesAsync();

            return allAttachments.Select(x => new jsonb_Attachment
            {
                Description = x.Description,
                DocType = x.DocType,
                FilePath = x.FilePath,
                Id = x.Id
            }).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            throw;
        }
    }

    private Expression<Func<sm_Asset, bool>> BuildQuery(AssetQueryModel query)
    {
        var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
        var predicate = PredicateBuilder.New<sm_Asset>(true);

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

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.Name.Trim().ToLower()));
        }

        if (Enum.TryParse(query.Status, out AssetStatus status))
        {
            predicate.And(x => x.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            predicate.And(x => x.Code.ToLower().Contains(query.Code.Trim().ToLower()));
        }

        if (query.AssetTypeId.HasValue)
        {
            predicate.And(x => x.AssetTypeId == query.AssetTypeId.Value);
        }

        if (query.AssetLocationId.HasValue)
        {
            predicate.And(x => x.AssetLocationId == query.AssetLocationId.Value);
        }

        if (query.PurchasedDate.Count >= 1 && query.PurchasedDate[0].HasValue)
        {
            predicate.And(x => x.PurchasedDate >= query.PurchasedDate[0].Value);
        }

        if (query.PurchasedDate.Count >= 2 && query.PurchasedDate[1].HasValue)
        {
            predicate.And(x => x.PurchasedDate <= query.PurchasedDate[1].Value);
        }

        if (query.UserId.HasValue)
        {
            predicate.And(x => x.UserId == query.UserId);
        }

        return predicate;
    }

    private async Task<Response<T>> ValidateCreateUpdateModel<T>(
        AssetCreateUpdateModel model)
    {
        Response<T> response = null;

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            response = Helper.CreateBadRequestResponse<T>("Tên tài sản không được để trống");
        }

        if (response == null && model.Quantity < 1)
        {
            response = Helper.CreateBadRequestResponse<T>("Số lượng không được nhỏ hơn 1");
        }

        if (response == null)
        {
            var assetType = await _dbContext.sm_AssetType
                .FirstOrDefaultAsync(x => x.Id == model.AssetTypeId);

            if (assetType == null)
            {
                response = Helper.CreateBadRequestResponse<T>("Loại tài sản không tồn tại");
            }
        }

        if (response == null)
        {
            var assetLocation = await _dbContext.sm_AssetLocation
                .FirstOrDefaultAsync(x => x.Id == model.AssetLocationId);

            if (assetLocation == null)
            {
                response = Helper.CreateBadRequestResponse<T>("Vị trí tài sản không tồn tại");
            }
        }

        if (response == null && model.MeasureUnitId.HasValue)
        {
            var measureUnit = await _dbContext.sm_MeasureUnit
                .FirstOrDefaultAsync(x => x.Id == model.MeasureUnitId.Value);

            if (measureUnit == null)
            {
                response = Helper.CreateBadRequestResponse<T>("Đơn vị tính không tồn tại");
            }
        }

        // Validate DepreciationUnit if DepreciationTime is provided
        if (response == null && model.DepreciationTime.HasValue)
        {
            if (string.IsNullOrEmpty(model.DepreciationUnit))
            {
                response = Helper.CreateBadRequestResponse<T>(
                    "Đơn vị khấu hao không được để trống khi có thời gian khấu hao");
            }
            else if (!Enum.TryParse<DepreciationUnit>(model.DepreciationUnit, out var depreciationUnit))
            {
                response = Helper.CreateBadRequestResponse<T>(
                    "Đơn vị khấu hao không hợp lệ. Chỉ chấp nhận giá trị Month hoặc Year");
            }
        }

        return response;
    }

    public async Task<Response<AssetViewModel>> ReportDamage(Guid assetId, AssetReportDamageModel model)
    {
        try
        {
            var asset = await _dbContext.sm_Asset.FindAsync(assetId);

            if (asset == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không tồn tại");
            }

            // Validate asset status
            var allowedStatuses = new[]
            {
                AssetStatus.InUse,
                AssetStatus.UnderMaintenance,
                AssetStatus.OutOfUse
            };

            if (!allowedStatuses.Contains(asset.Status))
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>(
                    "Chỉ được báo hỏng tài sản đang sử dụng, đang bảo trì hoặc chưa sử dụng");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var now = DateTime.Now;

            // Create AssetIncident
            var incident = new sm_AssetIncident
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                IncidentType = AssetBusinessOperation.ReportDamaged,
                IncidentDate = model.IncidentDate,
                Description = model.Description,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                LastModifiedOnDate = now,
                TenantId = currentUser.TenantId
            };

            // Create AssetUsageHistory
            var history = new sm_AssetUsageHistory
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                Operation = AssetBusinessOperation.ReportDamaged,
                AssetStatus = AssetStatus.Damaged,
                ExecutionDate = model.IncidentDate,
                Description = model.Description,
                LocationId = asset.AssetLocationId,
                UserId = asset.UserId,
                EntityType = "AssetIncident",
                EntityId = incident.Id,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                LastModifiedOnDate = now,
                TenantId = currentUser.TenantId
            };

            // Update asset status
            asset.Status = AssetStatus.Damaged;
            asset.LastModifiedByUserId = currentUser.UserId;
            asset.LastModifiedByUserName = currentUser.UserName;
            asset.LastModifiedOnDate = now;

            _dbContext.sm_AssetIncident.Add(incident);
            _dbContext.sm_AssetUsageHistory.Add(history);
            _dbContext.sm_Asset.Update(asset);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetViewModel>(asset), "Báo hỏng thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: AssetId: {@assetId}, Model: {@model}", assetId, model);
            return Helper.CreateExceptionResponse<AssetViewModel>(e);
        }
    }

    public async Task<Response<AssetViewModel>> ReportLost(Guid assetId, AssetReportLostModel model)
    {
        try
        {
            var asset = await _dbContext.sm_Asset.FindAsync(assetId);

            if (asset == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không tồn tại");
            }

            // Validate asset status
            var allowedStatuses = new[]
            {
                AssetStatus.InUse,
                AssetStatus.OutOfUse,
                AssetStatus.UnderMaintenance,
                AssetStatus.Damaged
            };

            if (!allowedStatuses.Contains(asset.Status))
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>(
                    "Chỉ được báo mất tài sản đang sử dụng, chưa sử dụng, đang bảo trì hoặc đã hỏng");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var now = DateTime.Now;

            // Create AssetIncident
            var incident = new sm_AssetIncident
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                IncidentType = AssetBusinessOperation.ReportLost,
                IncidentDate = model.IncidentDate,
                CompensationAmount = model.CompensationAmount,
                Description = model.Description,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                LastModifiedOnDate = now,
                TenantId = currentUser.TenantId
            };

            // Create AssetUsageHistory
            var history = new sm_AssetUsageHistory
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                Operation = AssetBusinessOperation.ReportLost,
                AssetStatus = AssetStatus.Lost,
                ExecutionDate = model.IncidentDate,
                Description = model.Description,
                LocationId = asset.AssetLocationId,
                UserId = asset.UserId,
                Cost = model.CompensationAmount,
                EntityType = "AssetIncident",
                EntityId = incident.Id,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                LastModifiedOnDate = now,
                TenantId = currentUser.TenantId
            };

            // Update asset status
            asset.Status = AssetStatus.Lost;
            asset.LastModifiedByUserId = currentUser.UserId;
            asset.LastModifiedByUserName = currentUser.UserName;
            asset.LastModifiedOnDate = now;

            _dbContext.sm_AssetIncident.Add(incident);
            _dbContext.sm_AssetUsageHistory.Add(history);
            _dbContext.sm_Asset.Update(asset);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetViewModel>(asset), "Báo mất thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: AssetId: {@assetId}, Model: {@model}", assetId, model);
            return Helper.CreateExceptionResponse<AssetViewModel>(e);
        }
    }

    public async Task<Response<AssetViewModel>> ReportDestroyed(Guid assetId, AssetReportDestroyedModel model)
    {
        try
        {
            var asset = await _dbContext.sm_Asset.FindAsync(assetId);

            if (asset == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không tồn tại");
            }

            // Validate asset status
            var allowedStatuses = new[]
            {
                AssetStatus.OutOfUse,
                AssetStatus.Damaged
            };

            if (!allowedStatuses.Contains(asset.Status))
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>(
                    "Chỉ được báo huỷ tài sản chưa sử dụng hoặc đã hỏng");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var now = DateTime.Now;

            // Create AssetIncident
            var incident = new sm_AssetIncident
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                IncidentType = AssetBusinessOperation.ReportDestroyed,
                IncidentDate = model.IncidentDate,
                Description = model.Description,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                LastModifiedOnDate = now,
                TenantId = currentUser.TenantId
            };

            // Create AssetUsageHistory
            var history = new sm_AssetUsageHistory
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                Operation = AssetBusinessOperation.ReportDestroyed,
                AssetStatus = AssetStatus.Destroyed,
                ExecutionDate = model.IncidentDate,
                Description = model.Description,
                LocationId = asset.AssetLocationId,
                UserId = asset.UserId,
                EntityType = "AssetIncident",
                EntityId = incident.Id,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                LastModifiedOnDate = now,
                TenantId = currentUser.TenantId
            };

            // Update asset status
            asset.Status = AssetStatus.Destroyed;
            asset.LastModifiedByUserId = currentUser.UserId;
            asset.LastModifiedByUserName = currentUser.UserName;
            asset.LastModifiedOnDate = now;

            _dbContext.sm_AssetIncident.Add(incident);
            _dbContext.sm_AssetUsageHistory.Add(history);
            _dbContext.sm_Asset.Update(asset);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetViewModel>(asset), "Báo huỷ thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: AssetId: {@assetId}, Model: {@model}", assetId, model);
            return Helper.CreateExceptionResponse<AssetViewModel>(e);
        }
    }

    public async Task<Response<AssetViewModel>> Allocate(Guid assetId, AssetAllocateModel model)
    {
        try
        {
            // Validate asset exists and status
            var asset = await _dbContext.sm_Asset.FindAsync(assetId);
            if (asset == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không tồn tại");
            }

            if (asset.Status != AssetStatus.OutOfUse)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không ở trạng thái có thể cấp phát");
            }

            // Validate user exists
            var user = await _dbContext.IdmUser.FindAsync(model.UserId);
            if (user == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Người dùng không tồn tại");
            }

            // Validate location exists
            var location = await _dbContext.sm_AssetLocation.FindAsync(model.AssetLocationId);
            if (location == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Vị trí không tồn tại");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var now = DateTime.Now;

            // Create asset allocation record
            var allocation = new sm_AssetAllocation
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                Operation = AssetBusinessOperation.Allocate,
                ExecutionDate = model.ExecutionDate,
                FromLocationId = asset.AssetLocationId,
                ToLocationId = model.AssetLocationId,
                FromUserId = asset.UserId,
                ToUserId = model.UserId,
                Description = model.Description,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                LastModifiedOnDate = now,
                TenantId = currentUser.TenantId,
                Status = AllocationStatus.Pending
            };
            var Code = Utils.RandomInt(6);
            while (_dbContext.sm_AssetAllocation.Any(x => x.Code == Code))
                Code = Utils.RandomInt(6);
            allocation.Code = Code.ToString();
            var locationObject = await _dbContext.sm_AssetLocation.FindAsync(model.AssetLocationId);
            var locationName = location.Name;


            SendGmail(user?.Email, "Thông báo cấp phát tài sản", string.Format(
                System.IO.File.ReadAllText(@"Resources/EmailTemplate/allocate-asset.html"),
                user.Name, locationName, asset.Name, asset.Code, model.ExecutionDate, model.Description,
                "http://quanlytaisan.geneat.io.vn/#/vn/allocate-email?assetId=" + assetId + "&allocate=true&code=" +
                Code + "&userId=" + currentUser.UserId + "&idAssetAllocation=" + allocation.Id,
                "http://quanlytaisan.geneat.io.vn/#/vn/revoke-email?assetId=" + assetId + "&revoke=true&code=" + Code +
                "&userId=" + currentUser.UserId + "&idAssetAllocation=" + allocation.Id, user.Email, user.PhoneNumber, user.UserName));
            // Create usage history record
            //var usageHistory = new sm_AssetUsageHistory
            //{
            //    Id = Guid.NewGuid(),
            //    AssetId = assetId,
            //    Operation = AssetBusinessOperation.Allocate,
            //    LocationId = model.AssetLocationId,
            //    UserId = model.UserId,
            //    EntityType = "AssetAllocation",
            //    EntityId = allocation.Id,
            //    CreatedByUserId = currentUser.UserId,
            //    CreatedByUserName = currentUser.UserName,
            //    CreatedOnDate = now,
            //    LastModifiedByUserId = currentUser.UserId,
            //    LastModifiedByUserName = currentUser.UserName,
            //    LastModifiedOnDate = now,
            //    TenantId = currentUser.TenantId
            //};

            // Update asset
            //asset.Status = AssetStatus.InUse;
            //asset.AssetLocationId = model.AssetLocationId;
            //asset.UserId = model.UserId;
            //asset.LastModifiedByUserId = currentUser.UserId;
            //asset.LastModifiedByUserName = currentUser.UserName;
            //asset.LastModifiedOnDate = now;

            // Save changes
            _dbContext.sm_AssetAllocation.Add(allocation);
            //_dbContext.sm_AssetUsageHistory.Add(usageHistory);
            //_dbContext.sm_Asset.Update(asset);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetViewModel>(asset),
                "Đã gửi mail về cho người cấp phát thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: AssetId: {@assetId}, Model: {@model}", assetId, model);
            return Helper.CreateExceptionResponse<AssetViewModel>(e);
        }
    }

    public async Task<Response<AssetViewModel>> Revoke(Guid assetId, AssetAllocateModel model)
    {
        try
        {
            // Validate asset exists and status
            var asset = await _dbContext.sm_Asset.FindAsync(assetId);
            if (asset == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không tồn tại");
            }

            if (asset.Status != AssetStatus.InUse)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không ở trạng thái có thể thu hồi");
            }

            var user = await _dbContext.IdmUser.FindAsync(model.UserId);
            if (user == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Người dùng không tồn tại");
            }

            // Validate location exists
            var location = await _dbContext.sm_AssetLocation.FindAsync(model.AssetLocationId);
            if (location == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Vị trí không tồn tại");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var now = DateTime.Now;

            // Create asset allocation record
            var allocation = new sm_AssetAllocation
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                Operation = AssetBusinessOperation.Revoke,
                ExecutionDate = model.ExecutionDate,
                FromLocationId = asset.AssetLocationId,
                ToLocationId = model.AssetLocationId,
                FromUserId = asset.UserId,
                ToUserId = model.UserId,
                Description = model.Description,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                LastModifiedOnDate = now,
                TenantId = currentUser.TenantId,
                Status = AllocationStatus.Pending
            };
            var Code = Utils.RandomInt(6);
            while (_dbContext.sm_AssetAllocation.Any(x => x.Code == Code))
                Code = Utils.RandomInt(6);
            allocation.Code = Code.ToString();
            var locationObject = await _dbContext.sm_AssetLocation.FindAsync(model.AssetLocationId);
            var locationName = location.Name;


            //SendGmailAllocate(user.Email, "Thông báo cấp phát tài sản", "1234");
            SendGmail(user?.Email, "Thông báo thu hồi tài sản", string.Format(
                System.IO.File.ReadAllText(@"Resources/EmailTemplate/revoke-asset.html"),
                user.Name, locationName, asset.Name, asset.Code, model.ExecutionDate, model.Description,
                "http://quanlytaisan.geneat.io.vn/#/vn/revoke-email?assetId=" + assetId + "&allocate=true&code=" +
                Code + "&userId=" + currentUser.UserId + "&idAssetAllocation=" + allocation.Id,
                "http://quanlytaisan.geneat.io.vn/#/vn/revoke-email?assetId=" + assetId + "&revoke=true&code=" + Code +
                "&userId=" + currentUser.UserId + "&idAssetAllocation=" + allocation.Id, user.Email, user.PhoneNumber, user.UserName));
            // Create usage history record
            //var history = new sm_AssetUsageHistory
            //{
            //    Id = Guid.NewGuid(),
            //    AssetId = assetId,
            //    Operation = AssetBusinessOperation.Revoke,
            //    AssetStatus = AssetStatus.OutOfUse,
            //    LocationId = model.AssetLocationId,
            //    UserId = null,
            //    EntityType = "AssetAllocation",
            //    EntityId = allocation.Id,
            //    CreatedByUserId = currentUser.UserId,
            //    CreatedByUserName = currentUser.UserName,
            //    CreatedOnDate = now,
            //    LastModifiedByUserId = currentUser.UserId,
            //    LastModifiedByUserName = currentUser.UserName,
            //    LastModifiedOnDate = now,
            //    TenantId = currentUser.TenantId
            //};

            //// Update asset
            //asset.Status = AssetStatus.OutOfUse;
            //asset.AssetLocationId = model.AssetLocationId;
            //asset.UserId = null;
            //asset.LastModifiedByUserId = currentUser.UserId;
            //asset.LastModifiedByUserName = currentUser.UserName;
            //asset.LastModifiedOnDate = now;

            // Save changes
            _dbContext.sm_AssetAllocation.Add(allocation);
            //_dbContext.sm_AssetUsageHistory.Add(usageHistory);
            //_dbContext.sm_Asset.Update(asset);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetViewModel>(asset),
                "Đã gửi mail về cho người thu hồi tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: AssetId: {@assetId}, Model: {@model}", assetId, model);
            return Helper.CreateExceptionResponse<AssetViewModel>(e);
        }
    }

    public async Task<Response<AssetViewModel>> Transfer(Guid assetId, AssetAllocateModel model)
    {
        try
        {
            // Validate asset exists and status
            var asset = await _dbContext.sm_Asset.FindAsync(assetId);
            if (asset == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không tồn tại");
            }

            if (asset.Status != AssetStatus.InUse)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không ở trạng thái có thể điều chuyển");
            }

            var user = await _dbContext.IdmUser.FindAsync(model.UserId);
            if (user == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Người dùng không tồn tại");
            }

            // Validate location exists
            var location = await _dbContext.sm_AssetLocation.FindAsync(model.AssetLocationId);
            if (location == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Vị trí không tồn tại");
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var now = DateTime.Now;

            // Create asset allocation record
            var allocation = new sm_AssetAllocation
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                Operation = AssetBusinessOperation.Transfer,
                ExecutionDate = model.ExecutionDate,
                FromLocationId = asset.AssetLocationId,
                ToLocationId = model.AssetLocationId,
                FromUserId = asset.UserId,
                ToUserId = model.UserId,
                Description = model.Description,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                LastModifiedOnDate = now,
                TenantId = currentUser.TenantId,
                Status = AllocationStatus.Pending
            };
            var Code = Utils.RandomInt(6);
            while (_dbContext.sm_AssetAllocation.Any(x => x.Code == Code))
                Code = Utils.RandomInt(6);
            allocation.Code = Code.ToString();
            var locationObject = await _dbContext.sm_AssetLocation.FindAsync(model.AssetLocationId);
            var locationName = location.Name;


            //SendGmailAllocate(user.Email, "Thông báo cấp phát tài sản", "1234");
            SendGmail(user?.Email, "Thông báo điều chuyển tài sản", string.Format(
                System.IO.File.ReadAllText(@"Resources/EmailTemplate/revoke-asset.html"),
                user.Name, locationName, asset.Name, asset.Code, model.ExecutionDate, model.Description,
                "http://quanlytaisan.geneat.io.vn/#/vn/revoke-email?assetId=" + assetId + "&allocate=true&code=" +
                Code + "&userId=" + currentUser.UserId + "&idAssetAllocation=" + allocation.Id + "&isTransfer=true",
                "http://quanlytaisan.geneat.io.vn/#/vn/revoke-email?assetId=" + assetId + "&revoke=true&code=" + Code +
                "&userId=" + currentUser.UserId + "&idAssetAllocation=" + allocation.Id + "&isTransfer=true", user.Email, user.PhoneNumber, user.UserName));
            // Create usage history record
            //var history = new sm_AssetUsageHistory
            //{
            //    Id = Guid.NewGuid(),
            //    AssetId = assetId,
            //    Operation = AssetBusinessOperation.Transfer,
            //    AssetStatus = AssetStatus.InUse,
            //    ExecutionDate = allocation.ExecutionDate,
            //    Description = allocation.Description,
            //    LocationId = model.AssetLocationId,
            //    UserId = model.UserId,
            //    EntityType = "AssetAllocation",
            //    EntityId = allocation.Id,
            //    CreatedByUserId = currentUser.UserId,
            //    CreatedByUserName = currentUser.UserName,
            //    CreatedOnDate = now,
            //    LastModifiedByUserId = currentUser.UserId,
            //    LastModifiedByUserName = currentUser.UserName,
            //    LastModifiedOnDate = now,
            //    TenantId = currentUser.TenantId
            //};

            //// Update asset
            //asset.AssetLocationId = model.AssetLocationId;
            //asset.UserId = model.UserId;
            //asset.LastModifiedByUserId = currentUser.UserId;
            //asset.LastModifiedByUserName = currentUser.UserName;
            //asset.LastModifiedOnDate = now;

            // Save changes
            _dbContext.sm_AssetAllocation.Add(allocation);
            //_dbContext.sm_AssetUsageHistory.Add(history);
            //_dbContext.sm_Asset.Update(asset);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<AssetViewModel>(asset),
                "Đã gửi mail về cho người điều chuyển tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: AssetId: {@assetId}, Model: {@model}", assetId, model);
            return Helper.CreateExceptionResponse<AssetViewModel>(e);
        }
    }

    public async void SendGmail(string email, string subject, string content)
    {
        try
        {
            var message = new EmailMessage
            {
                To = new[] { email },
                Subject = subject,
                Content = content
            };
            await _emailService.SendAsync(message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
        }
    }

    public async Task<Response> AllocateEmail(Guid assetId, AssetApprovalModel assetApproval)
    {
        try
        {
            // Validate asset exists and status
            var asset = await _dbContext.sm_Asset.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == assetId);
            if (asset == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không tồn tại");
            }

            var allocation = await _dbContext.sm_AssetAllocation
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.AssetId == assetId && x.Code == assetApproval.Code);
            if (allocation == null)
            {
                return Helper.CreateBadRequestResponse("Mã code đã quá hạn hoặc không hợp lệ");
            }

            // Validate user exists
            var user = await _dbContext.IdmUser.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == allocation.ToUserId);
            if (user == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Người dùng không tồn tại");
            }

            // Validate location exists
            var userAllocate = await _dbContext.IdmUser.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == assetApproval.UserId);
            var now = DateTime.Now;

            // Update asset allocation record
            //var allocation = new sm_AssetAllocation
            //{
            //    Id = Guid.NewGuid(),
            //    AssetId = assetId,
            //    Operation = AssetBusinessOperation.Allocate,
            //    ExecutionDate = model.ExecutionDate,
            //    FromLocationId = asset.AssetLocationId,
            //    ToLocationId = model.AssetLocationId,
            //    FromUserId = asset.UserId,
            //    ToUserId = model.UserId,
            //    Description = model.Description,
            //    CreatedByUserId = currentUser.UserId,
            //    CreatedByUserName = currentUser.UserName,
            //    CreatedOnDate = now,
            //    LastModifiedByUserId = currentUser.UserId,
            //    LastModifiedByUserName = currentUser.UserName,
            //    LastModifiedOnDate = now,
            //    TenantId = currentUser.TenantId
            //};
            if (assetApproval.IsAllocate)
                allocation.Status = AllocationStatus.Accepted;
            else allocation.Status = AllocationStatus.Rejected;
            allocation.Code = null;
            allocation.Description = assetApproval.Description;
            _dbContext.sm_AssetAllocation.Update(allocation);
            if (assetApproval.IsAllocate == false)
            {
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("Từ chối yêu cầu cấp phát thành công");
            }

            // Create usage history record
            var usageHistory = new sm_AssetUsageHistory
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                Operation = AssetBusinessOperation.Allocate,
                AssetStatus = AssetStatus.InUse,
                ExecutionDate = allocation.ExecutionDate,
                Description = allocation.Description,
                LocationId = allocation.ToLocationId,
                UserId = user.Id,
                EntityType = "AssetAllocation",
                EntityId = allocation.Id,
                CreatedByUserId = userAllocate.Id,
                CreatedByUserName = userAllocate.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = userAllocate.Id,
                LastModifiedByUserName = userAllocate.UserName,
                LastModifiedOnDate = now,
                TenantId = userAllocate.TenantId
            };

            // Update asset
            asset.Status = AssetStatus.InUse;
            asset.AssetLocationId = (Guid)allocation.ToLocationId;
            asset.UserId = user.Id;
            asset.LastModifiedByUserId = userAllocate.Id;
            asset.LastModifiedByUserName = userAllocate.UserName;
            asset.LastModifiedOnDate = now;

            // Save changes
            _dbContext.sm_AssetUsageHistory.Add(usageHistory);
            _dbContext.sm_Asset.Update(asset);
            await _dbContext.SaveChangesAsync();
            return Helper.CreateSuccessResponse("Cấp phát tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: AssetId: {@assetId}, Code: {@code}", assetId, assetApproval.Code);
            return Helper.CreateExceptionResponse<AssetViewModel>(e);
        }
    }

    public async Task<Response> RevokeEmail(Guid assetId, AssetApprovalModel assetApproval)
    {
        try
        {
            // Validate asset exists and status
            var asset = await _dbContext.sm_Asset.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == assetId);
            if (asset == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không tồn tại");
            }

            var allocation = await _dbContext.sm_AssetAllocation
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.AssetId == assetId && x.Code == assetApproval.Code);
            if (allocation == null)
            {
                return Helper.CreateBadRequestResponse("Mã code đã quá hạn hoặc không hợp lệ");
            }

            // Validate user exists
            var user = await _dbContext.IdmUser.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == allocation.ToUserId);
            if (user == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Người dùng không tồn tại");
            }

            // Validate location exists
            var userAllocate = await _dbContext.IdmUser.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == assetApproval.UserId);
            var now = DateTime.Now;

            // Update asset allocation record
            //var allocation = new sm_AssetAllocation
            //{
            //    Id = Guid.NewGuid(),
            //    AssetId = assetId,
            //    Operation = AssetBusinessOperation.Allocate,
            //    ExecutionDate = model.ExecutionDate,
            //    FromLocationId = asset.AssetLocationId,
            //    ToLocationId = model.AssetLocationId,
            //    FromUserId = asset.UserId,
            //    ToUserId = model.UserId,
            //    Description = model.Description,
            //    CreatedByUserId = currentUser.UserId,
            //    CreatedByUserName = currentUser.UserName,
            //    CreatedOnDate = now,
            //    LastModifiedByUserId = currentUser.UserId,
            //    LastModifiedByUserName = currentUser.UserName,
            //    LastModifiedOnDate = now,
            //    TenantId = currentUser.TenantId
            //};
            if (assetApproval.IsAllocate)
                allocation.Status = AllocationStatus.Accepted;
            else allocation.Status = AllocationStatus.Rejected;
            allocation.Code = null;
            allocation.Description = assetApproval.Description;
            _dbContext.sm_AssetAllocation.Update(allocation);
            if (assetApproval.IsAllocate == false)
            {
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("Từ chối yêu cầu thu hồi thành công");
            }

            // Create usage history record
            var usageHistory = new sm_AssetUsageHistory
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                Operation = AssetBusinessOperation.Revoke,
                AssetStatus = AssetStatus.OutOfUse,
                ExecutionDate = allocation.ExecutionDate,
                Description = allocation.Description,
                LocationId = allocation.ToLocationId,
                UserId = null,
                EntityType = "AssetAllocation",
                EntityId = allocation.Id,
                CreatedByUserId = userAllocate.Id,
                CreatedByUserName = userAllocate.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = userAllocate.Id,
                LastModifiedByUserName = userAllocate.UserName,
                LastModifiedOnDate = now,
                TenantId = userAllocate.TenantId
            };

            // Update asset
            asset.Status = AssetStatus.OutOfUse;
            asset.AssetLocationId = (Guid)allocation.ToLocationId;
            asset.UserId = null;
            asset.LastModifiedByUserId = userAllocate.Id;
            asset.LastModifiedByUserName = userAllocate.UserName;
            asset.LastModifiedOnDate = now;
            // Save changes
            _dbContext.sm_AssetUsageHistory.Add(usageHistory);
            _dbContext.sm_Asset.Update(asset);
            await _dbContext.SaveChangesAsync();
            return Helper.CreateSuccessResponse("Thu hồi tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: AssetId: {@assetId}, Code: {@code}", assetId, assetApproval.Code);
            return Helper.CreateExceptionResponse<AssetViewModel>(e);
        }
    }

    public async Task<Response> TransferEmail(Guid assetId, AssetApprovalModel assetApproval)
    {
        try
        {
            // Validate asset exists and status
            var asset = await _dbContext.sm_Asset.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == assetId);
            if (asset == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Tài sản không tồn tại");
            }

            var allocation = await _dbContext.sm_AssetAllocation
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.AssetId == assetId && x.Code == assetApproval.Code);
            if (allocation == null)
            {
                return Helper.CreateBadRequestResponse("Mã code đã quá hạn hoặc không hợp lệ");
            }

            // Validate user exists
            var user = await _dbContext.IdmUser.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == allocation.ToUserId);
            if (user == null)
            {
                return Helper.CreateBadRequestResponse<AssetViewModel>("Người dùng không tồn tại");
            }

            // Validate location exists
            var userAllocate = await _dbContext.IdmUser.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == assetApproval.UserId);
            var now = DateTime.Now;

            // Update asset allocation record
            //var allocation = new sm_AssetAllocation
            //{
            //    Id = Guid.NewGuid(),
            //    AssetId = assetId,
            //    Operation = AssetBusinessOperation.Allocate,
            //    ExecutionDate = model.ExecutionDate,
            //    FromLocationId = asset.AssetLocationId,
            //    ToLocationId = model.AssetLocationId,
            //    FromUserId = asset.UserId,
            //    ToUserId = model.UserId,
            //    Description = model.Description,
            //    CreatedByUserId = currentUser.UserId,
            //    CreatedByUserName = currentUser.UserName,
            //    CreatedOnDate = now,
            //    LastModifiedByUserId = currentUser.UserId,
            //    LastModifiedByUserName = currentUser.UserName,
            //    LastModifiedOnDate = now,
            //    TenantId = currentUser.TenantId
            //};
            if (assetApproval.IsAllocate)
                allocation.Status = AllocationStatus.Accepted;
            else allocation.Status = AllocationStatus.Rejected;
            allocation.Code = null;
            allocation.Description = assetApproval.Description;
            _dbContext.sm_AssetAllocation.Update(allocation);
            if (assetApproval.IsAllocate == false)
            {
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("Từ chối yêu cầu điều chuyển thành công");
            }
            var history = new sm_AssetUsageHistory
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                Operation = AssetBusinessOperation.Transfer,
                AssetStatus = AssetStatus.InUse,
                ExecutionDate = allocation.ExecutionDate,
                Description = allocation.Description,
                LocationId = allocation.ToLocationId,
                UserId = allocation.ToUserId,
                EntityType = "AssetAllocation",
                EntityId = allocation.Id,
                CreatedByUserId = userAllocate.Id,
                CreatedByUserName = userAllocate.UserName,
                CreatedOnDate = now,
                LastModifiedByUserId = userAllocate.Id,
                LastModifiedByUserName = userAllocate.UserName,
                LastModifiedOnDate = now,
                TenantId = userAllocate.TenantId
            };

            // Update asset
            asset.AssetLocationId = (Guid)allocation.ToLocationId;
            asset.UserId = allocation.ToUserId;
            asset.LastModifiedByUserId = userAllocate.Id;
            asset.LastModifiedByUserName = userAllocate.UserName;
            asset.LastModifiedOnDate = now;

            // Save changes
            _dbContext.sm_AssetUsageHistory.Add(history);
            _dbContext.sm_Asset.Update(asset);
            await _dbContext.SaveChangesAsync();
            return Helper.CreateSuccessResponse("Điều chuyển tài sản thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: AssetId: {@assetId}, Code: {@code}", assetId, assetApproval.Code);
            return Helper.CreateExceptionResponse<AssetViewModel>(e);
        }
    }
}