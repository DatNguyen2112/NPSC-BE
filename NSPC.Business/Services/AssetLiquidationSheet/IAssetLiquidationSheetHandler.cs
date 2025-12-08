using NSPC.Common;

namespace NSPC.Business.AssetLiquidationSheet;

public interface IAssetLiquidationSheetHandler
{
    Task<Response<AssetLiquidationSheetViewModel>> Create(AssetLiquidationSheetCreateUpdateModel model);
    Task<Response<AssetLiquidationSheetViewModel>> Update(Guid id, AssetLiquidationSheetCreateUpdateModel model);
    Task<Response<Pagination<AssetLiquidationSheetViewModel>>> GetPage(AssetLiquidationSheetQueryModel query);
    public Task<Response<Dictionary<string, int>>> CountByStatus(AssetLiquidationSheetQueryModel query);
    Task<Response<AssetLiquidationSheetViewModel>> GetById(Guid id);
    Task<Response> Delete(Guid id);
}