using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.EInvoice
{
    [Table("sm_EInvoiceVatAnalytics")]
    public class sm_EInvoiceVatAnalytics : BaseTableService<sm_EInvoiceVatAnalytics>
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Tổng hợp
        /// </summary>
        public string Synthetic { get; set; }

        /// <summary>
        /// Thành tiền trước thuế GTGT
        /// </summary>
        public decimal BeforeVatAmount { get; set; } = 0M;

        /// <summary>
        /// Tiền thuế GTGT
        /// </summary>
        public decimal VatAmount { get; set; } = 0M;

        /// <summary>
        /// Cộng tiền thanh toán
        /// </summary>
        public decimal TotalPaymentAmount { get; set; } = 0M;

        /// <summary>
        /// Id invoice
        /// </summary>
        public Guid EInvoiceId { get; set; }
    }
}
