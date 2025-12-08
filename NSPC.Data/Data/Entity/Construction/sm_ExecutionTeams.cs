using NSPC.Data.Entity;
using NSPC.Data.Data.Entity.Quotation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Data.Entity.AdvanceRequest;
using NSPC.Data.Data.Entity.CashbookTransaction;
using NSPC.Data.Data.Entity.Contract;
using NSPC.Data.Data.Entity.InventoryNote;
using NSPC.Data.Data.Entity.VatTu;

namespace NSPC.Data.Entity
{
    /// <summary>
    /// Bảng tổ thực hiện
    /// </summary>
    [Table("sm_ExecutionTeams")]
    public class sm_ExecutionTeams: BaseTableService<sm_ExecutionTeams>
    {
      [Key]
      public Guid  Id { get; set; }
      
      /// <summary>
      /// Id nhân sự / người theo dõi
      /// </summary>
      public Guid EmployeeId  { get; set; }
      
      /// <summary>
      /// Id công trình / dự án
      /// </summary>
      public Guid ConstructionId  { get; set; }
      
      /// <summary>
      /// Mã phòng ban
      /// </summary>
      public string MaPhongBan  { get; set; }
      
      /// <summary>
      /// Mã tổ
      /// </summary>
      public string MaTo { get; set; }
      
      /// <summary>
      /// Ảnh nhân sư thực hiện / người theo dõi
      /// </summary>
      public string EmployeeAvatarUrl { get; set; }
      
      /// <summary>
      /// Tên nhân sự thực hiện / người theo dõi
      /// </summary>
      public string EmployeeName { get; set; }
      
      /// <summary>
      /// Loại nhân sự (người theo dõi/nhân sự thực hiện)
      /// </summary>
      public string UserType  { get; set; }
      
      /// <summary>
      /// FK ConstructionId
      /// </summary>
      [ForeignKey("ConstructionId")]
      public virtual sm_Construction sm_Construction { get; set; }
    }   
}