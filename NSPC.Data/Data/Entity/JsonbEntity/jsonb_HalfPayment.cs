using System;


namespace NSPC.Data
{
    public class jsonb_HalfPayment
    {
        public Guid Id { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal? LinePaidAmount { get; set; }
        public string? Reference {  get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? SupplierName { get; set; }
    }
}
