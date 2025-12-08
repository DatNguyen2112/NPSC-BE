using NSPC.Data;
using NSPC.Common;

namespace NSPC.Business.Services.ConstructionWeekReport
{
    public class ConstructionWeekReportCreateModel
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title  { get; set; }
       
        /// <summary>
        /// Kế hoạch tuần trước
        /// </summary>
        public string LastWeekPlan { get; set; }
       
        /// <summary>
        /// Kết quả thực hiện
        /// </summary>
        public string ProcessResult { get; set; }
       
        /// <summary>
        /// Kế hoạch tuần sau
        /// </summary>
        public string NextWeekPlan  { get; set; }
        
        public Guid ConstructionId  { get; set; }
        
        public List<jsonb_Attachment> FileAttachments { get; set; }
        
        public string StatusCode  { get; set; }
        
        public DateTime? StartDate  { get; set; }
        
        public DateTime? EndDate   { get; set; }
    }
    
    public class ConstructionWeekReportViewModel
    {
        public Guid Id  { get; set; }
        
        /// <summary>
        /// Mã báo cáo
        /// </summary>
        public string Code { get; set; }
       
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title  { get; set; }
       
        /// <summary>
        /// Kế hoạch tuần trước
        /// </summary>
        public string LastWeekPlan { get; set; }
       
        /// <summary>
        /// Kết quả thực hiện
        /// </summary>
        public string ProcessResult { get; set; }
       
        /// <summary>
        /// Kế hoạch tuần sau
        /// </summary>
        public string NextWeekPlan  { get; set; }
       
        /// <summary>
        /// File đính kèm 
        /// </summary>
        public List<AttachmentViewModel> FileAttachments { get; set; }
        
        public string StatusCode  { get; set; }
        
        public string StatusName  { get; set; }
        
        public DateTime? StartDate  { get; set; }
        
        public DateTime? EndDate   { get; set; }
        
        public Guid ConstructionId   { get; set; }
        
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class ConstructionWeekReportQueryModel: PaginationRequest
    {
        public DateTime?[] DateRange  { get; set; }
        
        public Guid? ConstructionId  { get; set; }
    }
}
