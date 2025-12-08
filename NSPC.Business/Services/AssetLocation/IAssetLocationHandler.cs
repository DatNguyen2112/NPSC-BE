using NSPC.Common;

namespace NSPC.Business;

public interface IAssetLocationHandler
{
    Task<Response<AssetLocationViewModel>> Create(AssetLocationCreateUpdateModel model);
    Task<Response<AssetLocationViewModel>> Update(Guid id, AssetLocationCreateUpdateModel model);
    Task<Response<Pagination<AssetLocationViewModel>>> GetPage(AssetLocationQueryModel query);
    Task<Response<AssetLocationViewModel>> GetById(Guid id);
    Task<Response> Delete(Guid id);
}
