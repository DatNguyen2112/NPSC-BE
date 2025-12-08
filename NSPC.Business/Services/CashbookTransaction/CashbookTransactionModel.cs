using NSPC.Business.Services.NhaCungCap;
using NSPC.Common;
using NSPC.Data.Data.Entity.DuAn;
using NSPC.Data.Data.Entity.NhaCungCap;
using NSPC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services.DuAn;
using NSPC.Data.Data.Entity.Contract;
using NSPC.Data.Entity;
using NSPC.Business.Services.Contract;
using NSPC.Business.Services.AdvanceRequest;

namespace NSPC.Business.Services.CashbookTransaction
{
    public class CashbookDashboardViewModel
    {
        public object DashboardData { get; set; }
    }
    
    public class CashbookDashboardFilterModel
    {
        public string? Filter { get; set; } = "{}";
        public string? DateType { get; set; } = "DAYS";
          public DateTime?[] DateRange { get; set; }
        public string? TransactionType { get; set; } = "CHI";
    }
    
    public class CashbookDashboardFilterDatetimeModel
    {
        public DateTime?[] DateRange { get; set; }
        public string TransactionType { get; set; }
    }

    public class CashbookDashboardFilterDatetimeModelV2
    {
        public DateTime?[] DateRange { get; set; }
        public string TransactionType { get; set; }
    }

