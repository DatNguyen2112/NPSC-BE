using NSPC.Common;
using static NSPC.Common.Helper;


namespace NSPC.Business.Services.InvestorType
{
    public interface InterfaceInvestorTypeHandler
    {
       Task<Response<InvestorTypeViewModel>> CreateAsync (InvestorTypeCreateModel  model, RequestUser currentUser);
       Task<Response<InvestorTypeViewModel>> UpdateAsync (Guid Id, InvestorTypeCreateModel  model, RequestUser currentUser);
       Task<Response<InvestorTypeViewModel>> GetById (Guid Id, RequestUser currentUser);
       Task<Response<InvestorTypeViewModel>> DeleteAsync (Guid Id, RequestUser currentUser);
       Task<Response<Pagination<InvestorTypeViewModel>>> GetPageAsync (InvestorTypeQueryModel query);
    }
}
