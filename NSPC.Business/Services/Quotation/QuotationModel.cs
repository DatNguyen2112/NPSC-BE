using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.Quotation
{
    public class QuotationViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTaxCode { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public KhachHangViewModelInQuotation Customer { get; set; }
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
        public decimal DiscountAmount { get; set; } //Chiết khấu
        public string DiscountReason { get; set; } //Lý do chiết khấu
        public decimal ShippingCostAmount { get; set; } //Chi phí vận chuyển
        public decimal OtherCostAmount { get; set; } //Chi phí khác
        public string PaymentMethodCode { get; set; } //Phương thức thanh toán
        public string PaymentMethodName { get; set; } //Phương thức thanh toán
        public string Status { get; set; }
        public List<QuotationItemViewModel> QuotationItem { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class QuotationViewModelInProject
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }

    public class QuotationCreateUpdateModel
    {
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
        public string? Note { get; set; }
        public decimal VatPercent { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountAmount { get; set; } //Chiết khấu
        public decimal DiscountPercent { get; set; }
        public string DiscountReason { get; set; } //Lý do chiết khấu
        public decimal ShippingCostAmount { get; set; } //Chi phí vận chuyển
        public decimal OtherCostAmount { get; set; } //Chi phí khác
        public string PaymentMethodCode { get; set; } //Phương thức thanh toán
        public string Status { get; set; }
        public List<QuotationItemCreateUpdateModel> QuotationItem { get; set; }
    }

    public class QuotationItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Specifications { get; set; } //Quy cách
        public string Unit { get; set; } //Đơn vị tính
        public decimal Quantity { get; set; } //Số lượng
        public decimal UnitPrice { get; set; } //Đơn giá
        public decimal UnitPriceDiscountAmount { get; set; } //Chiết khấu
        public decimal? UnitPriceDiscountPercent { get; set; }
        public string UnitPriceDiscountType { get; set; }
        public decimal LineAmount { get; set; } //Thành tiền
        public string LineNote { get; set; } //Ghi chú
        public int LineNumber { get; set; } //Số thứ tự
        public decimal LineVATPercent { get; set; }
        public decimal LineVATableAmount { get; set; }
        public decimal LineVATAmount { get; set; }
        public string LineVATCode { get; set; }
        public bool IsProductVATApplied { get; set; }
        public decimal GoodsAmount { get; set; }
        public decimal AfterLineDiscountGoodsAmount { get; set; }
        public decimal OrderDiscountAmount { get; set; }
    }
    public class QuotationItemCreateUpdateModel
    {
        public Guid ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Specifications { get; set; } //Quy cách
        public string Unit { get; set; } //Đơn vị tính
        public decimal Quantity { get; set; } //Số lượng
        public decimal UnitPrice { get; set; } //Đơn giá
        public decimal UnitPriceDiscountAmount { get; set; } //Chiết khấu
        public decimal UnitPriceDiscountPercent { get; set; }
        public string UnitPriceDiscountType { get; set; }
        public decimal AfterLineDiscountGoodsAmount { get; set; } //Thành tiền
        public string LineNote { get; set; } //Ghi chú
        public int? LineNumber { get; set; } //Số thứ tự
    }

    public class QuotationIdCreatedModel
    {
        public Guid Id { get; set; }
    }

    public class QuotationQueryModel : PaginationRequest
    {
        public string Code { get; set; }
        public string TypeCode { get; set; }
        public string OrderCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime?[] DateRange { get; set; }
    }

    public class QuotationExcelImport
    {
        public byte[] Data { get; set; }
        public string Code { get; set; }
    }

    public class KhachHangViewModelInQuotation
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public decimal DebtAmount { get; set; }
        /// <summary>
        /// Tổng chi tiêu
        /// </summary>
        public decimal ExpenseAmount { get; set; } = 0;
        /// <summary>
        /// Tổng đơn hàng
        /// </summary>
        public decimal OrderCount { get; set; } = 0;
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public int TotalQuotationCount { get; set; }
    }
}
