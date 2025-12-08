using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    [Table("sm_ConstructionWeekReport")]
    public class sm_ConstructionWeekReport: BaseTableService<sm_ConstructionWeekReport>
    {
       [Key]
       public Guid Id  { get; set; }
       
       /// <summary>
       /// Mã báo cáo
       /// </summary>
       public string Code { get; set; }
       
       /// <summary>
       /// Tiêu đề
       /// </summary>
       public string Title  { get; set; }
       
       /// <summary>
       /// Kế hoạch tuần trước
       /// </summary>
       public string LastWeekPlan { get; set; }
       
       /// <summary>
       /// Kết quả thực hiện
       /// </summary>
       public string ProcessResult { get; set; }
       
       /// <summary>
       /// Kế hoạch tuần sau
       /// </summary>
       public string NextWeekPlan  { get; set; }
       
       /// <summary>
       /// File đính kèm 
       /// </summary>
       [Column(TypeName = "jsonb")]
       public List<jsonb_Attachment> FileAttachments { get; set; }
       
       /// <summary>
       /// Mã trạng thái
       /// </summary>
       public string StatusCode { get; set; }
       
       /// <summary>
       /// Tên trạng thái
       /// </summary>
       public string StatusName  { get; set; }
       
       /// <summary>
       /// Ngày bắt đầu 
       /// </summary>
       public DateTime? StartDate  { get; set; }
       
       /// <summary>
       /// Ngày kết thúc
       /// </summary>
       public DateTime? EndDate   { get; set; }
       
       /// <summary>
       /// FK ConstructionId
       /// </summary>
       public Guid ConstructionId  { get; set; }
       [ForeignKey("ConstructionId")]
       public virtual sm_Construction sm_Construction { get; set; }
       
       [ForeignKey("CreatedByUserId")]
       public virtual idm_User CreatedByUser { get; set; }
    }
}

