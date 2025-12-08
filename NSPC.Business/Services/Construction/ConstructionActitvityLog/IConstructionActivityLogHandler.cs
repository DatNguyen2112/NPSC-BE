using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.ConstructionActitvityLog
{
    public interface IConstructionActivityLogHandler
    {
        Task<Response<ConstructionActivityLogViewModel>> Create(ConstructionActivityLogCreateModel model,
            RequestUser currentUser);
    }
}
