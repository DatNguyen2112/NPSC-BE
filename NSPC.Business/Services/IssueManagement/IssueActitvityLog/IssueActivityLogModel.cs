using System.Text.Json.Serialization;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.Contract;
using NSPC.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NSPC.Business.Services.InventoryNote;
using NSPC.Data;

namespace NSPC.Business.Services
{
    public class IssueActivityLogCreateModel
    {
        /// <summary>
        /// Mô tả action
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Mã phiếu có trong công trình dự án (gán link trên FE)
        /// </summary>
        public string CodeLinkDescription { get; set; }
        public List<AttachmentViewModel> AttachmentsResolve { get; set; }
        public string ContentResovle { get; set; }
        public string ReasonCancel { get; set; }
        public string ReasonReopen { get; set; }

        /// <summary>
        /// Id phiếu có trong công trình dự án
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Id công trình
        /// </summary>
        public Guid? ConstructionId { get; set; }
    }

    public class IssueActivityLogViewModel
    {
        public Guid  Id { get; set; }
        /// <summary>
        /// Nguời thao tác
        /// </summary>
        public string UserName { get; set; }
        
        public string AvatarUrl { get; set; }

        /// <summary>
        /// Mô tả action
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Mã phiếu có trong công trình dự án (gán link trên FE)
        /// </summary>
        public string CodeLinkDescription { get; set; }

        /// <summary>
        /// Id phiếu có trong công trình dự án
        /// </summary>
        public Guid OrderId { get; set; }
        public string ContentResovle { get; set; }
        public string ReasonCancel { get; set; }
        public string ReasonReopen { get; set; }
        public List<AttachmentViewModel> AttachmentsResolve { get; set; }

        /// <summary>
        /// Id công trình
        /// </summary>
        public Guid ConstructionId { get; set; }
        
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class IssueActivityLogQuery: PaginationRequest
    {

    }
}


