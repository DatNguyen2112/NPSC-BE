using NSPC.Data.Data.Entity.DuAn;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.Quotation
{
    [Table("sm_Quotation")]
    public class sm_Quotation : BaseTableService<sm_Quotation>
    {
        [Key]
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTaxCode { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string? DeliveryAddress { get; set; }
        public Guid? ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string OrderCode { get; set; } //Mã đơn hàng
        public string TypeCode { get; set; } //Loại báo giá
        public DateTime DueDate { get; set; } //Ngày hiệu lực
        public string Note { get; set; }
        public decimal VatPercent { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// Loại chiết khấu (percent/value)
        /// </summary>
        [StringLength(10)]
        public string DiscountType { get; set; }

        public decimal DiscountAmount { get; set; } = 0M; //Giá trị chiết khấu

        /// <summary>
        /// Tỉ lệ chiết khấu
        /// </summary>
        public decimal DiscountPercent { get; set; } = 0M;

        public string DiscountReason { get; set; } //Lý do chiết khấu
        public decimal ShippingCostAmount { get; set; } //Chi phí vận chuyển
        public decimal OtherCostAmount { get; set; } //Chi phí khác
        public string PaymentMethodCode { get; set; } //Phương thức thanh toán
        public string PaymentMethodName { get; set; } //Phương thức thanh toán
        public string Status { get; set; }
        [ForeignKey("ProjectId")]
        public virtual mk_DuAn mk_DuAn { get; set; }
        public virtual ICollection<sm_QuotationItem> QuotationItem { get; set; }
        [ForeignKey("CustomerId")]
        public virtual sm_Customer sm_Customer { get; set; }
    }
}
