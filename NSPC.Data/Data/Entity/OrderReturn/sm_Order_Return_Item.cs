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
    // Bảng DB Sản phẩm khách muốn hoàn trả / hoàn trả NCC
    [Table("sm_Return_Order_Item")]
    public class sm_Return_Order_Item : BaseTableService<sm_Return_Order_Item>
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Mã vật tư / sản phẩm
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Tên vật tư / sản phẩm
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; }
        
        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;

        /// <summary>
        /// Số lượng sản phẩm khách muốn trả
        /// </summary>
        public decimal ReturnedQuantity { get; set; }

        /// <summary>
        /// Số lượng sản phẩm
        /// </summary>
        public decimal InitialQuantity { get; set; }

        /// <summary>
        /// Số lượng SP trả
        /// </summary>
        public decimal RemainingQuantity { get; set; }

        /// <summary>
        /// Đơn giá gốc
        /// </summary>
        public decimal InitialUnitPrice { get; set; }

        /// <summary>
        /// Đơn giá trả
        /// </summary>
        public decimal ReturnedUnitPrice { get; set; }

        /// <summary>
        /// Thành tiền
        /// </summary>
        public decimal LineAmount { get; set; }

        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual sm_Product Sm_Product { get; set; }

        /// <summary>
        /// Id phiếu trả hàng
        /// </summary>
        public Guid ReturnOrderId { get; set; }
        [ForeignKey("ReturnOrderId")]
        public virtual sm_Return_Order sm_Return_Order { get; set; }
    }
}
