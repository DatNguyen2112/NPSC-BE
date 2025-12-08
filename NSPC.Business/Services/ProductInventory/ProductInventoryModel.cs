namespace NSPC.Business.Services
{
    public class ProductInventoryCreateModel
    {
        public string WarehouseCode { get; set; }

        public decimal SellableQuantity { get; set; } = 0;
        
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
    }

    public class ProductInventoryViewModel
    {
        public Guid Id { get; set; }
        
        public string WarehouseCode { get; set; }

        public decimal SellableQuantity { get; set; } = 0;
        
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
    }
}

