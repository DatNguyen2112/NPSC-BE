using NSPC.Business.Services.AssetAllocation;
using NSPC.Common;

namespace NSPC.Business;

public interface IAssetAllocationHandler
{
    Task<Response<Pagination<AssetAllocationViewModel>>> GetPage(AssetAllocationQueryModel query);
    Task<Response<AssetAllocationViewModel>> GetById(Guid id);
}
