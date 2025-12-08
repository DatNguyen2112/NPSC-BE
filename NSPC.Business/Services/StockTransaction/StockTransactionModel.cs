using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.StockTransaction
{
    public class StockTransactionCreateModel
    {
        public Guid? Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string OriginalDocumentCode { get; set; }
        public string OriginalDocumentType { get; set; }
        public string WareCode { get; set; }
        public string Unit { get; set; }
        public decimal? StockTransactionQuantity { get; set; } // Tồn kho của sản phẩm
        public decimal? UnitPrice { get; set; }
        public decimal? OpeningInventoryQuantity { get; set; } // Số lượng tồn đầu kỳ
        public decimal? ClosingInventoryQuantity { get; set; } //Số lượng tồn cuối kỳ
        public decimal? ReceiptInventoryQuantity { get; set; } //Số lượng nhập kho
        public decimal? InventoryIncreased { get; set; } //Số lượng kiểm kê tăng
        public decimal? ExportInventoryQuantity { get; set; } //Số lượng xuất bán
        public decimal? InventoryDecreased { get; set; } //Số lượng kiểm kê giảm
        public decimal? TotalRecieptInventory{ get; set; } //Tổng giá trị nhập
        public decimal? TotalExportInventory { get; set; } //Tổng giá trị xuất
        public decimal? InitialStockQuantity { get; set; } //Số lượng tồn kho ban đầu của sản phẩm (dùng cho chức năng Lịch sử kho)
        
        /// <summary>
        /// Số lượng có thể bán trong kho (dùng để check khi tạo đơn bán hàng)
        /// </summary>
        public decimal SellableQuantity { get; set; }
        
        public Guid? OriginalDocumentId { get; set; } // Id Chứng từ gốc
        public Guid CreatedByUserId { get; set; }
        public Guid ProductId { get; set; }
        public string? Action {  get; set; }
    }

    public class StockTransactionViewModel
    {
        public Guid ProductId { get; set; }
        public string WareCode { get; set; }
        public string WareName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public string Action {  get; set; }
        public string OriginalDocumentCode { get; set; }
        public string OriginalDocumentName { get; set; }
        public string OriginalDocumentUrl { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? OpeningInventoryQuantity { get; set; } // Số lượng tồn đầu kỳ
        public decimal? OpeningInventoryAmount { get; set; } // Giá trị tồn đầu kỳ
        public decimal? ClosingInventoryQuantity
        {
            get { return OpeningInventoryQuantity + TotalImportQuantity - TotalExportQuantity; } 
            set {} 
        } //Số lượng tồn cuối kỳ
        public decimal? ClosingInventoryAmount
        {
            get { return OpeningInventoryAmount + TotalImportAmount - TotalExportAmount; } 
            set {} 
        } //Số lượng tồn cuối kỳ
        public decimal? TotalOpeningInventory { get; set; } // Tổng giá trị tồn đầu kỳ
        public decimal? TotalClosingInventory { get; set; } // Tổng giá trị tồn cuối kỳ
        public decimal? InventoryIncreased { get; set; } //Số lượng kiểm kê tăng
        public decimal? ExportInventoryQuantity { get; set; } //Số lượng xuất bán
        public decimal? StockTransactionQuantity // Số lượng tồn kho
        {
            get
            {
                return ReceiptInventoryQuantity - ExportInventoryQuantity;
            }
            set { }
        }
        public decimal? StockTransactionAmount // Giá trị tồn kho
        {
            get
            {
                return ReceiptInventoryAmount - ExportInventoryAmount;
            }
            set { }
        }
        // public decimal? TotalClosingImportAmount // Gía trị nhập trong kỳ
        // {
        //     get
        //     {
        //         return OpeningInventoryAmount + TotalImportAmount;
        //     }
        //     set { }
        // }
        // public decimal? TotalClosingImportQuantity // so luong  nhập trong kỳ
        // {
        //     get
        //     {
        //         return OpeningInventoryQuantity + TotalImportQuantity;
        //     }
        //     set { }
        // }
        public decimal? ExportInventoryAmount { get; set; } // Giá trị xuất bán trong kỳ
        public decimal? InventoryDecreased { get; set; } //Số lượng kiểm kê giảm
        public decimal? TotalImportAmount { get; set; } //Tổng giá trị nhập trong kỳ
        public decimal? TotalImportQuantity { get; set; } // Tổng số lượng nhập trong kỳ
        public decimal? ReceiptInventoryAmount { get; set; } // Giá trị nhập trong kỳ
        public decimal? ReceiptInventoryQuantity { get; set; } // Số lượng nhập trong kỳ
        public decimal? TotalExportAmount { get; set; } //Tổng giá trị xuất trong kỳ
        public decimal? TotalExportQuantity { get; set; } //Tổng số lượng xuất trong kỳ
        public decimal? InitialStockQuantity { get; set; } //Số lượng tồn kho ban đầu của sản phẩm (dùng cho chức năng Lịch sử kho)

        /// <summary>
        /// Số lượng có thể bán trong kho (dùng để check khi tạo đơn bán hàng)
        /// </summary>
        public decimal SellableQuantity { get; set; } = 0M;

        // public decimal? EndingInventoryReceivedQuantity {
        //     get {
        //         return OpeningInventoryQuantity + ReceiptInventoryQuantity - ExportInventoryQuantity;
        //     }
        //     set
        //     {
        //
        //     } 
        // } // Số lượng nhập của tồn cuối kỳ
        // public decimal? EndingInventoryReceivedAmount
        // {
        //     get {
        //         return OpeningInventoryAmount + ReceiptInventoryAmount - ExportInventoryAmount;
        //     }
        //
        //     set { }
        // } // Giá trị nhập của tồn cuối kỳ
        // public decimal? EndingInventoryIssuedQuantity
        // {
        //     get
        //     {
        //         return OpeningInventoryQuantity + ExportInventoryQuantity;
        //     }
        //     set
        //     {
        //
        //     }
        // } // Số lượng xuất của tồn cuối kỳ
        // public decimal? EndingInventoryIssuedAmount
        // {
        //     get
        //     {
        //         return OpeningInventoryAmount + ExportInventoryAmount;
        //     }
        //     set
        //     {
        //
        //     }
        // } // Giá trị xuất của tồn cuối kỳ
    }

    public class StockHistoryViewModel
    { 
        public Guid ProductId { get; set; }
        public string WareCode { get; set; }
        public string WareName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public string Action { get; set; }
        public string OriginalDocumentCode { get; set; }
        public string OriginalDocumentName { get; set; }
        public string OriginalDocumentUrl { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? OpeningInventoryQuantity { get; set; } // Số lượng tồn đầu kỳ
        public decimal? OpeningInventoryAmount { get; set; } // Giá trị tồn đầu kỳ
        public decimal? ClosingInventoryQuantity { get; set; } //Số lượng tồn cuối kỳ
        public decimal? ClosingInventoryAmount { get; set; } //Số lượng tồn cuối kỳ
        public decimal? TotalOpeningInventory { get; set; } // Tổng giá trị tồn đầu kỳ
        public decimal? TotalClosingInventory { get; set; } // Tổng giá trị tồn cuối kỳ
        public decimal? ImportQuantity { get; set; } //Số lượng nhập kho
        public decimal? InventoryIncreased { get; set; } //Số lượng kiểm kê tăng
        public decimal? ExportInventoryQuantity { get; set; } //Số lượng xuất bán
        public decimal? StockTransactionQuantity // Số lượng tồn kho
        {
            get;

            set;
        }
        public decimal? StockTransactionAmount // Giá trị tồn kho
        {
            get;

            set;
        }
        public decimal? TotalClosingImportAmount
        {
            get;

            set;
        }
        public decimal? TotalClosingImportQuantity
        {
            get;

            set;
        }
        public decimal? ExportInventoryAmount { get; set; } // Giá trị xuất bán trong kỳ
        public decimal? InventoryDecreased { get; set; } //Số lượng kiểm kê giảm
        public decimal? TotalImportAmount { get; set; } //Tổng giá trị nhập trong kỳ
        public decimal? ReceiptInventoryAmount { get; set; } // Giá trị nhập trong kỳ
        public decimal? ReceiptInventoryQuantity { get; set; } // Số lượng nhập trong kỳ
        public decimal? TotalExportAmount { get; set; } //Tổng giá trị xuất trong kỳ
        public decimal? TotalExportQuantity { get; set; } //Tổng số lượng xuất trong kỳ
        public decimal? EndingInventoryReceivedQuantity
        {
            get;

            set;
        } // Số lượng nhập của tồn cuối kỳ
        public decimal? EndingInventoryReceivedAmount
        {
            get;

            set;
        } // Giá trị nhập của tồn cuối kỳ
        public decimal? EndingInventoryIssuedQuantity
        {
            get;

            set;
        } // Số lượng xuất của tồn cuối kỳ
        public decimal? EndingInventoryIssuedAmount
        {
            get;

            set;
        } // Giá trị xuất của tồn cuối kỳ
        public decimal? InitialStockQuantity { get; set; } //Số lượng tồn kho ban đầu của sản phẩm (dùng cho chức năng Lịch sử kho)
        public Guid? OriginalDocumentId { get; set; } // Id Chứng từ gốc
        public DateTime? CreatedOnDate { get; set; }

        public CodeTypeListModel Ware { get; set; }
    }

    public class StockTransactionQueryModel : PaginationRequest
    {
        public List<string>? WaresCode { get; set; }
        public List<Guid>? ProductIds { get; set; }
        public string Type { get; set; }
        public DateTime?[] DateRange { get; set; }

    }
}