    public class CashbookTransactionViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Guid EntityId { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string EntityTypeCode { get; set; }
        public string EntityTypeName { get; set; }
        public string EntityUrl { get; set; }
        public Guid? OriginalDocumentId { get; set; }
        public string OriginalDocumentType { get; set; }
        public string OriginalDocumentCode { get; set; }
        public string PurposeCode { get; set; }
        public string PurposeName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethodCode { get; set; }
        public string PaymentMethodName { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string SubTypeCode { get; set; }
        public string SubTypeName { get; set; }
        public string Note { get; set; }
        public string IsActive { get; set; }
        public string TransactionTypeCode { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
        public Guid? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public bool IsDebt { get; set; }
        public Guid? ContractId { get; set; }
        public ContractViewModel Contract { get; set; }
        public Guid? ConstructionId { get; set; }
        public ConstructionViewModel Construction { get; set; }
        public Guid? AdvanceRequestId { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public AdvanceRequestViewModel AdvanceRequest { get; set; }
    }
    public class CashbookTransactionViewModelInProject
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string EntityName { get; set; }
        public string EntityTypeName { get; set; }
        public decimal Amount { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public string TransactionTypeCode { get; set; }
        public string PurposeCode { get; set; }
        public string PurposeName { get; set; }
    }
    public class CashbookTransactionCreateUpdateModel
    {
        public string Code { get; set; }
        public Guid EntityId { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        /// <summary>
        /// Nhóm người nộp trên giao diện
        /// </summary>
        public string EntityTypeCode { get; set; }
        public string EntityTypeName { get; set; }
        public Guid? OriginalDocumentId { get; set; }
        public string OriginalDocumentType { get; set; }
        public string OriginalDocumentCode { get; set; }
        public string PurposeCode { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethodCode { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string Description { get; set; }
        public string TransactionTypeCode { get; set; }
        public Guid? ProjectId { get; set; }
        public string Note { get; set; }
        public bool IsDebt { get; set; }
        public string Reference { get; set; }
        public Guid? ContractId { get; set; }
        public Guid? ConstructionId { get; set; }
        public Guid? AdvanceRequestId { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
    }

    public class CashbookTransactionCreateUpdateMutipleModel
    {
        public Guid? ConstructionId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? ContractId { get; set; }
        public List<ListCashbookTransaction> ListCashbookTransaction { get; set; }
    }

    public class ListCashbookTransaction
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Guid EntityId { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        /// <summary>
        /// Nhóm người nộp trên giao diện
        /// </summary>
        public string EntityTypeCode { get; set; }
        public string EntityTypeName { get; set; }
        public Guid? OriginalDocumentId { get; set; }
        public string OriginalDocumentType { get; set; }
        public string OriginalDocumentCode { get; set; }
        public string PurposeCode { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethodCode { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string Description { get; set; }
        public string TransactionTypeCode { get; set; }
        public string Note { get; set; }
        public bool IsDebt { get; set; }
        public string Reference { get; set; }
        public Guid? AdvanceRequestId { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
    }


    public class CashbookTransactionQueryModel : PaginationRequest
    {
        public List<string> EntityTypeCodes { get; set; }
        public string TransactionTypeCode { get; set; }
        public string PaymentMethodCode { get; set; }
        public string PurposeCode { get; set; }
        public string isActive { get; set; }
        public string?[] StatusCodes { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime?[] DateRange { get; set; }
        public string?[] ListPaymentMethodCode { get; set; }
        public string?[] ListPurposeCode { get; set; }
        public string?[] ListReceiptCode { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? ContractId { get; set; }
        public DateTime?[] ReceiptDateRange { get; set; }
        
        public Guid? ConstructionId { get; set; }

        ////////////// Báo cáo dòng tiền //////////////
        public DateTime YearDate { get; set; } = DateTime.Now;

    }
    public class CashbookTransactionSummaryModel
    {
        public decimal? TotalRevenueExpense { get; set; }
        public decimal? TotalMaterialRevenue { get; set; }
        public decimal? TotalMaterialExpense { get; set; }
        public decimal? TotalProjectRevenueExpense { get; set; }
        public decimal? TotalProductRevenueExpense { get; set; }
        public decimal? TotalInternalExpense { get; set; }
    }
    public class CashbookTransactionSummaryViewModel
    {
        public InitialInformationViewModel InitialInformation  { get; set; } =  new InitialInformationViewModel();
        public decimal OpeningBalance { get; set; } = 0M;
        public decimal ClosingBalance { get; set; } = 0M;
        public decimal TotalRevenueAmount { get; set; } = 0M;
        public decimal TotalExpenseAmount { get; set; } = 0M;
    }

    public class InitialInformationViewModel
    {
        public decimal InitialAmount { get; set; } = 0M; // Vào số dư đầu kỳ
        public Guid InitialId  { get; set; } // Id của bản ghi số dư đầu kỳ
        public DateTime? ReceiptDate  { get; set; } // Ngày ghi nhận số dư đầu kỳ
    }

    /////////////////////// Báo cáo dòng tiền ///////////////////////

    public class CashFlowReportViewModel
    {
        /// <summary>
        /// Tiền mặt hiện có đầu tháng
        /// </summary>
        public CashFlowReportItemViewModel OpeningCashBalance { get; set; }

        /// <summary>
        /// Phiếu thu (Receipt Vouchers)
        /// </summary>
        public ReceiptVouchersViewModel ReceiptVouchers { get; set; }

        /// <summary>
        /// Phiếu chi (Payment Vouchers)
        /// </summary>
        public PaymentVouchersViewModel PaymentVouchers { get; set; }
    }

    /// <summary>
    /// Model chứa danh sách phiếu thu (Receipt Vouchers)
    /// </summary>
    public class ReceiptVouchersViewModel
    {
        /// <summary>
        /// Danh sách khoản thu
        /// </summary>
        public List<CashFlowReportItemViewModel> Items { get; set; } = new();

        /// <summary>
        /// Tổng tiền thu
        /// </summary>
        public CashFlowReportItemViewModel ReceiptTotalAmount { get; set; }

        /// <summary>
        /// Tổng tiền mặt hiện có (trước khi rút tiền)
        /// </summary>
        public CashFlowReportItemViewModel TotalCashBalance { get; set; }
    }

    /// <summary>
    /// Model chứa danh sách phiếu chi (Payment Vouchers)
    /// </summary>
    public class PaymentVouchersViewModel
    {
        /// <summary>
        /// Danh sách khoản chi
        /// </summary>
        public List<CashFlowReportItemViewModel> Items { get; set; } = new();

        /// <summary>
        /// Tổng tiền mặt chi ra
        /// </summary>
        public CashFlowReportItemViewModel PaymentTotalAmount { get; set; }

        /// <summary>
        /// Tổng tiền mặt hiện tại sau khi chi
        /// </summary>
        public CashFlowReportItemViewModel TotalCashBalance { get; set; }
    }

    public class CashFlowReportItemViewModel
    {
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int? LineNumber { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// Tiền bắt đầu    
        /// </summary>
        public decimal? StartAmount { get; set; }

        /// <summary>
        /// Tiền tháng 1
        /// </summary>
        public decimal JanuaryAmount { get; set; }

        /// <summary>
        /// Tiền tháng 2
        /// </summary>
        public decimal FebruaryAmount { get; set; }

        /// <summary>
        /// Tiền tháng 3
        /// </summary>
        public decimal MarchAmount { get; set; }

        /// <summary>
        /// Tiền tháng 4
        /// </summary>
        public decimal AprilAmount { get; set; }

        /// <summary>
        /// Tiền tháng 5
        /// </summary>
        public decimal MayAmount { get; set; }

        /// <summary>
        /// Tiền tháng 6
        /// </summary>
        public decimal JuneAmount { get; set; }

        /// <summary>
        /// Tiền tháng 7
        /// </summary>
        public decimal JulyAmount { get; set; }

        /// <summary>
        /// Tiền tháng 8
        /// </summary>
        public decimal AugustAmount { get; set; }

        /// <summary>
        /// Tiền tháng 9
        /// </summary>
        public decimal SeptemberAmount { get; set; }

        /// <summary>
        /// Tiền tháng 10
        /// </summary>
        public decimal OctoberAmount { get; set; }

        /// <summary>
        /// Tiền tháng 11
        /// </summary>
        public decimal NovemberAmount { get; set; }

        /// <summary>
        /// Tiền tháng 12
        /// </summary>
        public decimal DecemberAmount { get; set; }

        /// <summary>
        /// Tổng cộng
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
