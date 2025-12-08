using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.EInvoice
{
    [Table("sm_EInvoiceItems")]
    public class sm_EInvoiceItems : BaseTableService<sm_EInvoiceItems>
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Tên phí dịch vụ
        /// </summary>
        [StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public decimal Quantity { get; set; } = 0M;

        /// <summary>
        /// Đơn giá
        /// </summary>
        public decimal UnitPrice { get; set; } = 0M;

        /// <summary>
        /// Thành tiền
        /// </summary>
        public decimal LineAmount { get; set; } = 0M;

        /// <summary>
        /// Thuế suất GTGT %
        /// </summary>
        public decimal VatPercent { get; set; } = 0M;

        /// <summary>
        /// Tiền thuế GTGT
        /// </summary>
        public decimal VatAmount { get; set; } = 0M;
    }
}
