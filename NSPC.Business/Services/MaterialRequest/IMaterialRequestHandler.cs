using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface IMaterialRequestHandler
    {
        Task<Response<MaterialRequestViewModel>> Create(MaterialRequestCreateUpdateModel model, RequestUser currentUser);
        Task<Response<MaterialRequestViewModel>> Update (Guid id, MaterialRequestCreateUpdateModel model, RequestUser currentUser);
        Task<Response<MaterialRequestViewModel>> Delete(Guid id, RequestUser currentUser);
        Task<Response<MaterialRequestViewModel>> GetById(Guid id, RequestUser currentUser);
        Task<Response<Pagination<MaterialRequestViewModel>>> GetPage(MaterialRequestQueryModel query);
        // Duyệt yêu cầu vật tư
        Task<Response<MaterialRequestViewModel>> ApproveMaterialRequest(Guid id, RequestUser currentUser);
        // Gửi duyệt yêu cầu vật tư
        Task<Response<MaterialRequestViewModel>> RequestApproveMaterialRequest(Guid id, RequestUser currentUser);
        // Từ chối duyệt yêu cầu vật tư
        Task<Response<MaterialRequestViewModel>> RejectApproveMaterialRequest(Guid id, MaterialRejectReason model, RequestUser currentUser);
    }
}

