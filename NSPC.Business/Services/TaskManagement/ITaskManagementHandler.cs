using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business
{
    public interface ITaskManagementHandler
    {
        #region TaskManagement
        Task<Response<TaskManagementViewModel>> CreateTask(TaskManagementCreateUpdateModel model, RequestUser currentUser);
        Task<Response<List<TaskManagementViewModel>>> CreateMultiTask(List<TaskManagementCreateUpdateModel> model, RequestUser currentUser);
        Task<Response<Pagination<TaskManagementViewModel>>> GetPageTask(TaskManagementQueryModel query, RequestUser currentUser);
        Task<Response<TaskSummanryModel>> GetSummaryTask(TaskManagementQueryModel query, RequestUser currentUser);
        Task<Response<TaskManagementViewModel>> GetByIdTask(Guid id, RequestUser currentUser);
        Task<Response<TaskManagementViewModel>> UpdateTask(Guid id, TaskManagementCreateUpdateModel model, RequestUser currentUser);
        Task<Response> DeleteTask(Guid id, RequestUser currentUser);
        #endregion

        #region TaskManagementAssignee
        Task<Response<TaskManagementAssigneeViewModel>> CreateTaskAssignee(TaskManagementAssigneeCreateUpdateModel model, RequestUser currentUser);
        Task<Response<Pagination<TaskManagementAssigneeViewModel>>> GetPageTaskAssignee(TaskManagementAssigneeQueryModel query, RequestUser currentUser);
        Task<Response<TaskManagementAssigneeViewModel>> GetByIdTaskAssignee(Guid id, RequestUser currentUser);
        //Task<Response<TaskManagementAssigneeViewModel>> UpdateTaskAssignee(Guid id, TaskManagementAssigneeCreateUpdateModel model, RequestUser currentUser);
        Task<Response> DeleteTaskAssignee(Guid id, RequestUser currentUser);
        #endregion

        #region TaskManagementComment
        Task<Response<TaskManagementCommentViewModel>> CreateTaskComment(TaskManagementCommentCreateUpdateModel model, RequestUser currentUser);
        Task<Response<Pagination<TaskManagementCommentViewModel>>> GetPageTaskComment(TaskManagementCommentQueryModel query, RequestUser currentUser);
        Task<Response<TaskManagementCommentViewModel>> GetByIdTaskComment(Guid id, RequestUser currentUser);
        Task<Response<TaskManagementCommentViewModel>> UpdateTaskComment(Guid id, TaskManagementCommentCreateUpdateModel model, RequestUser currentUser);
        Task<Response> DeleteTaskComment(Guid id, RequestUser currentUser);
        #endregion

        #region TaskManagementHistory
        Task<Response<TaskManagementHistoryViewModel>> CreateTaskHistory(TaskManagementHistoryCreateUpdateModel model, RequestUser currentUser);
        Task<Response<Pagination<TaskManagementHistoryViewModel>>> GetPageTaskHistory(TaskManagementHistoryQueryModel query, RequestUser currentUser);
        Task<Response<TaskManagementHistoryViewModel>> GetByIdTaskHistory(Guid id, RequestUser currentUser);
        //Task<Response<TaskManagementHistoryViewModel>> UpdateTaskHistory(Guid id, TaskManagementHistoryCreateUpdateModel model, RequestUser currentUser);
        #endregion

        #region TaskManagementMileStone
        Task<Response<TaskManagementMileStoneViewModel>> CreateTaskMileStone(TaskManagementMileStoneCreateUpdateModel model, RequestUser currentUser);
        Task<Response<TaskManagementMileStoneViewModel>> UpdateTaskMileStone(Guid id, TaskManagementMileStoneCreateUpdateModel model, RequestUser currentUser);
        Task<Response<Pagination<TaskManagementMileStoneViewModel>>> GetPageTaskMileStone(TaskManagementMileStoneQueryModel query, RequestUser currentUser);
        Task<Response<TaskManagementMileStoneViewModel>> GetByIdTaskMileStone(Guid id, RequestUser currentUser);
        Task<Response> DeleteTaskMileStone(Guid id, RequestUser currentUser);
        #endregion
    }
}
