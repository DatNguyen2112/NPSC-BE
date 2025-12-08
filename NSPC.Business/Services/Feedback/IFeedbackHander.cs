using NSPC.Business.Services.CashbookTransaction;
using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.Feedback
{
    public interface IFeedbackHandler
    {
        Task<Response<FeedbackViewModel>> Create(FeedbackCreateModel model, RequestUser currentUser);
    }
}
