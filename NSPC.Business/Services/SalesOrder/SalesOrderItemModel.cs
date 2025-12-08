using NSPC.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services
{
    public class SalesOrderItemViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Số dòng
        /// </summary>
        public int LineNo { get; set; }

        /// <summary>
        /// Id Product
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Đơn giá sản phẩm
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Giá sản phẩm trong báo giá nếu có
        /// </summary>
        public decimal QuotationUnitPrice { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Tỉ lệ chiết khấu trên đơn giá
        /// </summary>
        public decimal UnitPriceDiscountPercent { get; set; }

        /// <summary>
        /// Số tiền chiết khấu trên đơn giá
        /// </summary>
        public decimal UnitPriceDiscountAmount { get; set; }

        /// <summary>
        /// Loại chiết khấu cho đơn giá
        /// </summary>
        [StringLength(10)]
        public string UnitPriceDiscountType { get; set; }

        /// <summary>
        /// Tỉ lệ VAT
        /// </summary>
        public decimal VATPercent { get; set; }

        /// <summary>
        /// Số tiền chịu thuế
        /// </summary>
        public decimal VATableAmount { get; set; }

        /// <summary>
        /// Số tiền thuế VAT
        /// </summary>
        public decimal VATAmount { get; set; }

        /// <summary>
        /// Ghi chú dòng
        /// </summary>
        [StringLength(256)]
        public string Note { get; set; }

        /// <summary>
        /// Thành tiền sau thuế
        /// </summary>
        public decimal LineAmount { get; set; }

        public Guid SalesOrderId { get; set; }

        /// <summary>
        /// Chiết khấu trực tiếp từ đơn hàng, phân bổ theo giá trị VATableAmount
        /// </summary>
        public decimal OrderDiscountAmount { get; set; } = 0M;

        /// <summary>
        /// Giá trị hàng hóa trước tất cả chiết khấu
        /// </summary>
        public decimal GoodsAmount { get; set; }

        /// <summary>
        /// Số lượng hoàn trả còn lại
        /// </summary>
        public decimal RemainingQuantity { get; set; } = 0;

        /// <summary>
        /// Đã trả hêt số lượng hàng hay chưa
        /// </summary>
        public bool IsReturnedItem { get; set; } //Check thằng con Đã trả hết hay chưa ?

        /// <summary>
        /// Giá trị hàng hóa sau chiết khấu từng Line
        /// </summary>
        public decimal AfterLineDiscountGoodsAmount { get; set; }

        /// <summary>
        /// Mã thuế VAT
        /// </summary>
        public string VATCode { get; set; }

        /// <summary>
        /// Sản phẩm áp dụng VAT
        /// </summary>
        public bool IsVATApplied { get; set; }
    }

    public class SalesOrderItemCreateUpdateModel
    {
         /// <summary>
        /// Số dòng
        /// </summary>
        public int LineNo { get; set; }

        /// <summary>
        /// Id Product
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Đơn giá sản phẩm
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Giá sản phẩm trong báo giá nếu có
        /// </summary>
        public decimal QuotationUnitPrice { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Số lượng hoàn trả còn lại
        /// </summary>
        public decimal RemainingQuantity { get; set; } = 0;

        /// <summary>
        /// Tỉ lệ chiết khấu trên đơn giá
        /// </summary>
        public decimal UnitPriceDiscountPercent { get; set; }

        /// <summary>
        /// Số tiền chiết khấu trên đơn giá
        /// </summary>
        public decimal UnitPriceDiscountAmount { get; set; }

        /// <summary>
        /// Loại chiết khấu cho đơn giá
        /// </summary>
        [StringLength(10)]
        public string UnitPriceDiscountType { get; set; }

        /// <summary>
        /// Ghi chú dòng
        /// </summary>
        [StringLength(256)]
        public string Note { get; set; }
    }
}
