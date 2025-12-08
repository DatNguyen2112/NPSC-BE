using NSPC.Common;
using System;
using System.Threading.Tasks;

namespace NSPC.Business.Services.VehicleRequest
{
    /// <summary>
    /// Interface for vehicle request handler
    /// </summary>
    public interface IVehicleRequestHandler
    {
        /// <summary>
        /// Create a new vehicle request
        /// </summary>
        /// <param name="model">Vehicle request data</param>
        /// <returns>Created vehicle request</returns>
        Task<Response<VehicleRequestViewModel>> Create(VehicleRequestCreateUpdateModel model);

        /// <summary>
        /// Update an existing vehicle request
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <param name="model">Updated vehicle request data</param>
        /// <returns>Updated vehicle request</returns>
        Task<Response<VehicleRequestViewModel>> Update(Guid id, VehicleRequestCreateUpdateModel model);

        /// <summary>
        /// Get a vehicle request by ID with activity history
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <returns>Vehicle request details with activity history</returns>
        Task<Response<VehicleRequestDetailViewModel>> GetById(Guid id);

        /// <summary>
        /// Get a paginated list of vehicle requests
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <returns>Paginated list of vehicle requests</returns>
        Task<Response<Pagination<VehicleRequestViewModel>>> GetPage(VehicleRequestQueryModel query);

        /// <summary>
        /// Delete a vehicle request
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <returns>Response indicating success or failure</returns>
        Task<Response> Delete(Guid id);

        /// <summary>
        /// Approve or reject a vehicle request
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <param name="model">Approval data</param>
        /// <returns>Updated vehicle request</returns>
        Task<Response<List<VehicleRequestViewModel>>> ProcessApproval(Guid id, VehicleRequestApprovalModel model);

        /// <summary>
        /// Submit a vehicle request for approval
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <returns>List of conflicting vehicle requests (empty if no conflicts or other errors)</returns>
        Task<Response<List<VehicleRequestViewModel>>> SubmitForApproval(Guid id);

        /// <summary>
        /// Export vehicle request to PDF
        /// </summary>
        /// <param name="id">Vehicle request ID</param>
        /// <returns>Updated vehicle request</returns>
        Task<(Response, Stream, string)> GetRequestPdf(Guid id);

        /// <summary>
        /// Get vehicle request export configuration
        /// </summary>
        /// <returns>Export configuration</returns>
        Task<Response<VehicleRequestExportConfig>> GetExportConfig();

        /// <summary>
        /// Update vehicle request export configuration
        /// </summary>
        /// <param name="config">Export configuration data</param>
        /// <returns>Updated export configuration</returns>
        Task<Response<VehicleRequestExportConfig>> UpdateExportConfig(VehicleRequestExportConfig config);

        /// <summary>
        /// Submit vehicle sharing request
        /// </summary>
        /// <param name="draftRequestId">ID of the draft request to be shared</param>
        /// <param name="approvedRequestIds">List of IDs of approved/shared requests to share with</param>
        /// <returns>Updated vehicle request</returns>
        Task<Response<VehicleRequestViewModel>>
            SubmitVehicleSharing(Guid draftRequestId, List<Guid> approvedRequestIds);

        /// <summary>
        /// Approve vehicle sharing request
        /// </summary>
        /// <param name="id">ID of the vehicle request to approve sharing</param>
        /// <returns>Updated vehicle request</returns>
        Task<Response<VehicleRequestViewModel>> ApproveVehicleSharing(Guid id, List<Guid> approvedRequestIds);
    }
}