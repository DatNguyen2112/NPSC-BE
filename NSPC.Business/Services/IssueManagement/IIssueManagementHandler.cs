using NSPC.Business.Services.CashbookTransaction;
using NSPC.Common;
using static NSPC.Common.Helper;


namespace NSPC.Business.Services
{
    public interface IIssueManagementHandler
    {
        Task<Response<IssueManagementViewModel>> Create(IssueManagementCreateUpdateModel model, RequestUser currentUser);
        Task<Response<IssueManagementViewModel>> GetById(Guid id, RequestUser currentUser);
        Task<Response<Pagination<IssueActivityLogViewModel>>> GetActivityLogById(Guid id, RequestUser currentUser);
        Task<Response<IssueManagementViewModel>> Update(Guid id, IssueManagementCreateUpdateModel model, RequestUser currentUser);
        Task<Response<Pagination<IssueManagementViewModel>>> GetPage(IssueManagementQuery query);
        Task<Response> DeactiveIssueAsync(Guid id, string reasonCancel);
        Task<Response<IssueManagementViewModel>> Delete(Guid id, RequestUser currentUser);
        Task<Response> ReopenIssueAsync(Guid id, string reasonOpen);
        Task<Response> ResolveIssueAsync(Guid id, ResolveModel model);
        Task<Response<IssueCountByStatus>> CountByStatus(RequestUser currentUser);
    }
}

