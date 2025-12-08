using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Data.Entity.VatTu;

namespace NSPC.Data.Entity
{
    [Table("sm_InventoryCheckNoteItems")]
    public class sm_InventoryCheckNoteItems : BaseTableService<sm_InventoryCheckNoteItems>
    {
        [Key]
        public Guid Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        
        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;

        /// <summary>
        /// Tồn kho
        /// </summary>
        public decimal RecordedQuantity { get; set; } = 0M;

        /// <summary>
        /// Tồn kho thực tế
        /// </summary>
        public decimal ActualQuantity { get; set; } = 0M; // Tồn thực tế

        /// <summary>
        /// Chênh lệch (âm hoặc dương)
        /// </summary>
        public decimal DifferenceQuantity { get; set; } = 0M;
        
        /// <summary>
        /// Ghi chú cho vật tư tồn kho
        /// </summary>
        public string NoteInventory { get; set; }
        
        /// <summary>
        /// Lý do cho vật tư tồn kho
        /// </summary>
        public string ReasonInventory { get; set; }
        
        /// <summary>
        /// Loại chênh lệch (Lệch, Khớp)
        /// </summary>
        public string DifferenceType { get; set; }
        
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual sm_Product Sm_Product { get; set; }

        /// <summary>
        /// Id phiếu kiểm kho
        /// </summary>
        public Guid CheckInventoryNoteId { get; set; }
        [ForeignKey("CheckInventoryNoteId")]
        public virtual sm_InventoryCheckNote sm_InventoryCheckNote { get; set; }
    }
}
