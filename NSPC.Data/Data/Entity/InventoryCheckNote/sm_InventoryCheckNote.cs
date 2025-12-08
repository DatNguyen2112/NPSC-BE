using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    [Table("sm_InventoryCheckNote")]
    public class sm_InventoryCheckNote : BaseTableService<sm_InventoryCheckNote>
    {
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Mã kho
        /// </summary>
        [StringLength(64)]
        public string WareCode { get; set; }
        
        /// <summary>
        /// Tên kho
        /// </summary>
        public string WareName { get; set; }
        
        /// <summary>
        /// Mã đơn, sinh tự động
        /// </summary>
        [StringLength(64)]
        public string OrderCode { get; set; }
        
        public string Note { get; set; }
        public string Tag { get; set; }
        
        /// <summary>
        /// Trạng thái phiếu kiểm (Nháp / Đã hủy / Hoàn thành)
        /// </summary>
        public string StatusCode { get; set; }
        
        /// <summary>
        /// Ngày kiểm 
        /// </summary>
        public DateTime? CheckDate { get; set; }
        
        /// <summary>
        /// Người cân bằng
        /// </summary>
        public string BalancedByUserName { get; set; }
        
        /// <summary>
        /// Danh sách sản phẩm được kiểm
        /// </summary>
        public virtual ICollection<sm_InventoryCheckNoteItems> Items { get; set; }
    }
}

