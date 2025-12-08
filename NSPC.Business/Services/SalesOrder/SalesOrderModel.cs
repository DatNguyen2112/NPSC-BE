using Newtonsoft.Json;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services.Contract;
using NSPC.Business.Services.DuAn;

namespace NSPC.Business.Services
{
    /// <summary>
    /// Model để tạo và update SalesOrder
    /// </summary>
    public class SalesOrderCreateUpdateModel
    {
        /// <summary>
        /// Tham chiếu đơn hàng
        /// </summary>
        public string Reference { get; set; }
        
        /// <summary>
        /// Mã phiếu
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// Mã kho
        /// </summary>
        [StringLength(64)]
        public string WareCode { get; set; }

        /// <summary>
        /// Id Báo giá liên kết với Order
        /// </summary>
        public Guid? QuotationId { get; set; }

        /// <summary>
        /// Mã giảm giá cho đơn
        /// </summary>
        public string DiscountCode { get; set; }

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
        /// Mã loại thanh toán dự kiến
        /// </summary>
        public string PaymentMethodCode { get; set; }

        /// <summary>
        /// Chi phí vận chuyển
        /// </summary>
        public decimal DeliveryFee { get; set; }
        
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
        public decimal OtherCostAmount { get; set; }

        /// <summary>
        /// Danh sách thanh toán
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_HalfPayment> ListPayment { get; set; }

        /// <summary>
        /// Id của Customer
        /// </summary>
        public Guid CustomerId { get; set; }
        
        /// <summary>
        /// Id dự án
        /// </summary>
        public Guid? ProjectId { get; set; }
        
        /// <summary>
        /// Id công trình/dự án
        /// </summary>
        public Guid? ConstructionId  { get; set; }
        
        /// <summary>
        /// Id hợp đồng/phụ lục
        /// </summary>
        public Guid? ContractId  { get; set; }

        /// <summary>
        /// Danh sách các Item trong đơn
        /// </summary>
        public List<SalesOrderItemCreateUpdateModel> SalesOrderItems { get; set; }
    }

    public class SalesOrderViewModel
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
        /// Thông tin kho
        /// </summary>
        public CodeTypeListModel Ware { get; set; }

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
        public string DiscountCode { get; set; }

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
        /// Mã trạng thái thanh toán
        /// </summary>
        public string PaymentStatusCode { get; set; }

        /// <summary>
        /// Trạng thái thanh toán
        /// </summary>
        public CodeTypeListModel PaymentStatus { get; set; }

        /// <summary>
        /// Mã loại thanh toán dự kiến
        /// </summary>
        public string PaymentMethodCode { get; set; }

        /// <summary>
        /// Payment Method
        /// </summary>
        public CodeTypeListModel PaymentMethod { get; set; }

        /// <summary>
        /// Tổng tiền của các sản phẩm (Line Amount)
        /// </summary>
        public decimal? SubTotal { get; set; }

        /// <summary>
        /// Chi phí vận chuyển
        /// </summary>
        public decimal DeliveryFee { get; set; }
        
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
        public decimal OtherCostAmount { get; set; }

        /// <summary>
        /// Tổng thành tiền khách phải trả
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Tổng SL SP
        /// </summary>
        public decimal TotalQuantity { get; set; } = 0;

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
        ///  Trạng thái đơn hàng
        /// </summary>
        public StatusViewModel Status { get;set; }

        /// <summary>
        /// Mã trạng thái xuất kho
        /// </summary>
        public string ExportStatusCode { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Thông tin khách hàng
        /// </summary>
        public KhachHangViewModel Customer { get; set; }
        
        /// <summary>
        /// Id Công trình
        /// </summary>
        public Guid? ConstructionId  { get; set; }
        
        /// <summary>
        /// Thông tin công trình dự án
        /// </summary>
        public ConstructionViewModel Construction  { get; set; }
        
        /// <summary>
        /// Id dự án
        /// </summary>
        public Guid? ProjectId { get; set; }
        
        /// <summary>
        /// Dự án
        /// </summary>
        public DuAnViewModel Project { get; set; }
        
        /// <summary>
        /// Id hợp đồng/phụ lục
        /// </summary>
        public Guid? ContractId  { get; set; }
        
        /// <summary>
        /// Thông tin hợp đồng
        /// </summary>
        public ContractViewModel Contract { get; set; }

        /// <summary>
        /// Danh sách sản phẩm
        /// </summary>
        public List<SalesOrderItemViewModel> Items { get; set; }
        public bool IsReturned { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class SalesOrderQueryModel : PaginationRequest
    {
        public string OrderCode { get; set; }
        public Guid? CustomerId { get; set; }
        public string Customer { get; set; }
        public string PhoneNumber { get; set; }
        public string StatusCode { get; set; }
        public string ExportStatusCode { get; set; }
        public List<string>? PaymentStatusCodes { get; set; } 
        public Guid? CreatedByUserId { get; set; }
        public DateTime?[] DateRange { get; set; }
        public bool? IsReturned { get; set; }
    }
    
    public class SalesOrderSummaryModel
    {
        public decimal? PendingExportQuantity { get; set; }
        public decimal? PendingExportAmount { get; set; }
        public decimal? PendingPaymentQuantity { get; set; }
        public decimal? PendingPaymentAmount { get; set; }
    }

    public class SalesOrderStatusViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string BackgroundColor { get; set; }
        public string ForeColor { get; set; }
    }
}
