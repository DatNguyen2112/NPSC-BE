using NSPC.Data.Data.Entity.InventoryNote;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Data.Entity.Contract;

namespace NSPC.Data.Entity
{
    /// <summary>
    /// Bảng công trình/dự án
    /// </summary>
    [Table("sm_Construction")]
    public class sm_Construction : BaseTableService<sm_Construction>
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Mã công trình/dự án
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên công trình dự án
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Lịch sử hoạt động của công trình/dự án
        /// </summary>
        public virtual ICollection<sm_ConstructionActivityLog> sm_ConstructionActivityLog { get; set; } = new List<sm_ConstructionActivityLog>();

        /// <summary>
        /// Loại cấp điện áp
        /// </summary>
        public string VoltageTypeCode { get; set; }

        /// <summary>
        /// Loại chủ đầu tư
        /// </summary>
        public string OwnerTypeCode { get; set; }

        /// <summary>
        /// Id chủ đầu tư / BQLDA
        /// </summary>
        public Guid InvestorId { get; set; }

        /// <summary>
        /// Template dự án
        /// </summary>
        public Guid? ConstructionTemplateId { get; set; }

        /// <summary>
        /// Nhân sự tham gia / người theo dõi
        /// </summary>
        public virtual ICollection<sm_ExecutionTeams> sm_ExecutionTeams { get; set; } = new List<sm_ExecutionTeams>();
        public virtual ICollection<sm_IssueManagement> sm_IssueManagements { get; set; } = new List<sm_IssueManagement>();
        public virtual ICollection<sm_Contract> sm_Contract { get; set; } = new List<sm_Contract>();
        /// <summary>
        /// Ngày giao A
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Độ ưu tiên
        /// </summary>
        public string PriorityCode { get; set; }

        /// <summary>
        /// Tên độ ưu tiên
        /// </summary>
        public string PriorityName { get; set; }

        /// <summary>
        /// Tiến độ hoàn thành theo chủ đầu tư
        /// </summary>
        public string CompletionByInvestor { get; set; }

        /// <summary>
        /// Tiến độ hoàn thành theo XNTV
        /// </summary>
        public string CompletionByCompany { get; set; }

        /// <summary>
        /// Tình hình thực hiện
        /// </summary>
        public string ExecutionStatusCode { get; set; }

        /// <summary>
        /// Tên tình hình thực hiện
        /// </summary>
        public string ExecutionStatusName { get; set; }

        /// <summary>
        /// Tình hình hồ sơ
        /// </summary>
        public string DocumentStatusCode { get; set; }

        /// <summary>
        /// Tên tình hình hồ sơ
        /// </summary>
        public string DocumentStatusName { get; set; }

        /// <summary>
        /// Tình hình dự án
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// Tình hình dự án
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        [Column(TypeName = "jsonb")]
        public List<jsonb_TemplateStage> TemplateStages { get; set; } = new List<jsonb_TemplateStage>();

        /// <summary>
        /// FK ConstructionTemplateId
        /// </summary>
        [ForeignKey("ConstructionTemplateId")]
        public virtual sm_ProjectTemplate sm_ProjectTemplate { get; set; }

        /// <summary>
        /// FK InvestorId
        /// </summary>
        [ForeignKey("InvestorId")]
        public virtual sm_Investor sm_Investor { get; set; }

        /// <summary>
        /// Danh sách công việc
        /// </summary>
        public virtual ICollection<sm_Task> Tasks { get; set; } = new List<sm_Task>();
    }
}

