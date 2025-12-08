using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    [Table("sm_InvestorType")]
    public class sm_InvestorType: BaseTableService<sm_InvestorType>
    {
       [Key]
       public Guid Id { get; set; }
       
       /// <summary>
       /// Mã loại chủ đầu tư
       /// </summary>
       public string Code { get; set; }
       
       /// <summary>
       /// Tên loại chủ đầu tư
       /// </summary>
       public string Name { get; set; }
       
       /// <summary>
       /// Danh sách chủ đầu tư 
       /// </summary>
       public virtual ICollection<sm_Investor> Investor { get; set;} =  new List<sm_Investor>();
    }
}
