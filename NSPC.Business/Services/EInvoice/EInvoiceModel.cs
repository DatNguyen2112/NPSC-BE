using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Common;
using NSPC.Data.Data.Entity.JsonbEntity;

namespace NSPC.Business.Services.EInvoice
{
    public class EInvoiceViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mã hóa đơn
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên người bán
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// Mã số thuế người bán
        /// </summary>
        public string SellerTaxCode { get; set; }

        /// <summary>
        /// Địa chỉ người bán
        /// </summary>
        public string SellerAddress { get; set; }

        /// <summary>
        /// Số điện thoại người bán
        /// </summary>
        public string SellerPhoneNumber { get; set; }

        /// <summary>
        /// Số tài khoản ngân hàng người bán
        /// </summary>
        public string SellerBankAccount { get; set; }

        /// <summary>
        /// Tên ngân hàng người bán
        /// </summary>
        public string SellerBankName { get; set; }

        /// <summary>
        /// Tên người mua
        /// </summary>
        public string BuyerName { get; set; }

        /// <summary>
        /// Mã số thuế người mua
        /// </summary>
        public string BuyerTaxCode { get; set; }

        /// <summary>
        /// Địa chỉ người mua
        /// </summary>
        public string BuyerAddress { get; set; }

        /// <summary>
        /// Số điện thoại người mua
        /// </summary>
        public string BuyerPhoneNumber { get; set; }

        /// <summary>
        /// Hình thức thanh toán
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Số tài khoản ngân hàng người mua
        /// </summary>
        public string BuyerBankAccount { get; set; }

        /// <summary>
        /// Tên ngân hàng người mua
        /// </summary>
        public string BuyerBankName { get; set; }

        /// <summary>
        /// Ghi chú trên hóa đơn
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Mã trạng thái thanh toán
        /// </summary>
        public string PaymentStatusCode { get; set; }

        /// <summary>
        /// Tên trạng thái thanh toán
        /// </summary>
        public string PaymentStatusName { get; set; }

        /// <summary>
        /// Màu trạng thái thanh toán
        /// </summary>
        public string PaymentStatusColor { get; set; }

        /// <summary>
        /// Tổng thành tiền trước thuế GTGT
        /// </summary>
        public decimal TotalBeforeVatAmount { get; set; } = 0M;

        /// <summary>
        /// Tổng tiền thuế GTGT
        /// </summary>
        public decimal TotalVatAmount { get; set; } = 0M;

        /// <summary>
        /// Tổng tiền
        /// </summary>
        public decimal TotalAmount { get; set; } = 0M;

        /// <summary>
        /// Tổng tiền bằng chữ
        /// </summary>
        public string TotalAmountInWords { get; set; }

        /// <summary>
        /// Số tiền đã trả
        /// </summary>
        public decimal PaidAmount { get; set; } = 0M;

        /// <summary>
        /// Số tiền còn nợ
        /// </summary>
        public decimal StillInDebtAmount { get; set; } = 0M;

        /// <summary>
        /// Lịch sử thanh toán hóa đơn
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_PaymentInvoice> ListOfPaymentHistory { get; set; } = new List<jsonb_PaymentInvoice>();

        /// <summary>
        /// Danh sách items
        /// </summary>
        public List<EInvoiceItemsViewModel> EInvoiceItems { get; set; } = new List<EInvoiceItemsViewModel>();

        /// <summary>
        /// Danh sách vat analytics
        /// </summary>
        public List<EInvoiceVatAnalyticsViewModel> EInvoiceVatAnalytics { get; set; } = new List<EInvoiceVatAnalyticsViewModel>();

        public DateTime? LastModifiedOnDate { get; set; }

        public DateTime CreatedOnDate { get; set; }

        public string CreatedByUserName { get; set; }

