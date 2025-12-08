namespace NSPC.Business.Services
{
    public class InventoryCheckNoteItemCreateUpdateModel
    {
        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;

        /// <summary>
        /// Tồn kho thực tế
        /// </summary>
        public decimal ActualQuantity { get; set; } = 0M; // Tồn thực tế
        
        /// <summary>
        /// Ghi chú cho vật tư tồn kho
        /// </summary>
        public string NoteInventory { get; set; }
        
        /// <summary>
        /// Lý do cho vật tư tồn kho
        /// </summary>
        public string ReasonInventory { get; set; }
        
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
    }

    public class InventoryCheckNoteItemViewModel
    {
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
        public decimal RecordedQuantity { get; set; }

        /// <summary>
        /// Tồn kho thực tế
        /// </summary>
        public decimal ActualQuantity { get; set; } // Tồn thực tế

        /// <summary>
        /// Chênh lệch (âm hoặc dương)
        /// </summary>
        public decimal DifferenceQuantity { get; set; }
        
        /// <summary>
        /// Ghi chú cho vật tư tồn kho
        /// </summary>
        public string NoteInventory { get; set; }
        
        /// <summary>
        /// Lý do cho vật tư tồn kho
        /// </summary>
        public string ReasonInventory { get; set; }
        
        public CodeTypeListModel Reason { get; set; }
        
        /// <summary>
        /// Loại chênh lệch (Lệch, Khớp)
        /// </summary>
        public string DifferenceType { get; set; }
        
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
    }
} 
