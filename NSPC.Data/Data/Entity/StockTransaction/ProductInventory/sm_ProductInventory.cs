using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Data.Entity.VatTu;

namespace NSPC.Data.Entity
{
    [Table("sm_ProductInventory")]
    public class sm_ProductInventory : BaseTableService<sm_ProductInventory>
    {
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Mã kho 
        /// </summary>
        [StringLength(64)]
        public string WarehouseCode { get; set; }
        
        /// <summary>
        /// Số lượng có thể bán trong kho
        /// </summary>
        public decimal SellableQuantity { get; set; } = 0M;
        
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual sm_Product Sm_Product { get; set; }
    }
}

