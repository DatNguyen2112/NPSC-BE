using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.JsonbEntity
{
    public class jsonb_PaymentInvoice
    {
        /// <summary>
        /// Id thanh toán
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Mã phương thức thanh toán
        /// </summary>
        public string PaymentMethodCode { get; set; }

        /// <summary>
        /// Tên phương thức thanh toán
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        public decimal Amount { get; set; } = 0M;

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime PaymentOnDate { get; set; }

        /// <summary>
        /// Người thanh toán
        /// </summary>
        public string PaymentByUserName { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [MaxLength(150)]
        public string Note { get; set; }
    }
}
