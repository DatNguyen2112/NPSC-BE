using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Data.Entity.VatTu;

namespace NSPC.Data.Entity
{
    [Table("sm_WarehouseTransferNoteItem")]
    public class sm_WarehouseTransferNoteItem: BaseTableService<sm_WarehouseTransferNoteItem>
    {
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public string ProductCode { get; set; }
        
        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string ProductName { get; set; }
        
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; }
        
        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;
        
        /// <summary>
        /// Số lượng điều chuyển
        /// </summary>
        public decimal Quantity { get; set; } = 0;
        
        /// <summary>
        /// Ghi chú dòng
        /// </summary>
        public string LineNote { get; set; }
        
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual sm_Product Sm_Product { get; set; }
        
        /// <summary>
        /// Id phiếu chuyển kho
        /// </summary>
        public Guid TransferNoteID { get; set; }
        [ForeignKey("TransferNoteID")]
        public virtual sm_WarehouseTransferNote sm_WarehouseTransferNote { get; set; }
    }
}

