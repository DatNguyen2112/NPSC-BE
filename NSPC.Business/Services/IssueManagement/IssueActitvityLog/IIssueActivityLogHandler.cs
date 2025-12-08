using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface IIssueActivityLogHandler
    {
        Task<Response<IssueActivityLogViewModel>> Create(IssueActivityLogCreateModel model,
            RequestUser currentUser);
        Task<Response<IssueActivityLogViewModel>> GetById(Guid id, RequestUser currentUser);
    }
}
