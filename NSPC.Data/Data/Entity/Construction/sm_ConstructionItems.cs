using NSPC.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Data.Entity.VatTu;

namespace NSPC.Data.Entity
{
    [Table("sm_ConstructionItems")]
    public class sm_ConstructionItems: BaseTableService<sm_ConstructionItems>
    {
       [Key]
       public Guid Id { get; set; }
       
       /// <summary>
       /// Line Number
       /// </summary>
       public int LineNo { get; set; } = 0;
       
       /// <summary>
       /// Mã vật tư
       /// </summary>
       public string Code  { get; set; }
        
       /// <summary>
       /// Tên vật tư
       /// </summary>
       public string Name  { get; set; }
        
       /// <summary>
       /// Đơn vị tính
       /// </summary>
       public string Unit  { get; set; }
       
       /// <summary>
       /// Id Sản phẩm
       /// </summary>
       public Guid ProductId  { get; set; }
       [ForeignKey("ProductId")]
       public sm_Product sm_Product { get; set; }
       
       /// <summary>
       /// Id Công trình
       /// </summary>
       public Guid ConstructionId  { get; set; }
       [ForeignKey("ConstructionId")]
       public sm_Construction sm_Construction { get; set; }
       
       /// <summary>
       /// SL thực tế
       /// </summary>
       public decimal PlannedQuantity { get; set; } = 0M;
       
       /// <summary>
       /// SL kế hoạch (không đổi)
       /// </summary>
       public decimal PlannedDefaultQuantity { get; set; } = 0M;
       
       /// <summary>
       /// Số lượng thực tế
       /// </summary>
       /// <returns></returns>
       public decimal RealQuantity { get; set; } = 0M;
       
       /// <summary>
       /// Số lượng chênh lệch
       /// </summary>
       public decimal GapQuantity  { get; set; } = 0M;
       
       /// <summary>
       /// Ghi chú dòng
       /// </summary>
       public string LineNote  { get; set; }
    }   
}