using NSPC.Common;

namespace NSPC.Business;

public interface IAssetTypeHandler
{
    Task<Response<AssetTypeViewModel>> Create(AssetTypeCreateUpdateModel model);
    Task<Response<AssetTypeViewModel>> Update(Guid id, AssetTypeCreateUpdateModel model);
    Task<Response<Pagination<AssetTypeViewModel>>> GetPage(AssetTypeQueryModel query);
    Task<Response<AssetTypeViewModel>> GetById(Guid id);
    Task<Response> Delete(Guid id);
}