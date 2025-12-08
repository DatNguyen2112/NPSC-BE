using NSPC.Common;

namespace NSPC.Business.Services.Contract
{
    public class AppendixAttachmentViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public string FileUrl { get; set; }
    }

    public class ContractAppendixItemViewModel
    {
        public string Content { get; set; }
        public AppendixAttachmentViewModel Attachment { get; set; }
    }

    public class ContractViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string ContractNumber { get; set; }
        public Guid ConstructionId { get; set; }
        public ConstructionViewModel Construction { get; set; }
        public Guid TemplateStageId { get; set; }
        public string TemplateStageName { get; set; }
        public int AssignmentAYear { get; set; }
        public Guid ConsultingServiceId { get; set; }
        public CodeTypeViewModel ConsultingService { get; set; }
        public decimal? ValueBeforeVatAmount { get; set; }
        public decimal? ExpectedVolume { get; set; }
        public decimal? SettlementValueAmount { get; set; }
        public decimal? AcceptanceValueBeforeVatAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public float? TaxRatePercentage { get; set; }
        public DateTime? ContractSigningDate { get; set; }
        public int? ContractDurationDays { get; set; }
        public string Issues { get; set; }
        public string Notes { get; set; }
        public string ImplementationStatus { get; set; }
        public string AcceptanceDocumentStatus { get; set; }
        public string ExpectedApprovalMonth { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? DesignApprovalDate { get; set; }
        public string ExpectedAcceptanceMonth { get; set; }
        public string InvoiceStatus { get; set; }
        public List<DateTime> InvoiceIssuanceDates { get; set; }
        public int? AcceptanceYear { get; set; }
        public DateTime? HandoverRecordDate { get; set; }
        public DateTime? SiteSurveyRecordDate { get; set; }
        public DateTime? SurveyAcceptanceRecordDate { get; set; }
        public string SupplementaryContractRequired { get; set; }
        public string AcceptancePlan { get; set; }
        public List<ContractAppendixItemViewModel> Appendices { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public string LastModifiedByUserName { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
    }

    public class ContractDetailViewModel : ContractViewModel
    {
        public List<ActivityHistoryViewModel> ActivityHistories { get; set; } = new();
    }

    public class ContractCreateUpdateModel
    {
        public string Code { get; set; }
        public string ContractNumber { get; set; }
        public Guid ConstructionId { get; set; }
        public Guid TemplateStageId { get; set; }
        public int AssignmentAYear { get; set; }
        public Guid ConsultingServiceId { get; set; }
        public decimal? ValueBeforeVatAmount { get; set; }
        public decimal? ExpectedVolume { get; set; }
        public decimal? SettlementValueAmount { get; set; }
        public decimal? AcceptanceValueBeforeVatAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public float? TaxRatePercentage { get; set; }
        public DateTime? ContractSigningDate { get; set; }
        public int? ContractDurationDays { get; set; }
        public string Issues { get; set; }
        public string Notes { get; set; }
        public string ImplementationStatus { get; set; }
        public string AcceptanceDocumentStatus { get; set; }
        public string ExpectedApprovalMonth { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? DesignApprovalDate { get; set; }
        public string ExpectedAcceptanceMonth { get; set; }
        public string InvoiceStatus { get; set; }
        public List<DateTime> InvoiceIssuanceDates { get; set; }
        public int? AcceptanceYear { get; set; }
        public DateTime? HandoverRecordDate { get; set; }
        public DateTime? SiteSurveyRecordDate { get; set; }
        public DateTime? SurveyAcceptanceRecordDate { get; set; }
        public string SupplementaryContractRequired { get; set; }
        public string AcceptancePlan { get; set; }
        public List<ContractAppendixInputItem> Appendices { get; set; }
    }

    public class ContractAppendixInputItem
    {
        public string Content { get; set; }
        public Guid? AttachmentId { get; set; }
    }

    public class ContractQueryModel : PaginationRequest
    {
        /// <value>Mã hợp đồng</value>
        public string Code { get; set; }

        /// <value>Số hợp đồng</value>
        public string ContractNumber { get; set; }

        /// <value>ID công trình/dự án</value>
        public Guid? ConstructionId { get; set; }

        /// <value>ID giai đoạn</value>
        public Guid? TemplateStageId { get; set; }

        /// <value>ID dịch vụ tư vấn</value>
        public Guid? ConsultingServiceId { get; set; }

        /// <value>Năm giao A</value>
        public int? AssignmentAYear { get; set; }

        /// <value>Khoảng giá trị hợp đồng (trước VAT) [Min, Max]</value>
        public decimal?[] ValueBeforeVatAmountRange { get; set; }

        /// <value>Khoảng thời gian phê duyệt [FromDate, ToDate]</value>
        public DateTime?[] ApprovalDateRange { get; set; }

        /// <value>Khoảng thời gian xuất hóa đơn [FromDate, ToDate]</value>
        public DateTime?[] InvoiceIssuanceDateRange { get; set; }

        /// <value>Tình hình thực hiện</value>
        public string ImplementationStatus { get; set; }

        /// <value>Tình hình lập hồ sơ nghiệm thu</value>
        public string AcceptanceDocumentStatus { get; set; }

        /// <value>Tình hình xuất hoá đơn</value>
        public string InvoiceStatus { get; set; }

        /// <value>Cần ký PLHĐ (phụ lục hợp đồng)</value>
        public string SupplementaryContractRequired { get; set; }

        /// <value>Code cấp điện áp</value>
        public string VoltageTypeCode { get; set; }
    }

    public class DebtReportViewModel
    {
        /// <value>ID hợp đồng</value>
        public Guid? Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string InvestorTypeName { get; set; } = string.Empty;
        public decimal? AcceptanceValueBeforeVatAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public ConstructionViewModel Construction { get; set; }
    }

    public class DebtReportQueryModel : PaginationRequest
    {
        /// <value>Mã hợp đồng</value>
        public string Code { get; set; }

        /// <value>Loại tiêu chí lọc báo cáo công nợ: contract / project / investor</value>
        public string GroupBy { get; set; }

        public decimal? MinClosingDebt { get; set; }
        public decimal? MaxClosingDebt { get; set; }
        public bool? Positive { get; set; }
    }
}