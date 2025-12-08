using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Entity;

namespace NSPC.Data.Data.Entity.Contract
{
    // Helper classes for Appendices JSONB field
    public class AppendixAttachment
    {
        /// <value>ID của file đính kèm</value>
        public Guid Id { get; set; }

        /// <value>File name</value>
        public string FileName { get; set; }

        /// <value>File type (e.g., pdf, docx)</value>
        public string FileType { get; set; }

        /// <value>File path or URL</value>
        public string FilePath { get; set; }
    }

    public class ContractAppendixItem
    {
        /// <value>Nội dung của mục trong phụ lục</value>
        [Required]
        public string Content { get; set; }

        /// <value>File đính kèm cho mục này (tùy chọn)</value>
        public AppendixAttachment Attachment { get; set; } // Renamed from FileDinhKem
    }

    public enum ImplementationStatus
    {
        /// <value>Đang trình duyệt</value>
        PendingApproval = 1,

        /// <value>Chưa triển khai</value>
        NotImplemented = 2,

        /// <value>Đã phê duyệt</value>
        Approved = 3,

        /// <value>Đang thực hiện</value>
        InProgress = 4,

        /// <value>Vướng mắc/Tạm dừng</value>
        OnHoldOrSuspended = 5
    }

    public enum AcceptanceDocumentStatus
    {
        /// <value>Đã chuyển HS cho CĐT</value>
        TransferredToOwner = 1,

        /// <value>Chưa lập hồ sơ</value>
        NotPrepared = 2
    }

    public enum InvoiceStatus
    {
        /// <value>Đã xuất hoá đơn</value>
        Issued = 1,

        /// <value>Chưa xuất hoá đơn</value>
        NotIssued = 2
    }

    public enum SupplementaryContractRequired
    {
        /// <value>Cần ký PLHĐ (phụ lục hợp đồng)</value>
        Required = 1,

        /// <value>Không cần ký PLHĐ (phụ lục hợp đồng)</value>
        NotRequired = 2
    }

    [Table("sm_Contract")]
    public class sm_Contract : BaseTableService<sm_Contract>
    {
        [Key]
        public Guid Id { get; set; }

        /// <value>Mã hợp đồng</value>
        [Required]
        [MaxLength(100)]
        public string Code { get; set; }

        /// <value>Số hợp đồng</value>
        [MaxLength(100)]
        public string ContractNumber { get; set; }

        /// <value>Công trình/dự án ID</value>
        [Required]
        public Guid ConstructionId { get; set; }

        /// <value>Công trình/dự án</value>
        [ForeignKey(nameof(ConstructionId))]
        public virtual sm_Construction Construction { get; set; }

        /// <value>Giai đoạn ID</value>
        [Required]
        public Guid TemplateStageId { get; set; }

        /// <value>Giai đoạn</value>
        [Required]
        [Column(TypeName = "jsonb")]
        public jsonb_TemplateStage TemplateStage { get; set; }

        /// <value>Năm giao A</value>
        [Required]
        public int AssignmentAYear { get; set; }

        /// <value>Dịch vụ tư vấn ID</value>
        [Required]
        public Guid ConsultingServiceId { get; set; }

        /// <value>Dịch vụ tư vấn (CodeType có Type là CodeTypeConstants.ConsultService)</value>
        [ForeignKey(nameof(ConsultingServiceId))]
        public virtual sm_CodeType ConsultingService { get; set; }

        /// <value>Giá trị hợp đồng (trước VAT)</value>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ValueBeforeVatAmount { get; set; }

        /// <value>Sản lượng dự kiến</value>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ExpectedVolume { get; set; }

        /// <value>Giá trị nghiệm thu (trước VAT)</value>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AcceptanceValueBeforeVatAmount { get; set; }

        /// <value>Giá trị đã xuất hóa đơn (đã thanh toán)</value>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PaidAmount { get; set; }

        /// <value>Thuế suất (ví dụ: 10%)</value>
        public float? TaxRatePercentage { get; set; }

        /// <value>Ngày ký hợp đồng</value>
        public DateTime? ContractSigningDate { get; set; }

        /// <value>Thời hạn hợp đồng (ngày)</value>
        public int? ContractDurationDays { get; set; }

        /// <value>Vướng mắc</value>
        public string Issues { get; set; }

        /// <value>Ghi chú</value>
        public string Notes { get; set; }

        /// <value>Tình hình thực hiện</value>
        [Required]
        public ImplementationStatus ImplementationStatus { get; set; }

        /// <value>Tình hình lập hồ sơ nghiệm thu</value>
        [Required]
        public AcceptanceDocumentStatus AcceptanceDocumentStatus { get; set; }

        /// <value>Tháng dự kiến phê duyệt</value>
        [MaxLength(255)]
        public string ExpectedApprovalMonth { get; set; }

        /// <value>Ngày phê duyệt</value>
        public DateTime? ApprovalDate { get; set; }

        /// <value>Ngày phê duyệt thiết kế</value>
        public DateTime? DesignApprovalDate { get; set; }

        /// <value>Tháng dự kiến nghiệm thu</value>
        [MaxLength(255)]
        public string ExpectedAcceptanceMonth { get; set; }

        /// <value>Tình hình xuất hoá đơn</value>
        [Required]
        public InvoiceStatus InvoiceStatus { get; set; }

        /// <value>Ngày xuất hóa đơn (danh sách)</value>
        [Column(TypeName = "jsonb")]
        public List<DateTime> InvoiceIssuanceDates { get; set; }

        /// <value>Năm nghiệm thu</value>
        public int? AcceptanceYear { get; set; }

        /// <value>Ngày lập BB bàn giao</value>
        public DateTime? HandoverRecordDate { get; set; }

        /// <value>Ngày lập BB khảo sát hiện trường</value>
        public DateTime? SiteSurveyRecordDate { get; set; }

        /// <value>Ngày lập BB nghiệm thu khảo sát</value>
        public DateTime? SurveyAcceptanceRecordDate { get; set; }

        /// <value>Cần ký PLHĐ</value>
        [Required]
        public SupplementaryContractRequired SupplementaryContractRequired { get; set; }

        /// <value>Kế hoạch nghiệm thu</value>
        public string AcceptancePlan { get; set; }

        /// <value>Phụ lục hợp đồng (danh sách các mục nội dung và file đính kèm)</value>
        [Column(TypeName = "jsonb")]
        public List<ContractAppendixItem> Appendices { get; set; }

        // Navigation properties for BaseTableService fields
        [ForeignKey(nameof(CreatedByUserId))]
        public virtual idm_User CreatedByUser { get; set; }

        [ForeignKey(nameof(LastModifiedByUserId))]
        public virtual idm_User LastModifiedByUser { get; set; }
    }
}