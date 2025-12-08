namespace NSPC.Data
{
    public class jsonb_WareCodes
    {
        public Guid? Id { get; set; }
        public decimal InitialStockQuantity { get; set; } = 0M;
        public decimal SellableQuantity { get; set; } = 0M;
        public string WareCode { get; set; }
    }
}

