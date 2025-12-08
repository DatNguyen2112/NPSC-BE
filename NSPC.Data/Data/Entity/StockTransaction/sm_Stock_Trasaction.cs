using NSPC.Data.Data.Entity.KiemKho;
using NSPC.Data.Data.Entity.VatTu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.StockTransaction
{
    [Table("sm_Stock_Transaction")]
    public class sm_Stock_Transaction : BaseTableService<sm_Stock_Transaction>
    {
        [Key]
        public Guid Id { get; set; }
        public string TypeCode { get; set; } // Mã phiếu
        public string TypeName { get; set; } // Tên phiếu
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Action { get; set; }
        public Guid? OriginalDocumentId { get; set; }
        public string OriginalDocumentCode { get; set; }
        public string OriginalDocumentType { get; set; }
        public string Unit { get; set; }
        public string WareCode { get; set; }
        public int? Quantity { get; set; }
        
        /// <summary>
        /// Số lượng có thể bán trong kho
        /// </summary>
        public decimal SellableQuantity { get; set; } = 0M;
        
        public decimal? StockTransactionQuantity { get; set; } //Số lượng tồn kho
        public decimal? OpeningInventoryQuantity { get; set; } // Số lượng tồn đầu kỳ
        public decimal? ClosingInventoryQuantity { get; set; } //Số lượng tồn cuối kỳ
        public decimal? ReceiptInventoryQuantity { get; set; } //Số lượng nhập kho
        public decimal? OpeningInventoryAmount { get; set; } // Giá trị tồn đầu kỳ
        public decimal? ClosingInventoryAmount { get; set; } //Giá trị tồn cuối kỳ
        public decimal? ReceiptInventoryAmount { get; set; } //Giá trị nhập kho
        public decimal? ExportInventoryQuantity { get; set; } //Số lượng xuất bán
        public decimal? ExportInventoryAmount { get; set; } //Giá trị xuất bán
        public decimal? InventoryIncreased { get; set; } //Số lượng kiểm kê tăng
        public decimal? InventoryDecreased { get; set; } //Số lượng kiểm kê giảm
        public decimal? InitialStockQuantity { get; set; } //Tồn tổng của sản phẩm
        public Guid? CheckInventoryId { get; set; } // Kiểm kho
        [ForeignKey("CheckInventoryId")]
        public virtual mk_KiemKho mk_KiemKho { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual sm_Product Sm_Product { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual idm_User CreatedByUser { get; set; }
    }

}
