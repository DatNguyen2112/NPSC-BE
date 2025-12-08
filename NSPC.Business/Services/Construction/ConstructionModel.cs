using NSPC.Business.Services.ConstructionActitvityLog;
using NSPC.Business.Services.Contract;
using NSPC.Business.Services.ExecutionTeams;
using NSPC.Business.Services.ProjectTemplate;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Entity;

namespace NSPC.Business.Services
{
    public class ConstructionCreateUpdateModel
    {
        /// <summary>
        /// Mã công trình/dự án
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên công trình dự án
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Loại cấp điện áp
        /// </summary>
        public string VoltageTypeCode { get; set; }

        /// <summary>
        /// Loại chủ đầu tư
        /// </summary>
        public string OwnerTypeCode { get; set; }

        /// <summary>
        /// Id Chủ đầu tư / BQLDA
        /// </summary>
        public Guid InvestorId { get; set; }

        /// <summary>
        /// Template dự án
        /// </summary>
        public Guid? ConstructionTemplateId { get; set; }

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

        public List<ExecutionTeamsCreateModel> ExecutionTeams { get; set; } = new List<ExecutionTeamsCreateModel>();

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
        /// Trạng thái công trình/dự án
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// Tên trạng thái công trình/dự án
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Chỉnh sửa giai đoạn khi thêm mới
        /// </summary>
        public List<jsonb_TemplateStage> TemplateStages { get; set; } = new List<jsonb_TemplateStage>();
    }

    public class ConstructionViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mã công trình/dự án
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên công trình dự án
        /// </summary>
        public string Name { get; set; }

        public string VoltageTypeCode { get; set; }

        /// <summary>
        /// Loại cấp điện áp
        /// </summary>
        public CodeTypeListModel Voltage { get; set; }

        /// <summary>
        /// Loại chủ đầu tư
        /// </summary>
        public string OwnerTypeCode { get; set; }

        /// <summary>
        /// Template dự án
        /// </summary>
        public Guid? ConstructionTemplateId { get; set; }

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
        /// Trạng thái công trình/dự án
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// Tên trạng thái công trình/dự án
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        public Guid InvestorId { get; set; }

        /// <summary>
        /// Nhân sự tham gia
        /// </summary>
        public string ExecutionParticipantTeams { get; set; }

        /// <summary>
        /// Người theo dõi
        /// </summary>
        public string ExecutionFollowerTeams { get; set; }

        /// <summary>
        /// Tổ thực hiện
        /// </summary>
        public string ExecutionProjectTeams { get; set; }

