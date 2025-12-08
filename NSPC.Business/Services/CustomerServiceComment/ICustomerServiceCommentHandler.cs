using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface ICustomerServiceCommentHandler
    {
        Task<Response<CustomerServiceCommentViewModel>> Create (CustomerServiceCommentCreateModel model, RequestUser currentUser);
        
        Task<Response<CustomerServiceCommentViewModel>> Update (Guid Id, CustomerServiceCommentCreateModel model, RequestUser currentUser);
        
        Task<Response<CustomerServiceCommentViewModel>> Delete(Guid Id, RequestUser currentUser);
        
        Task<Response<Pagination<CustomerServiceCommentViewModel>>> GetPageAsync (CustomerServiceQueryModel query);
    }
}

