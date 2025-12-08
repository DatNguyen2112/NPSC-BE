using NSPC.Business.Services.InventoryNote;
using NSPC.Common;

namespace NSPC.Business.Services
{
    public class InventoryCheckNoteCreateUpdateModel
    {
        /// <summary>
        /// Mã kho
        /// </summary>
        public string WareCode { get; set; }
        
        /// <summary>
        /// Mã đơn, sinh tự động
        /// </summary>
        public string OrderCode { get; set; }
        
        public string Note { get; set; }
        public string Tag { get; set; }
        
        /// <summary>
        /// Ngày kiểm 
        /// </summary>
        public DateTime? CheckDate { get; set; }
        
        /// <summary>
        /// Danh sách sản phẩm cần kiểm
        /// </summary>
        public List<InventoryCheckNoteItemCreateUpdateModel> Items { get; set; }
    }

    public class InventoryCheckNoteViewModel
    {
        
        public Guid Id { get; set; }
        
        /// <summary>
        /// Kho
        /// </summary>
        public CodeTypeListModel Ware{ get; set; }
        
        /// <summary>
        /// Tên kho
        /// </summary>
        public string WareName { get; set; }
        
        /// <summary>
        /// Mã đơn, sinh tự động
        /// </summary>
        public string OrderCode { get; set; }
        
        public string Note { get; set; }
        public string Tag { get; set; }
        
        /// <summary>
        /// Trạng thái kiểm (Đã kiểm / Đang kiểm kho)
        /// </summary>
        public string StatusCode { get; set; }
        
        /// <summary>
        /// Ngày kiểm 
        /// </summary> 
        public DateTime? CheckDate { get; set; }
        
        /// <summary>
        /// Danh sách sản phẩm cần kiểm
        /// </summary>
        public List<InventoryCheckNoteItemViewModel> Items { get; set; }
        
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
        public string BalancedByUserName { get; set; }
        public string[] AllowedActions { get; set; }
    }

    public class InventoryCheckNoteQuery : PaginationRequest
    {
        public string StatusCode { get; set; }
        public DateTime?[] DateRange { get; set; }
        public string OrderCode { get; set; }
    }
}