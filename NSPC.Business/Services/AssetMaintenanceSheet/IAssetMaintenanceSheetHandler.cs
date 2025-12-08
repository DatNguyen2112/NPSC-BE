using NSPC.Common;
using NSPC.Data.Data.Entity.Asset;

namespace NSPC.Business;

public interface IAssetMaintenanceSheetHandler
{
    Task<Response<AssetMaintenanceSheetViewModel>> Create(AssetMaintenanceSheetCreateUpdateModel model);
    Task<Response<AssetMaintenanceSheetViewModel>> Update(Guid id, AssetMaintenanceSheetCreateUpdateModel model);

    Task<Response<AssetMaintenanceSheetViewModel>> CompleteMaintenance(Guid id,
        AssetMaintenanceSheetCompleteModel model);

    Task<Response<Pagination<AssetMaintenanceSheetViewModel>>> GetPage(AssetMaintenanceSheetQueryModel query);
    Task<Response<Dictionary<string, int>>> CountByStatus(AssetMaintenanceSheetQueryModel query);
    Task<Response<AssetMaintenanceSheetViewModel>> GetById(Guid id);
    Task<Response> Delete(Guid id);
}