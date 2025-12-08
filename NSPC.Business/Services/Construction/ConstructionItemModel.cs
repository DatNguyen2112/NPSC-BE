namespace NSPC.Business.Services
{
    public class ConstructionItemCreateUpdateModel
    {
        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;
        
        /// <summary>
        /// Id Product
        /// </summary>
        public Guid ProductId { get; set; }

        public decimal PlannedQuantity { get; set; } = 0M;
            
        /// <summary>
        /// Số lượng kế hoạch mặc định (không đổi)
        /// </summary>
        public decimal PlannedDefaultQuantity { get; set; } = 0M;    
        
        /// <summary>
        /// Ghi chú dòng
        /// </summary>
        public string LineNote  { get; set; }
    }    
    
    public class ConstructionItemViewModel
    {
        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;
        
        public string Code  { get; set; }
        
        public string Name  { get; set; }
        
        public string Unit  { get; set; }
        
        /// <summary>
        /// Id Product
        /// </summary>
        public Guid ProductId { get; set; }

        public decimal PlannedQuantity { get; set; }
        
        public decimal RealQuantity { get; set; }
        
        public decimal GapQuantity  { get; set; }
        
        /// <summary>
        /// Id yêu cầu vật tư
        /// </summary>
        public Guid ConstructionId  { get; set; }
        
        /// <summary>
        /// Ghi chú dòng
        /// </summary>
        public string LineNote  { get; set; }
        
        /// <summary>
        /// Trạng thái yêu cầu vật tư 
        /// </summary>
        public string StatusCode { get; set; }
        
        /// <summary>
        /// Số lượng kế hoạch mặc định (không đổi)
        /// </summary>
        public decimal PlannedDefaultQuantity { get; set; }
    }    
}
