using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface ITaskCommentHandler
    {
        Task<Response<TaskCommentViewModel>> Create(TaskCommentCreateModel model, RequestUser currentUser);

        Task<Response<TaskCommentViewModel>> Update(Guid Id, TaskCommentCreateModel model, RequestUser currentUser);

        Task<Response<TaskCommentViewModel>> Delete(Guid Id, RequestUser currentUser);

        Task<Response<Pagination<TaskCommentViewModel>>> GetPageAsync(TaskQueryModel query);
    }
}

