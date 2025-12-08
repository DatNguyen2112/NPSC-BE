namespace NSPC.Business.Services
{
    public class WarehouseTransferNoteItemCreateUpdateModel
    {
        public int LineNo { get; set; } = 0;
        public decimal Quantity { get; set; } = 0;
        public string LineNote { get; set; }
        public Guid ProductId { get; set; }
    }
    
    public class WarehouseTransferNoteItemViewModel
    {
        public Guid Id { get; set; }
        public int LineNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public string LineNote { get; set; }
        public Guid ProductId { get; set; }
    }
}

