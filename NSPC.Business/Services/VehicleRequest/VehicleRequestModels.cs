using NSPC.Common;
using System.Collections.Generic;
using NSPC.Business.Services.QuanLyPhuongTien;

namespace NSPC.Business.Services.VehicleRequest
{
    /// <summary>
    /// View model for vehicle request
    /// </summary>
    public class VehicleRequestViewModel
    {
        public Guid Id { get; set; }
        public string RequestCode { get; set; }
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ContactPhone { get; set; }
        public Guid? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Purpose { get; set; }
        public string Priority { get; set; }
        public int NumPassengers { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string DepartureLocation { get; set; }
        public string DestinationLocation { get; set; }
        public Guid? RequestedVehicleId { get; set; }
        public PhuongTienViewModel RequestedVehicle { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public string RejectNotes { get; set; }
        public Guid? SharingGroupId { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public string LastModifiedByUserName { get; set; }
    }


    /// <summary>
    /// Create/Update model for vehicle request
    /// </summary>
    public class VehicleRequestCreateUpdateModel
    {
        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set; }
        public string ContactPhone { get; set; }
        public Guid? ProjectId { get; set; }
        public string Purpose { get; set; }
        public string Priority { get; set; }
        public int NumPassengers { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string DepartureLocation { get; set; }
        public string DestinationLocation { get; set; }
        public Guid? RequestedVehicleId { get; set; }
        public string Notes { get; set; }
    }

    /// <summary>
    /// Query model for vehicle request
    /// </summary>
    public class VehicleRequestQueryModel : PaginationRequest
    {
        /// <summary>
        /// Mã yêu cầu
        /// </summary>
        public string RequestCode { get; set; }

        /// <summary>
        /// ID người tạo
        /// </summary>
        public Guid? CreatedByUserId { get; set; }

        /// <summary>
        /// Độ ưu tiên
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// Khoảng thời gian tạo [FromDate, ToDate]
        /// </summary>
        public DateTime?[] CreatedDateRange { get; set; }

        /// <summary>
        /// Khoảng thời gian sử dụng [FromDate, ToDate]
        /// </summary>
        public DateTime?[] UsageDateRange { get; set; }

        /// <summary>
        /// ID đơn vị sử dụng xe
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// ID người sử dụng xe
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// ID loại xe
        /// </summary>
        public Guid? VehicleTypeId { get; set; }

        /// <summary>
        /// ID xe
        /// </summary>
        public Guid? VehicleId { get; set; }

        /// <summary>
        /// Trạng thái yêu cầu
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// ID dự án
        /// </summary>
        public Guid? ProjectId { get; set; }

        public bool IncludeVehicle { get; set; } = false;
    }

    /// <summary>
    /// Model for approving or rejecting a vehicle request
    /// </summary>
    public class VehicleRequestApprovalModel
    {
        public bool IsApproved { get; set; }
        public string RejectNotes { get; set; }
    }

    /// <summary>
    /// Detailed view model for vehicle request that includes activity history
    /// </summary>
    public class VehicleRequestDetailViewModel : VehicleRequestViewModel
    {
        /// <summary>
        /// List of activity history records for this vehicle request, sorted by creation date (newest first)
        /// </summary>
        public List<ActivityHistoryViewModel> ActivityHistories { get; set; } = new List<ActivityHistoryViewModel>();

        /// <summary>
        /// List of other vehicle requests in the same sharing group (if any)
        /// </summary>
        public List<VehicleRequestViewModel> SharingGroupRequests { get; set; } = new List<VehicleRequestViewModel>();
    }

    /// <summary>
    /// Configuration model for vehicle request export
    /// </summary>
    public class VehicleRequestExportConfig
    {
        /// <summary>
        /// Tên công ty
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Phụ trách điều xe
        /// </summary>
        public string VehicleCoordinator { get; set; }

        /// <summary>
        /// ID phụ trách điều xe
        /// </summary>
        public Guid? VehicleCoordinatorId { get; set; }
    }
}