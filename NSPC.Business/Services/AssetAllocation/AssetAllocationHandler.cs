using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.AssetAllocation;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.AssetAllocation;
using Serilog;
using System.Linq.Expressions;
namespace NSPC.Business;

public class AssetAllocationHandler : IAssetAllocationHandler
{
    private readonly SMDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AssetAllocationHandler(
        SMDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<Pagination<AssetAllocationViewModel>>> GetPage(AssetAllocationQueryModel query)
    {
        try
        {
            var predicate = BuildQuery(query);
            var queryResult = _dbContext.sm_AssetAllocation.Include(x => x.ToUser).Where(predicate);
            var data = await queryResult.GetPageAsync(query);
            var result = _mapper.Map<Pagination<AssetAllocationViewModel>>(data);

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Query: {@query}", query);
            return Helper.CreateExceptionResponse<Pagination<AssetAllocationViewModel>>(e);
        }
    }

    public async Task<Response<AssetAllocationViewModel>> GetById(Guid id)
    {
        try
        {
            var entity = await _dbContext.sm_AssetAllocation
                .IgnoreQueryFilters()
                .Include(x => x.Asset)
                .Include(x => x.ToUser)
                .ThenInclude(x => x.mk_ChucVu)
                .Include(x => x.ToLocation)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return Helper.CreateBadRequestResponse<AssetAllocationViewModel>("Phân bổ tài sản không tồn tại");
            }

            return Helper.CreateSuccessResponse(_mapper.Map<AssetAllocationViewModel>(entity));
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Id: {@id}", id);
            return Helper.CreateExceptionResponse<AssetAllocationViewModel>(e);
        }
    }

    private Expression<Func<sm_AssetAllocation, bool>> BuildQuery(AssetAllocationQueryModel query)
    {
        var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
        var predicate = PredicateBuilder.New<sm_AssetAllocation>(true);

        if (query.IsActive == true)
        {
            predicate = predicate.And(x => x.Status == AllocationStatus.Rejected || x.Status == AllocationStatus.Accepted);
        }

        return predicate;
    }
}