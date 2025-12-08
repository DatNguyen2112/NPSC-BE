using NSPC.Common;

namespace NSPC.Business.Services.TaskUsageHistory
{
    public interface ITaskUsageHistoryHandler
    {
        Task<Response<TaskUsageHistoryViewModel>> GetById(Guid id);
        Task<Response<Pagination<TaskUsageHistoryViewModel>>> GetPage(TaskUsageHistoryQueryModel query);
    }
}
