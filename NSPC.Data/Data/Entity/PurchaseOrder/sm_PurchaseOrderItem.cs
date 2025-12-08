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
    [Table("sm_PurchaseOrderItem")]
    public class sm_PurchaseOrderItem : BaseTableService<sm_PurchaseOrderItem>
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;


        [StringLength(64)]
        public string ProductCode { get; set; }
        [StringLength(512)]
        public string ProductName { get; set; }


        public decimal UnitPrice { get; set; } = 0M;
        [StringLength(128)]
        public string Unit { get; set; }
        public decimal Quantity { get; set; } = 0M;

        /// <summary>
        /// Số lượng hoàn trả còn lại
        /// </summary>
        public decimal RemainingQuantity { get; set; } = 0;

        /// <summary>
        /// Đã trả hêt số lượng hàng hay chưa
        /// </summary>
        public bool IsReturnedItem { get; set; } //Check thằng con Đã trả hết hay chưa ?

        public decimal LineAmount { get; set; } = 0M;

        /// <summary>
        /// Tỉ lệ chiết khấu trên đơn giá
        /// </summary>
        public decimal UnitPriceDiscountPercent { get; set; } = 0M;

        /// <summary>
        /// Số tiền chiết khấu trên đơn giá
        /// </summary>
        public decimal UnitPriceDiscountAmount { get; set; } = 0M;

        /// <summary>
        /// Loại chiết khấu (percent/value)
        /// </summary>
        [StringLength(10)]
        public string UnitPriceDiscountType { get; set; }

        /// <summary>
        /// Đơn giá sau chiết khấu
        /// </summary>
        public decimal DiscountedUnitPrice { get; set; } = 0M;
        
        /// <summary>
        /// Ghi chú dòng
        /// </summary>
        [StringLength(256)]
        public string Note { get; set; }

        /// <summary>
        /// Sản phẩm áp dụng VAT
        /// </summary>
        public bool IsProductVATApplied { get; set; }

        /// <summary>
        /// Mã thuế VAT
        /// </summary>
        public string VATCode { get; set; }

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
        /// Chiết khấu trực tiếp từ đơn hàng, phân bổ theo giá trị VATableAmount
        /// </summary>
        public decimal OrderDiscountAmount { get; set; } = 0M;

        /// <summary>
        /// Giá trị hàng hóa trước tất cả chiết khấu
        /// </summary>
        public decimal GoodsAmount { get; set; }

        /// <summary>
        /// Giá trị hàng hóa sau chiết khấu từng Line
        /// </summary>
        public decimal AfterLineDiscountGoodsAmount { get; set; }

        public Guid PurchaseOrderId { get; set; }
        [ForeignKey("PurchaseOrderId")]
        public virtual sm_PurchaseOrder sm_PurchaseOrder { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual sm_Product Sm_Product { get; set; }
    }
}
