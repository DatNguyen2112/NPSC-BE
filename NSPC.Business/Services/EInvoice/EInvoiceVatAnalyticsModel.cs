using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.EInvoice
{
    public class EInvoiceVatAnalyticsViewModel
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
    }

    public class EInvoiceVatAnalyticsCreateUpdateModel
    {
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
    }
}
