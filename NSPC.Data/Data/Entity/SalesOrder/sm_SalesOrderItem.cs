//using NSPC.Data.Data.Entity.QuanLyKho;
using NSPC.Data.Data.Entity.VatTu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Entity
{
    [Table("sm_SalesOrderItem")]
    public class sm_SalesOrderItem : BaseTableService<sm_SalesOrderItem>
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        [StringLength(64)]
        public string ProductCode { get; set; }
        [StringLength(512)]
        public string ProductName { get; set; }

        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        [StringLength(128)]
        public string Unit {  get; set; }

        /// <summary>
        /// Số tiền thực người dùng phải trả cho sản phẩm
        /// </summary>
        public decimal LineAmount { get; set; }
        public decimal QuotationUnitPrice { get; set; } // giá góc sản phẩm từ báo giá
        public decimal? UnitPriceDiscountPercent { get; set; }
        public decimal UnitPriceDiscountAmount { get; set; } = 0M;

        /// <summary>
        /// Loại chiết khấu (percent/value)
        /// </summary>
        [StringLength(10)]
        public string UnitPriceDiscountType { get; set; }

        /// <summary>
        /// Tỉ lệ VAT
        /// </summary>
        public decimal VATPercent { get; set; }

        /// <summary>
        /// Số tiền chịu thuế (sau tất cả các chiết khấu)
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
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;

        /// <summary>
        /// Chiết khấu trực tiếp từ đơn hàng, phân bổ theo giá trị VATableAmount
        /// </summary>
        public decimal OrderDiscountAmount { get; set; } = 0M;

        /// <summary>
        /// Số lượng hoàn trả còn lại
        /// </summary>
        public decimal RemainingQuantity { get; set; } = 0;

        /// <summary>
        /// Đã trả hêt số lượng hàng hay chưa
        /// </summary>
        public bool IsReturnedItem { get; set; } //Check thằng con Đã trả hết hay chưa ?

        /// <summary>
        /// Giá trị hàng hóa trước tất cả chiết khấu
        /// </summary>
        public decimal GoodsAmount { get; set; }

        /// <summary>
        /// Giá trị hàng hóa sau chiết khấu từng Line
        /// </summary>
        public decimal AfterLineDiscountGoodsAmount { get; set; }

        /// <summary>
        /// Mã thuế VAT
        /// </summary>
        public string VATCode { get;set; }

        /// <summary>
        /// Sản phẩm áp dụng VAT
        /// </summary>
        public bool IsProductVATApplied { get; set; }  

        public Guid SalesOrderId { get; set; }
        [ForeignKey("SalesOrderId")]
        public virtual sm_SalesOrder sm_SalesOrder { get; set; }
        [ForeignKey("ProductId")]
        public virtual sm_Product sm_Product { get; set; }
    }
}
