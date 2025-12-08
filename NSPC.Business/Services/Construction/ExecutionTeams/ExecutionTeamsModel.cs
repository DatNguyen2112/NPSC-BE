using NSPC.Common;

namespace NSPC.Business.Services.ExecutionTeams
{
    public class ExecutionTeamsCreateModel
    {
        /// <summary>
        /// Id nhân sự / người theo dõi
        /// </summary>
        public Guid EmployeeId  { get; set; }
      
        /// <summary>
        /// Id công trình / dự án
        /// </summary>
        public Guid ConstructionId  { get; set; }
      
        /// <summary>
        /// Loại nhân sự (người theo dõi/nhân sự thực hiện)
        /// </summary>
        public string UserType  { get; set; }
    }

    public class ExecutionTeamsViewModel
    {
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
        public string MaPhongBan { get; set; }

        /// <summary>
        /// Mã tổ
        /// </summary>
        public string MaTo { get; set; }
        
        public CodeTypeListModel PhongBan  { get; set; }
        
        public CodeTypeItemViewModel ToThucHien { get; set; }
      
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
      
        public ConstructionViewModel Construction { get; set; }
    }

    public class ExecutionTeamsQueryModel : PaginationRequest
    {
        public string MaPhongBan { get; set; }
        
        public Guid ConstructionId  { get; set; }
        
        public DateTime?[] DateRange  { get; set; }
    }
}

