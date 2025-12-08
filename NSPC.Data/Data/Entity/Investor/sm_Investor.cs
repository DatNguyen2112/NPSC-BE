using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    [Table("sm_Investor")]
    public class sm_Investor : BaseTableService<sm_Investor>
    {
       [Key]
       public Guid  Id { get; set; }
       
       /// <summary>
       /// Mã chủ đầu tư
       /// </summary>
       public string Code  { get; set; }
       
       /// <summary>
       /// Tên chủ đầu tư
       /// </summary>
       public string Name  { get; set; }
       
       /// <summary>
       /// FK Loại chủ đầu tư
       /// </summary>
       public Guid InvestorTypeId  { get; set; }
       [ForeignKey("InvestorTypeId")]
       public virtual sm_InvestorType InvestorType  { get; set; }
    }
}
