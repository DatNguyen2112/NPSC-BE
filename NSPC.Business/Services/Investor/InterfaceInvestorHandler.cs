using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.Investor
{
    public interface InterfaceInvestorHandler
    {
        Task<Response<InvestorViewModel>> CreateAsync (InvestorCreateModel  model, RequestUser currentUser);
        Task<Response<InvestorViewModel>> UpdateAsync (Guid Id, InvestorCreateModel model, RequestUser currentUser);
        Task<Response<InvestorViewModel>> GetById (Guid Id, RequestUser currentUser);
        Task<Response<InvestorViewModel>> DeleteAsync (Guid Id, RequestUser currentUser);
        Task<Response<Pagination<InvestorViewModel>>> GetPageAsync (InvestorQueryModel query);
    }
}

