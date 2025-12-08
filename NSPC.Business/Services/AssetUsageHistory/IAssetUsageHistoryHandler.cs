using NSPC.Common;

namespace NSPC.Business;

public interface IAssetUsageHistoryHandler
{
    Task<Response<Pagination<AssetUsageHistoryViewModel>>> GetPage(AssetUsageHistoryQueryModel query);
    Task<Response<AssetUsageHistoryViewModel>> GetById(Guid id);
}