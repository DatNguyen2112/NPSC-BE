using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data.Data;
using SaleManagement.Data.Data.Entity.TaskHistory;
using Serilog;
using System.Linq.Expressions;
namespace NSPC.Business.Services.TaskUsageHistory
{
    public class TaskUsageHistoryHandler : ITaskUsageHistoryHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public TaskUsageHistoryHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<TaskUsageHistoryViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_TaskUsageHistory
                    .AsNoTracking()
                    .Include(x => x.Task)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<TaskUsageHistoryViewModel>("Lịch sử công việc không tồn tại trong hệ thống.");

                var result = _mapper.Map<TaskUsageHistoryViewModel>(entity);
                return new Response<TaskUsageHistoryViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<TaskUsageHistoryViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<TaskUsageHistoryViewModel>>> GetPage(TaskUsageHistoryQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_TaskUsageHistory
                    .AsNoTracking().Include(x => x.Task).Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<TaskUsageHistoryViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<TaskUsageHistoryViewModel>>(ex);
            }
        }
        private Expression<Func<sm_TaskUsageHistory, bool>> BuildQuery(TaskUsageHistoryQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_TaskUsageHistory>(true);
            //if (!string.IsNullOrEmpty(query.FullTextSearch))
            //    predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()));

            return predicate;
        }
    }
}
