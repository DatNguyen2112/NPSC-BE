using NSPC.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.DebtTransaction
{
    public class DebtTransactionViewModel
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public string EntityCode { get; set; }
        public string EntityType { get; set; }
        public string EntityName { get; set; }
        public Guid? OriginalDocumentId { get; set; }
        public string OriginalDocumentCode { get; set; }
        public string OriginalDocumentType { get; set; }
        public decimal ChangeAmount { get; set; }
        public decimal DebtAmount { get; set; }
        public string Action { get; set; }
        public string Note { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }

    public class DebtTransactionCreateModel
    {
        public Guid EntityId { get; set; }
        public string EntityCode { get; set; } // customer or supplier code
        public string EntityType { get; set; } // customer or supplier
        public string EntityName { get; set; } // customer or supplier name
        public Guid? OriginalDocumentId { get; set; }
        public string OriginalDocumentCode { get; set; }
        public string OriginalDocumentType { get; set; } // receipt_voucher, payment_voucher, sales_order, purchase_order, customer_return, supplier_return
        public decimal ChangeAmount { get; set; } // Outstanding debt value change
        public decimal DebtAmount { get; set; } // Cumulative outstanding debt value up to the creation time
        public string Action { get; set; } // receipt_voucher_create, payment_voucher_create, inventory_export, inventory_import, receipt_voucher_cancel, payment_voucher_cancel, sales_order_cancel,
                                           // purchase_order_cancel, return_inventory_export, return_inventory_import, customer_debt_init, supplier_debt_init
        public string Note { get; set; }
    }

    public class DebtTransactionReportViewModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Id đối tác
        /// </summary>
        public Guid EntityId { get; set; }
        /// <summary>
        /// Mã đối tác
        /// </summary>
        public string EntityCode { get; set; }
        /// <summary>
        /// Loại đối tác (Khách hàng hoặc Nhà cung cấp)
        /// </summary>
        public string EntityType { get; set; }
        /// <summary>
        /// Tên đối tác
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// Nợ đầu kỳ
        /// </summary>
        public decimal OpeningDebt { get; set; } = 0M;
        /// <summary>
        /// Nợ tăng trong kỳ
        /// </summary>
        public decimal DebtIncrease { get; set; } = 0M;
        /// <summary>
        /// Nợ giảm trong kỳ
        /// </summary>
        public decimal DebtDecrease { get; set; } = 0M;
        /// <summary>
        /// Nợ còn trong kỳ
        /// </summary>
        public decimal DebtRemain { get; set; } = 0M;
        /// <summary>
        /// Nợ cuối kỳ
        /// </summary>
        public decimal ClosingDebt { get; set; } = 0M;
    }

    public class DebtTransactionQueryModel : PaginationRequest
    {
        public Guid? EntityId { get; set; }
        public string EntityType { get; set; }
        public DateTime?[] DateRange { get; set; }
        public decimal? MinClosingDebt { get; set; }
        public decimal? MaxClosingDebt { get; set; }
        public bool? Positive { get; set; }
        // Công nợ dương hoặc âm
        public int? DebtAmountType { get; set; } // Dương: 0, Âm: 1

    }
}
