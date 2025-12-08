using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface IProductInventoryHandler
    {
        Task<Response<ProductInventoryViewModel>> Create(ProductInventoryCreateModel model, RequestUser currentUser);
    }
}