        public ProjectTemplateViewModel ProjectTemplate { get; set; }
        public List<ExecutionTeamsViewModel> ExecutionTeams { get; set; }
        public List<ConstructionActivityLogViewModel> ActivityLogs { get; set; }
        public List<IssueManagementDTO> IssueManagements { get; set; }
        public List<TaskViewModelDTO> Tasks { get; set; }
        public List<ContractDTO> Contracts { get; set; }
        public Boolean IsHasIssue { get; set; } = false;
        public InvestorDTOViewModel Investor { get; set; }

        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }

        /// <summary>
        /// Danh sách giai đoạn của template dự án
        /// </summary>
        public List<jsonb_TemplateStage> TemplateStages { get; set; }
    }

    public class InvestorDTOViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public InvestorTypeDTO InvestorType { get; set; }
    }

    public class InvestorTypeDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class TaskViewModelDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Description { get; set; }
        public string PriorityLevel { get; set; }
        public string? Status { get; set; }
        public Guid TemplateStageId { get; set; }
        public Guid ConstructionId { get; set; }
    }

    public class IssueManagementDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public UserModel User { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string PriorityLevel { get; set; }
        public string Content { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public List<AttachmentViewModel> AttachmentsResolve { get; set; }
        public string ReasonReopen { get; set; }
        public string ReasonCancel { get; set; }
        public string ContentResolve { get; set; }
    }

    public class ContractDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string ContractNumber { get; set; }
        public Guid ConstructionId { get; set; }
        public Guid ConsultingServiceId { get; set; }
        public CodeTypeViewModel ConsultingService { get; set; }
        public decimal? ValueBeforeVatAmount { get; set; }
        public List<ContractAppendixItemViewModel> Appendices { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string ImplementationStatus { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public string LastModifiedByUserName { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
    }

    public class ConstructionDashboardViewModel
    {
        public object AmountData { get; set; }
        public object ContractData { get; set; }
        public object MaterialData { get; set; }
        public object QuantityOfMaterialData { get; set; }
        public object AdvanceData { get; set; }
    }

    public class ConstructionChartByStatus
    {
        public decimal Value { get; set; } = 0M; // Số lượng công trình dự án
        public string Name { get; set; } // Tên trạng thái / Tên vật tư trong dự án;
    }

    public class TopAmountAnalyze
    {
        public string Title { get; set; }
        public decimal Amount { get; set; } = 0M;
    }

    #region Thống kê tổng dự án (Phục vụ cho phân quyền không bị bắn message nhiều lần)
    public class ConstructionSummary
    {
        public ConstructionAnalyzeViewModel ConstructionAnalyzeViewModelData { get; set; }
        public List<ConstructionAnalyzeByPriorityOrInvestor> ListConstructionAnalyzeByPriorityData { get; set; }
        public List<ConstructionAnalyzeByPriorityOrInvestor> ListConstructionAnalyzeByInvestorData { get; set; }
        public List<ConstructionAnalyzeByPriorityOrInvestor> ListConstructionQuantityByInvestorData { get; set; }
        public List<TopFiveConstructionHasBigQuality> ListTopFiveConstructionHasBigQualityData { get; set; }
        public List<TopAmountAnalyze> ListTopFiveConstructionHasBigDebtData { get; set; }
        public List<TopFiveInvestorHasLowQuality> ListTopFiveInvestorHasLowQualityData { get; set; }
        public List<TopConstructionHasIssue> ListTopConstructionHasIssueData { get; set; }
    }
    #endregion

    #region Thống kê tổng hợp đồng trong dự án (Phục vụ cho phân quyền không bị bắn message nhiều lần)
    public class ContractSummary
    {
        public AnalyzeRevenueContract AnalyzeRevenueContractData { get; set; }
        public List<AnalyzeContractAmount> AnalyzeContractAmountData { get; set; }
        public List<AnalyzePercent> AnalyzePercentData { get; set; }
        public List<AnalyzePercent> AnalyzeApprovePercentData { get; set; }
        public List<AnalyzeByInvestor> AnalyzeByInvestorData { get; set; }
    }
    #endregion

    #region Thống kê dự án theo nhiều tiêu chí
    public class ConstructionAnalyzeViewModel
    {
        public ConstructionAnalyzeByVoltage ConstructionAnalyzeByVoltageData { get; set; }
        public ConstructionAnalyzeByDocumentStatus ConstructionAnalyzeByDocumentStatusData { get; set; }
        public ConstructionAnalyzeByStatus ConstructionAnalyzeByStatusData { get; set; }
        public ConstructionAnalyzeByIssue ConstructionAnalyzeByIssueData { get; set; }
        public ContractAnalyzeByConsultService ConstructionAnalyzeByConsultServiceData { get; set; }
    }

    public class ConstructionAnalyzeByIssue
    {
        public decimal TotalConstructionQuantity { get; set; } = 0M; // Tổng số dự án
        public decimal TotalConstructionHasIssueQuantity { get; set; } = 0M; // Tổng số dự án có vướng mắc
        public decimal TotalConstructionNotIssueQuantity { get; set; } = 0M; // Tổng số dự án không vướng mắc
        public decimal ConstructionHasIssuePercent { get; set; } = 0M; // Phần trăm dự án có vướng mắc
        public decimal ConstructionNotIssuePercent { get; set; } = 0M; // Phần trăm dự án không có vướng mắc
    }

    public class ConstructionAnalyzeByVoltage
    {
        public decimal TotalConstructionHas110kV { get; set; } = 0M; // 110kV
        public decimal TotalConstructionHas220kV { get; set; } = 0M; // 220kV
        public decimal TotalConstructionHasMediumVoltage { get; set; } = 0M; // Trung áp
        public decimal ConstructionHas110kVPercent { get; set; } = 0M; // Phần trăm 110kV
        public decimal ConstructionHas220kVPercent { get; set; } = 0M; // Phần trăm 220KV
        public decimal ConstructionHasMediumVoltagePercent { get; set; } = 0M; // Phần trăm trung áp
    }

    public class ConstructionAnalyzeByDocumentStatus
    {
        public decimal TotalConstructionApproved { get; set; } = 0M; // Đã phê duyệt
        public decimal TotalConstructionNotApproved { get; set; } = 0M; // Chưa phê duyệt
        public decimal ConstructionApprovedPercent { get; set; } = 0M; // Phần trăm phê duyệt
        public decimal ConstructionNotApprovedPercent { get; set; } = 0M; // Phần trăm chưa phê duyệt
    }

    public class ConstructionAnalyzeByStatus
    {
        public decimal TotalConstructionIsDesigning { get; set; } = 0M; // Đang thiết kế
        public decimal TotalConstructionSupervisorAuthor { get; set; } = 0M; // Giám sát tác giả
        public decimal TotalConstructionIsDesigningPercent { get; set; } = 0M; // Phần trăm đang thiết kế
        public decimal TotalConstructionSupervisorAuthorPercent { get; set; } = 0M; // Phần trăm giám sát tác giả
    }

    public class ContractAnalyzeByConsultService
    {
        public decimal TotalContractByKSTK { get; set; } = 0M; // KS, TK
        public decimal TotalContractByTest { get; set; } = 0M; // Thẩm tra
        public decimal ContractByKSTKPercent { get; set; } = 0M; // Phần trăm KS, TK
        public decimal ContractByTestPercent { get; set; } = 0M; // Phần trăm thẩm tra
    }
    #endregion

    #region Tỉ lệ dự án theo mức độ ưu tiên / Tỉ lệ dự án theo chủ đầu tư / Số lượng dự án theo chủ đầu tư
    public class ConstructionAnalyzeByPriorityOrInvestor
    {
        public decimal Value { get; set; } = 0M; // Số lượng công trình dự án
        public string Name { get; set; } // Tên mức độ ưu tiên / Tên chủ đầu tư trong dự án;
    }

    public class ConstructionPriorityViewModel
    {
        public string PriorityCode { get; set; }
        public string PriorityName { get; set; }
    }

    public class ConstructionInvestorTypeViewModel
    {
        public string OwnerTypeCode { get; set; }
        public string OwnerTypeName { get; set; }
    }
    #endregion

    #region Top 5 dự án có giá trị nghiệm thu trước VAT lớn nhất
    public class TopFiveConstructionHasBigQuality
    {
        public string ConstructionName { get; set; } // Tên dự án
        public decimal TotalHasExportBill { get; set; } = 0M; // Đã xuất hoá đơn
        public decimal TotalRemaining { get; set; } = 0M; // Còn lại
        public decimal TotalExpectedAmountBeforeVAT { get; set; } = 0M; // Giá trị quyết toán
    }
    #endregion

    #region Top 5 chủ đầu tư có công nợ cao
    public class TopFiveInvestorHasBigDebt
    {
        public string InvestorName { get; set; } // Tên chủ đầu tư
        public decimal Value { get; set; } = 0M; // Giá trị
    }
    #endregion

    #region Top 5 dự án có tiến độ chậm nhất
    public class TopFiveInvestorHasLowQuality
    {
        public string ConstructionCode { get; set; } // Mã CT dự án
        public decimal ConstructionProcess { get; set; } = 0M; // Phần trăm tiến độ
        public string ConstructionName { get; set; } // Tên công trình
        public string StatusName { get; set; } // Tình trạng dự án
        public string StatusCode { get; set; } // Mã tình trạng dự án
        public Guid ConstructionId { get; set; } // Id Công trình
        public decimal TotalTemplateStageQuantity { get; set; } = 0M; // Số giai đoạn
        public List<ExecutionTeamsViewModel> ExecutionTeams { get; set; } = new List<ExecutionTeamsViewModel>(); // Tổ thực hiện
        public decimal TotalTaskQuantity { get; set; } = 0M;
        public decimal TotalTaskCompletedQuantity { get; set; } = 0M;
        public decimal TotalTaskExpiredDateQuantity { get; set; } = 0M;
    }
    #endregion

    #region Top dự án có vướng mắc chưa xử lý
    public class TopConstructionHasIssue
    {
        public string ConstructionName { get; set; }
        public List<IssueManagementDTO> IssueManagements { get; set; } = new List<IssueManagementDTO>();
        public Guid ConstructionId { get; set; }
        public decimal TotalIssuePending { get; set; }
        public decimal TotalIssueExpired { get; set; }
    }
    #endregion

    #region Thống kê hợp đồng theo nhiều tiêu chí
    public class AnalyzeRevenueContract
    {
        public decimal TotalContractQuantity { get; set; } = 0M; // Số lượng hợp đồng
        public decimal TotalExpectedAmountBeforeVAT { get; set; } = 0M; // Tổng giá trị hợp đồng (trước VAT)
        public decimal TotalExpectedAmount { get; set; } = 0M; // Giá trị dự kiến
        public decimal TotalReceiptAmount { get; set; } = 0M; // Giá trị nghiệm thu
        public decimal TotalAmountHasExportBillOrder { get; set; } = 0M; // Giá trị đã xuất hoá đơn
        public decimal TotalRemainingAmount { get; set; } = 0M; // Giá trị còn lại
    }
    #endregion

    #region Số lượng và giá trị hợp đồng (trước VAT) theo chủ đầu tư
    public class AnalyzeContractAmount
    {
        public string InvestorName { get; set; }
        public decimal ContractQuantity { get; set; } = 0M;
        public decimal ContractAmount { get; set; } = 0M;
    }
    #endregion

    #region Tỉ lệ số lượng hợp đồng đã phê duyệt / Tỷ lệ giá trị hợp đồng (trước VAT) đã phê duyệt
    public class AnalyzePercent
    {
        public string Name { get; set; }
        public decimal Value { get; set; } = 0M;
    }
    #endregion

    #region Tổng sản lượng dự kiến và giá trị nghiệm thu theo chủ đầu tư
    public class AnalyzeByInvestor
    {
        public string InvestorName { get; set; } // Tên chủ đầu tư
        public decimal ExpectedQuantity { get; set; } = 0M; // Sản lượng dự kiến
        public decimal ReceivedAmount { get; set; } = 0M; // Giá trị nghiệm thu
    }
    #endregion

    public class OverloadEmployeeModel
    {
        public bool IsOverload { get; set; } = false;
        public List<EmployeeHasOverload> ListEmployeeHasOverloads { get; set; } = new List<EmployeeHasOverload>();
    }
    

    public class CheckOverloadModel
    {
        public Guid? ProjectId { get; set; }
        public ConstructionCreateUpdateModel PayloadEmployee { get; set; }
    }

    public class EmployeeHasOverload
    {
        public string Name { get; set; }

        public List<ConstructionHasEmployeeOverload> ListEmployeeHasOverloads { get; set; } = new List<ConstructionHasEmployeeOverload>();
    }

    public class ConstructionHasEmployeeOverload
    {
        public string NameConstruction { get; set; }
        public string CodeConstruction { get; set; }
        public string StatusCodeConstruction { get; set; }
        public string StatusNameConstruction { get; set; }
        public int TotalTaskInConstruction { get; set; } = 0;
    }

    public class ConstructionQueryModel : PaginationRequest
    {
        public DateTime?[] DateRange { get; set; }
        public string StatusCode { get; set; }
        public string ExecutionStatusCode { get; set; }
        public string DocumentStatusCode { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public Guid? InvestorId { get; set; }
        public Guid? TaskUserId { get; set; }
        public string PermissionCode { get; set; }
    }

    public class AnalyzeContractQueryModel : PaginationRequest
    {
        public List<int> DeliveryDateArr { get; set; }
        public List<string> InvestorCodes { get; set; }
    }
}