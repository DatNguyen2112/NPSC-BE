using NSPC.Common;

namespace NSPC.Business;

public interface IAssetHandler
{
    Task<Response<List<AssetViewModel>>> Create(AssetCreateUpdateModel model);
    Task<Response<AssetViewModel>> Update(Guid id, AssetCreateUpdateModel model);
    Task<Response<Pagination<AssetViewModel>>> GetPage(AssetQueryModel query);
    public Task<Response<Dictionary<string, int>>> CountByStatus(AssetQueryModel query);
    Task<Response<AssetDetailViewModel>> GetById(Guid id);
    Task<Response> Delete(Guid id);
    Task<Response<AssetViewModel>> ReportDamage(Guid assetId, AssetReportDamageModel model);
    Task<Response<AssetViewModel>> ReportLost(Guid assetId, AssetReportLostModel model);
    Task<Response<AssetViewModel>> ReportDestroyed(Guid assetId, AssetReportDestroyedModel model);
    Task<Response<AssetViewModel>> Allocate(Guid assetId, AssetAllocateModel model);
    Task<Response<AssetViewModel>> Revoke(Guid assetId, AssetAllocateModel model);
    Task<Response<AssetViewModel>> Transfer(Guid assetId, AssetAllocateModel model);
    Task<Response> AllocateEmail(Guid assetId, AssetApprovalModel assetApproval);
    Task<Response> RevokeEmail(Guid assetId, AssetApprovalModel assetApproval);
    Task<Response> TransferEmail(Guid assetId, AssetApprovalModel assetApproval);
}
