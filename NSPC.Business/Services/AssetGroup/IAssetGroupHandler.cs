using NSPC.Common;

namespace NSPC.Business;

public interface IAssetGroupHandler
{
    Task<Response<AssetGroupViewModel>> Create(AssetGroupCreateUpdateModel model);
    Task<Response<AssetGroupViewModel>> Update(Guid id, AssetGroupCreateUpdateModel model);
    Task<Response<Pagination<AssetGroupViewModel>>> GetPage(AssetGroupQueryModel query);
    Task<Response<AssetGroupViewModel>> GetById(Guid id);
    Task<Response> Delete(Guid id);
}