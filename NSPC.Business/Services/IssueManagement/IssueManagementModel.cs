using NPSC.Data;
using NSPC.Business.Services;
using NSPC.Business.Services.ChucVu;
using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using NSPC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public class IssueManagementViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public UserModel User { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string PriorityLevel { get; set; }
        public string Content { get; set; }
        public ConstructionViewModel Construction { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }

    }

    public class ResolveModel
    {

        public string ContentResolve { get; set; }
        public List<AttachmentViewModel> AttachmentsResolve { get; set; }

    }


    public class IssueManagementCreateUpdateModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string PositionCode { get; set; }
        public Guid? UserId { get; set; }
        public string Status { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public string Description { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string PriorityLevel { get; set; }
        public string Content { get; set; }
        public Guid ConstructionId { get; set; }
    }
    
    public class IssueManagementQuery: PaginationRequest
    {
        public string Code { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime?[] DateRange { get; set; }
        public DateTime?[] ExpiryDate { get; set; }
        public Guid? UserId { get; set; }
        public string Status { get; set; }
        public string PriorityLevel { get; set; }
        public Guid? ConstructionId { get; set; }
    }

    public class IssueCountByStatus
    {
        public int TotalIssue { get; set; }
        public int CancelIssue { get; set; }
        public int ResolveIssue { get; set; }
        public int WaitResolveIssue { get; set; }
     
    }
}
