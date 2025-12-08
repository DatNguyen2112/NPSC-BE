using NSPC.Data.Data.Entity.Quotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.DebtTransaction
{
    [Table("sm_DebtTransaction")]
    public class sm_DebtTransaction : BaseTableService<sm_DebtTransaction>
    {
        [Key]
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        [StringLength(30)]
        public string EntityCode { get; set; } // customer or supplier code
        [StringLength(30)]
        public string EntityType { get; set; } // customer or supplier
        [StringLength(256)]
        public string EntityName { get; set; } // customer or supplier name
        public Guid? OriginalDocumentId { get; set; }
        public string OriginalDocumentCode { get; set; }
        public string OriginalDocumentType { get; set; } // receipt_voucher, payment_voucher, sales_order, purchase_order, customer_return, supplier_return
        public decimal ChangeAmount { get; set; } // Outstanding debt value change
        public decimal DebtAmount { get; set; } // Cumulative outstanding debt value up to the creation time
        [StringLength(30)]
        public string Action { get; set; } // receipt_voucher_create, payment_voucher_create, inventory_export, inventory_import, receipt_voucher_cancel, payment_voucher_cancel, sales_order_cancel,
                                           // purchase_order_cancel, return_inventory_export, return_inventory_import, customer_debt_init, supplier_debt_init
        [StringLength(256)]
        public string Note { get; set; }
    }
}
