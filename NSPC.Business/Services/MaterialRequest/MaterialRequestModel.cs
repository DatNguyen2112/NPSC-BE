using NSPC.Data;
using NSPC.Common;

namespace NSPC.Business.Services
{
    public class MaterialRequestCreateUpdateModel
    {
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content  { get; set; }
        
        /// <summary>
        /// Hạn xử lý
        /// </summary>
        public DateTime DateProcess { get; set; }
        
        /// <summary>
        /// Độ ưu tiên
        /// </summary>
        public string Priority { get; set; }
        
        /// <summary>
        /// Tên độ ưu tiên
        /// </summary>
        public string PriorityName  { get; set; }
        
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        
        public List<MaterialRequestItemCreateUpdateModel> MaterialRequestItems { get; set; } = new List<MaterialRequestItemCreateUpdateModel>();
        
        /// <summary>
        /// Id công trình/dự án
        /// </summary>
        public Guid ConstructionId  { get; set; }
    }

    public class MaterialRequestViewModel
    {
        public Guid Id  { get; set; }
        
        /// <summary>
        /// Mã yêu cầu
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content  { get; set; }
        
        /// <summary>
        /// Hạn xử lý
        /// </summary>
        public DateTime DateProcess { get; set; }
        
        /// <summary>
        /// Độ ưu tiên
        /// </summary>
        public string Priority { get; set; }
        
        /// <summary>
        /// Tên độ ưu tiên
        /// </summary>
        public string PriorityName  { get; set; }
        
        /// <summary>
        /// Trạng thái
        /// </summary>
        public string StatusCode { get; set; }
        
        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public string StatusName  { get; set; }
        
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        
        public List<MaterialRequestItemViewModel> MaterialRequestItems { get; set; }
        
        /// <summary>
        /// Thông tin vật tư yêu cầu
        /// </summary>
        public List<jsonb_MaterialRequest> MaterialRequestItem { get; set; } = new List<jsonb_MaterialRequest>();
        
        /// <summary>
        /// Lịch sử xử lý
        /// </summary>
        public List<jsonb_HistoryProcess> ListHistoryProcess { get; set; } =  new List<jsonb_HistoryProcess>();
        
        /// <summary>
        /// Id công trình/dự án
        /// </summary>
        public Guid ConstructionId  { get; set; }
        public ConstructionViewModel Construction { get; set; }
        
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class MaterialRejectReason
    {
        public string Reason { get; set; }
    }

    public class MaterialRequestQueryModel : PaginationRequest
    {
        /// <value>Id công trình/dự án</value>
        public Guid? ConstructionId { get; set; }

        /// <value>Hạn xử lý</value>
        public DateTime? DueDate { get; set; }

        /// <value>Ngày tạo</value>
        public DateTime? CreatedOnDate { get; set; }

        /// <value>Mã trạng thái</value>
        public string StatusCode { get; set; }

        /// <value>Mã độ ưu tiên</value>
        public string PriorityLevelCode { get; set; }

        /// <value>Cần thanh toán</value>
        public decimal? TotalAmount { get; set; }
        
        public DateTime?[] DateRange { get; set; }
    }
}

