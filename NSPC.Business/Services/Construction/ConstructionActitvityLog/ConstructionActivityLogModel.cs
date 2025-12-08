using System.Text.Json.Serialization;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.Contract;
using NSPC.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NSPC.Business.Services.InventoryNote;
using NSPC.Data;

namespace NSPC.Business.Services.ConstructionActitvityLog
{
    public class ConstructionActivityLogCreateModel
    {
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

        /// <summary>
        /// Id công trình
        /// </summary>
        public Guid? ConstructionId { get; set; }

        /// <summary>
        /// Thứ tự bước thực hiện
        /// </summary>
        public int StepOrder { get; set; } = 0;
        
        /// <summary>
        /// Loại hành động
        /// </summary>
        public string ActionType { get; set; }
    }

    public class ConstructionActivityLogViewModel
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

        /// <summary>
        /// Id công trình
        /// </summary>
        public Guid ConstructionId { get; set; }
        
        /// <summary>
        /// Loại hành động
        /// </summary>
        public string ActionType { get; set; }
        
        /// <summary>
        /// Thứ tự bước thực hiện
        /// </summary>
        public int StepOrder { get; set; }
        
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }
}


