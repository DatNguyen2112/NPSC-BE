using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data.Data.Entity.AssetHistory;
using Serilog;
using System.Linq.Expressions;
using NSPC.Data.Data;

namespace NSPC.Business;

public class AssetUsageHistoryHandler : IAssetUsageHistoryHandler
{
    private readonly SMDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AssetUsageHistoryHandler(
        SMDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<Pagination<AssetUsageHistoryViewModel>>> GetPage(AssetUsageHistoryQueryModel query)
    {
        try
        {
            var predicate = BuildQuery(query);
            var queryResult = _dbContext.sm_AssetUsageHistory
                .Include(x => x.Asset)
                .Include(x => x.Location)
                .Include(x => x.User)
                .Include(x => x.CreatedByUser)
                .Where(predicate);
            var data = await queryResult.GetPageAsync(query);
            var result = _mapper.Map<Pagination<AssetUsageHistoryViewModel>>(data);

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            Log.Information("Params: Model: {@query}", query);
            return Helper.CreateExceptionResponse<Pagination<AssetUsageHistoryViewModel>>(ex);
        }
    }

    public async Task<Response<AssetUsageHistoryViewModel>> GetById(Guid id)
    {
        try
        {
            var history = await _dbContext.sm_AssetUsageHistory
                .Include(x => x.Asset)
                .Include(x => x.Location)
                .Include(x => x.User)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (history == null)
            {
                return Helper.CreateBadRequestResponse<AssetUsageHistoryViewModel>("Lịch sử sử dụng không tồn tại");
            }

            return Helper.CreateSuccessResponse(_mapper.Map<AssetUsageHistoryViewModel>(history));
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@id}", id);
            return Helper.CreateExceptionResponse<AssetUsageHistoryViewModel>(e);
        }
    }

    private Expression<Func<sm_AssetUsageHistory, bool>> BuildQuery(AssetUsageHistoryQueryModel query)
    {
        var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
        var predicate = PredicateBuilder.New<sm_AssetUsageHistory>(true);

        if (currentUser.TenantId != null)
        {
            predicate.And(x => x.TenantId == currentUser.TenantId);
        }

        if (query.AssetId.HasValue)
        {
            predicate.And(x => x.AssetId == query.AssetId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Operation))
        {
            if (Enum.TryParse<AssetBusinessOperation>(query.Operation, out var operation))
            {
                predicate.And(x => x.Operation == operation);
            }
        }

        if (query.LocationId.HasValue)
        {
            predicate.And(x => x.LocationId == query.LocationId.Value);
        }

        if (query.FromDate.HasValue)
        {
            predicate.And(x => x.CreatedOnDate.Date >= query.FromDate.Value.Date);
        }

        if (query.ToDate.HasValue)
        {
            predicate.And(x => x.CreatedOnDate.Date <= query.ToDate.Value.Date);
        }

        if (query.UserId.HasValue)
        {
            predicate.And(x => x.UserId == query.UserId.Value);
        }

        return predicate;
    }
}