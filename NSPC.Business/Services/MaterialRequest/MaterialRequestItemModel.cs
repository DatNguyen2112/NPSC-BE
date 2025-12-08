namespace NSPC.Business.Services
{
    public class MaterialRequestItemCreateUpdateModel
    {
        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;
        
        /// <summary>
        /// Id Product
        /// </summary>
        public Guid ProductId { get; set; }

        public decimal RequestQuantity { get; set; } = 0M;
        
        /// <summary>
        /// Id yêu cầu vật tư
        /// </summary>
        public Guid MaterialRequestId  { get; set; }
        
        /// <summary>
        /// Id công trình
        /// </summary>
        public Guid ConstructionId  { get; set; }
        
        /// <summary>
        /// Ghi chú dòng
        /// </summary>
        public string LineNote  { get; set; }

        public Boolean IsApprove { get; set; } = false;
    }    
    
    public class MaterialRequestItemViewModel
    {
        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;
        
        public string Code  { get; set; }
        
        public string Name  { get; set; }
        
        public string Unit  { get; set; }
        
        public decimal UnitPrice  { get; set; }
        
        public decimal ImportVATPercent  { get; set; }
        
        /// <summary>
        /// Id Product
        /// </summary>
        public Guid ProductId { get; set; }

        public decimal RequestQuantity { get; set; }

        public decimal PlannedQuantity  { get; set; }
        
        public Boolean IsApprove { get; set; }
        
        /// <summary>
        /// Id yêu cầu vật tư
        /// </summary>
        public Guid MaterialRequestId  { get; set; }
        
        /// <summary>
        /// Id công trình
        /// </summary>
        public Guid ConstructionId  { get; set; }
        
        /// <summary>
        /// Ghi chú dòng
        /// </summary>
        public string LineNote  { get; set; }
    }    
}
