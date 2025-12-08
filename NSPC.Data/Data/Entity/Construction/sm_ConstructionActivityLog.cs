using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NSPC.Data.Entity
{
    [Table("sm_ConstructionActivityLog")]
    public class sm_ConstructionActivityLog: BaseTableService<sm_ConstructionActivityLog>
    {
       [Key]
       public Guid Id { get; set; }
       
       /// <summary>
       /// Nguời thao tác
       /// </summary>
       public string UserName  { get; set; }
       
       /// <summary>
       /// Ảnh người thao tác
       /// </summary>
       public string AvatarUrl  { get; set; }
       
       /// <summary>
       /// Mô tả action
       /// </summary>
       public string Description { get; set; }
       
       /// <summary>
       /// Mã phiếu có trong công trình dự án (gán link trên FE)
       /// </summary>
       public string CodeLinkDescription  { get; set; }
       
       /// <summary>
       /// Id phiếu có trong công trình dự án
       /// </summary>
       public Guid OrderId  { get; set; }
       
       /// <summary>
       /// Loại hành động
       /// </summary>
       public string ActionType { get; set; }

       /// <summary>
       /// Số thứ tự của giai đoạn
       /// </summary>
       public int StepOrder { get; set; } = 0;
       
       /// <summary>
       /// Id công trình
       /// </summary>
       public Guid? ConstructionId  { get; set; }
       [ForeignKey("ConstructionId")]
       public virtual sm_Construction sm_Construction { get; set; }
    }
}