        public string LastModifiedByUserName { get; set; }
    }

    public class EInvoiceCreateUpdateModel
    {
        /// <summary>
        /// Tên người bán
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// Mã số thuế người bán
        /// </summary>
        public string SellerTaxCode { get; set; }

        /// <summary>
        /// Địa chỉ người bán
        /// </summary>
        public string SellerAddress { get; set; }

        /// <summary>
        /// Số điện thoại người bán
        /// </summary>
        public string SellerPhoneNumber { get; set; }

        /// <summary>
        /// Số tài khoản ngân hàng người bán
        /// </summary>
        public string SellerBankAccount { get; set; }

        /// <summary>
        /// Tên ngân hàng người bán
        /// </summary>
        public string SellerBankName { get; set; }

        /// <summary>
        /// Tên người mua
        /// </summary>
        public string BuyerName { get; set; }

        /// <summary>
        /// Mã số thuế người mua
        /// </summary>
        public string BuyerTaxCode { get; set; }

        /// <summary>
        /// Địa chỉ người mua
        /// </summary>
        public string BuyerAddress { get; set; }

        /// <summary>
        /// Số điện thoại người mua
        /// </summary>
        public string BuyerPhoneNumber { get; set; }

        /// <summary>
        /// Hình thức thanh toán
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Số tài khoản ngân hàng người mua
        /// </summary>
        public decimal BuyerBankAccount { get; set; }

        /// <summary>
        /// Tên ngân hàng người mua
        /// </summary>
        public string BuyerBankName { get; set; }

        /// <summary>
        /// Ghi chú trên hóa đơn
        /// </summary>
        public string Note { get; set; }

        ///// <summary>
        ///// Mã trạng thái thanh toán
        ///// </summary>
        //public string PaymentStatusCode { get; set; }

        ///// <summary>
        ///// Tên trạng thái thanh toán
        ///// </summary>
        //public string PaymentStatusName { get; set; }

        ///// <summary>
        ///// Màu trạng thái thanh toán
        ///// </summary>
        //public string PaymentStatusColor { get; set; }

        ///// <summary>
        ///// Tổng thành tiền trước thuế GTGT
        ///// </summary>
        //public decimal TotalBeforeVatAmount { get; set; } = 0M;

        ///// <summary>
        ///// Tổng tiền thuế GTGT
        ///// </summary>
        //public decimal TotalVatAmount { get; set; } = 0M;

        ///// <summary>
        ///// Tổng tiền
        ///// </summary>
        //public decimal TotalAmount { get; set; } = 0M;

        /// <summary>
        /// Tổng tiền bằng chữ
        /// </summary>
        public string TotalAmountInWords { get; set; }

        ///// <summary>
        ///// Số tiền đã trả
        ///// </summary>
        //public decimal PaidAmount { get; set; } = 0M;

        ///// <summary>
        ///// Số tiền còn nợ
        ///// </summary>
        //public decimal StillInDebtAmount { get; set; } = 0M;

        ///// <summary>
        ///// Lịch sử thanh toán hóa đơn
        ///// </summary>
        //[Column(TypeName = "jsonb")]
        //public List<jsonb_PaymentInvoice> ListOfPaymentHistory { get; set; } = new List<jsonb_PaymentInvoice>();

        /// <summary>
        /// Danh sách items
        /// </summary>
        public List<EInvoiceItemsCreateUpdateModel> EInvoiceItems { get; set; } = new List<EInvoiceItemsCreateUpdateModel>();

        ///// <summary>
        ///// Danh sách vat analytics
        ///// </summary>
        //public List<EInvoiceVatAnalyticsCreateUpdateModel> EInvoiceVatAnalytics { get; set; } = new List<EInvoiceVatAnalyticsCreateUpdateModel>();
    }

    public class PaymentHistoryViewModel
    {

        /// <summary>
        /// Mã phương thức thanh toán
        /// </summary>
        public string PaymentMethodCode { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        public decimal Amount { get; set; } = 0M;

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime PaymentOnDate { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [MaxLength(150)]
        public string Note { get; set; }
    }

    public class EInvoiceQueryModel : PaginationRequest
    {
        public DateTime?[] CreatedOnDateRange { get; set; }
        public string PaymentStatusCode { get; set; }
    }
}
