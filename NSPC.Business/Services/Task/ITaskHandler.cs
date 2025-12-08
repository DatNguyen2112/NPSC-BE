using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.WorkItem
{
    public interface ITaskHandler
    {
        Task<Response<TaskViewModel>> Create(TaskCreateUpdateModel model, RequestUser currentUser);
        Task<Response<TaskViewModel>> Update(Guid id, TaskCreateUpdateModel model, RequestUser currentUser);
        Task<Response<TaskViewModel>> GetById(Guid id);
        Task<Response<Pagination<TaskViewModel>>> GetPage(TaskQueryModel query);
        Task<Response<TaskViewModel>> Delete(Guid id);
        Task<Response> DeleteMany(List<Guid> ids);
        Task<Response<TaskViewModel>> UpdateStatus(Guid id, string status, string description);
        Task<Response<Pagination<TaskViewModel>>> GetPageByConstructionId(Guid constructionId, TaskQueryModel query);
        Task<Response<TaskOverviewEachStage>> GetAnalyzeByEachStage(Guid idTemplateStage, Guid constructionId);
        Task<Response<List<TaskViewModel>>> UpdateStatusMany(List<Guid> ids, string status, string description);
        Task CheckAndNotifyTasksExpiringSoon();
        Task<Response<int>> GetMaxPriorityOrderByConstructionIdAndTemplateStage(Guid constructionId, Guid idTemplateStage);
        Task<Response<TaskStatusSummaryViewModel>> GetTaskStatusSummary(TaskQueryModel query);
        Task<Response<TaskOverviewSummaryViewModel>> GetTaskOverviewSummary(TaskQueryModel query);
    }
}
