using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.Quotation
{
    [Table("sm_QuotationItem")]
    public class sm_QuotationItem : BaseTableService<sm_QuotationItem>
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Id Product
        /// </summary>
        public Guid ProductId { get; set; }

        [StringLength(64)]
        public string Code { get; set; }
        [StringLength(512)]
        public string Name { get; set; }
        public string Specifications { get; set; } //Quy cách
        [StringLength(128)]
        public string Unit { get; set; } //Đơn vị tính
        public decimal Quantity { get; set; } //Số lượng
        public decimal? UnitPrice { get; set; } //Đơn giá
        public decimal UnitPriceDiscountAmount { get; set; } = 0M; //Chiết khấu

        public decimal? UnitPriceDiscountPercent { get; set; }

        /// <summary>
        /// Loại chiết khấu (percent/value)
        /// </summary>
        [StringLength(10)]
        public string UnitPriceDiscountType { get; set; }

        public decimal LineAmount { get; set; } //Thành tiền
        [StringLength(128)]
        public string LineNote { get; set; } //Ghi chú
        public int LineNumber { get; set; } //Số thứ tự
        /// <summary>
        /// Tỉ lệ VAT
        /// </summary>
        public decimal LineVATPercent { get; set; }

        /// <summary>
        /// Số tiền chịu thuế (sau tất cả các chiết khấu)
        /// </summary>
        public decimal LineVATableAmount { get; set; }

        /// <summary>
        /// Số tiền thuế VAT
        /// </summary>
        public decimal LineVATAmount { get; set; }

        /// <summary>
        /// Mã thuế VAT
        /// </summary>
        public string LineVATCode { get; set; }

        /// <summary>
        /// Sản phẩm áp dụng VAT
        /// </summary>
        public bool IsProductVATApplied { get; set; }

        /// <summary>
        /// Giá trị hàng hóa trước tất cả chiết khấu (lúc chưa tính chiết khấu)
        /// </summary>
        public decimal GoodsAmount { get; set; }

        /// <summary>
        /// Giá trị hàng hóa sau chiết khấu từng Line
        /// </summary>
        public decimal AfterLineDiscountGoodsAmount { get; set; }

        /// <summary>
        /// Chiết khấu trực tiếp từ đơn hàng, phân bổ theo giá trị VATableAmount
        /// </summary>
        public decimal OrderDiscountAmount { get; set; } = 0M;

        public Guid QuotationId { get; set; }
        [ForeignKey("QuotationId")]
        public virtual sm_Quotation sm_Quotation { get; set; }
    }
}
