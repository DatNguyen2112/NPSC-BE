using NSPC.Data.Data.Entity.JsonbEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.EInvoice
{
    [Table("sm_EInvoice")]
    public class sm_EInvoice : BaseTableService<sm_EInvoice>
    {
        [Key]
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
        [MaxLength(400)]
        public string Note { get; set; }

        /// <summary>
        /// Mã trạng thái thanh toán
        /// </summary>
        [MaxLength(64)]
        public string PaymentStatusCode { get; set; }

        /// <summary>
        /// Tên trạng thái thanh toán
        /// </summary>
        [MaxLength(64)]
        public string PaymentStatusName { get; set; }

        /// <summary>
        /// Màu trạng thái thanh toán
        /// </summary>
        [MaxLength(64)]
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
        /// Danh sách phí dịch vụ
        /// </summary>
        public virtual ICollection<sm_EInvoiceItems> EInvoiceItems { get; set; } = new List<sm_EInvoiceItems>();

        /// <summary>
        /// Danh sách các bản ghi trong bảng sm_EInvoiceVatAnalytics
        /// </summary>
        public virtual ICollection<sm_EInvoiceVatAnalytics> EInvoiceVatAnalytics { get; set; } = new List<sm_EInvoiceVatAnalytics>();
    }
}
