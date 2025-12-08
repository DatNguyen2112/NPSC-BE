using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Data.Data.Entity.Contract;
using NSPC.Data.Data.Entity.DuAn;

namespace NSPC.Data.Entity
{
    // Bảng DB Bán hàng
    [Table("sm_SalesOrder")]
    public class sm_SalesOrder : BaseTableService<sm_SalesOrder>
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(64)]
        public string TypeCode { get; set; }
        [StringLength(512)]
        public string TypeName { get; set; }

        /// <summary>
        /// Tham chiếu đơn
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Mã đơn, sinh tự động
        /// </summary>
        [StringLength(64)]
        public string OrderCode { get; set; }

        /// <summary>
        /// Mã kho
        /// </summary>
        [StringLength(64)]
        public string WareCode { get; set; }
        
        /// <summary>
        /// Tên kho
        /// </summary>
        public string WareName { get; set; }

        /// <summary>
        /// Id Báo giá
        /// </summary>
        public Guid? QuotationId { get; set; }
        
        /// <summary>
        /// Mã báo giá
        /// </summary>
        public string QuotationCode { get; set; }

        /// <summary>
        /// Tổng số lượng hàng tất cả các sản phẩm
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Mã giảm giá cho đơn
        /// </summary>
        public string DiscountCode {  get; set; }

        /// <summary>
        /// Tỉ lệ chiết khấu
        /// </summary>
        public decimal DiscountPercent { get; set; } = 0M;

        /// <summary>
        /// Tiền chiết khấu
        /// </summary>
        public decimal DiscountAmount { get; set; } = 0M;

        /// <summary>
        /// Loại chiết khấu (percent/value)
        /// </summary>
        [StringLength(10)]
        public string DiscountType { get; set; }

        /// <summary>
        /// Số tiền thuế VAT
        /// </summary>
        public decimal VATAmount { get; set; }

        /// <summary>
        /// Địa chỉ nhận hóa đơn
        /// </summary>
        public string InvoiceReceiptAddress { get; set; }

        /// <summary>
        ///  Địa chỉ giao hàng
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// Tags
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Trạng thái thanh toán
        /// </summary>
        public string PaymentStatusCode { get; set; }

        /// <summary>
        /// Mã loại thanh toán dự kiến
        /// </summary>
        public string PaymentMethodCode { get; set; }

        /// <summary>
        /// Tổng tiền của các sản phẩm (Line Amount)
        /// </summary>
        public decimal? SubTotal {  get; set; } // Tiền khách phải trả

        /// <summary>
        /// Chi phí vận chuyển
        /// </summary>
        public decimal DeliveryFee { get; set; }
        
        /// <summary>
        /// Tổng thành tiền khách phải trả, Total = SubTotal + VATAmount + DeliveryFee - DiscountAmount
        /// </summary>
        public decimal Total { get; set; }
        
        /// <summary>
        /// Tổng SL sản phẩm, Total = SubTotal + VATAmount + DeliveryFee - DiscountAmount
        /// </summary>
        public decimal TotalQuantity { get; set; }

        /// <summary>
        /// Danh sách thanh toán
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_HalfPayment> ListPayment { get; set; }

        /// <summary>
        /// Số tiền khách đã trả
        /// </summary>
        public decimal? PaidAmount { get; set; }

        /// <summary>
        /// Số tiền khách còn phải thanh toán
        /// </summary>
        public decimal? RemainingAmount { get; set; }

        /// <summary>
        /// Trạng thái đơn bán: đã hoàn thành, đã hủy, đang giao dịch
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// Trạng thái xuất kho
        /// </summary>
        public string ExportStatusCode { get; set; } // Trạng thái xuất kho

        public Guid CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual sm_Customer sm_Customer { get; set; }

        /// <summary>
        /// Discount Reason
        /// </summary>
        public string DiscountReason { get; set; }

        /// <summary>
        /// Tổng discount cả của sản phẩm và của đơn
        /// </summary>
        
        /// <summary>
        /// Ngày hủy
        /// </summary>
        public DateTime? CancelledOnDate { get; set; }
        
        /// <summary>
        /// Lý do hủy
        /// </summary>
        [StringLength(128)]
        public string? CancelledReason { get; set; }
        
        /// <summary>
        /// Chi phí khác
        /// </summary>
        public decimal OtherCostAmount { get; set; } //Chi phí khác
        
        public decimal TotalDiscountAmount { get; set; }    

        public virtual ICollection<sm_SalesOrderItem> SalesOrderItems { get; set; }

        public bool IsReturned { get; set; } // Đã trả hết hay chưa ?
        
        /// <summary>
        /// Dự án
        /// </summary>
        public Guid? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual mk_DuAn mk_DuAn { get; set; }
        
        /// <summary>
        /// Công trình/Dự án
        /// </summary>
        public Guid? ConstructionId { get; set; }
        [ForeignKey("ConstructionId")]
        public virtual sm_Construction sm_Construction { get; set; }
        
        /// <summary>
        /// Hợp đồng/phụ lục
        /// </summary>
        public Guid? ContractId { get; set; }
        [ForeignKey("ContractId")]
        public virtual sm_Contract sm_Contract { get; set; }
    }
}
