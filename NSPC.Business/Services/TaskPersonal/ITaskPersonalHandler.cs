using NSPC.Business.Services.WorkItem;
using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface ITaskPersonalHandler
    {
        Task<Response<TaskPersonalViewModel>> Create(TaskPersonalCreateModel model, RequestUser currentUser);
        Task<Response<TaskPersonalViewModel>> Update(Guid id, TaskPersonalCreateModel model, RequestUser currentUser);
        Task<Response<TaskPersonalViewModel>> GetById(Guid id);
        Task<Response<Pagination<TaskPersonalViewModel>>> GetPage(TaskPersonalQueryModel query);
        Task<Response<TaskPersonalViewModel>> Delete(Guid id);
        Task<Response> DeleteMany(List<Guid> ids);
        Task<Response<TaskPersonalViewModel>> UpdateStatus(Guid id, string status);
        Task<Response<List<TaskPersonalViewModel>>> UpdateStatusMany(List<Guid> ids, string status);
        Task<Response<TaskStatusSummaryViewModel>> GetTaskStatusSummary(TaskPersonalQueryModel query);
    }
}

