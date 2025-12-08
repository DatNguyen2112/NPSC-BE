using Microsoft.AspNetCore.Http;
using NSPC.Business.Services.Contract;
using NSPC.Business.Services.ExecutionTeams;
using NSPC.Common;
using NSPC.Data;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface IConstructionHandler
    {
        #region Process Construction Handler
        Task<Response<ConstructionViewModel>> Create(ConstructionCreateUpdateModel model, RequestUser currentUser);
        Task<Response<ConstructionViewModel>> Delete(Guid id, RequestUser currentUser);
        Task<Response<ConstructionViewModel>> GetById(Guid id, RequestUser currentUser);
        Task<Response<ConstructionViewModel>> Update(Guid id, ConstructionCreateUpdateModel model, RequestUser currentUser);
        Task<Response<Pagination<ConstructionViewModel>>> GetPage(ConstructionQueryModel query);
        Task<Response<Pagination<ExecutionTeamsViewModel>>> GetExecutionTeamsInConstruction(ExecutionTeamsQueryModel query);
        Task<(Response, Stream, string)> ExportExcelFile(ConstructionQueryModel model);
        // Task<Response<string>> ExportListToExcel(ConstructionQueryModel query, RequestUser currentUser);
        Task<Response> ImportExcelFile(IFormFile formFile, bool overwrite);
        // Task<Response<List<ConstructionViewModel>>> Import(string path, RequestUser currentUser);
        #endregion

        #region Dashboard Analyze
        // Thống kê tổng dashboard
        Task<Response<ConstructionSummary>> ConstructionSummaryAll(ConstructionQueryModel query);
        // Thống kê tổng hợp đồng
        Task<Response<ContractSummary>> ContractSummaryAll(AnalyzeContractQueryModel query);
        // // Thống kê theo tiêu chí
        // Task<Response<ConstructionAnalyzeViewModel>> ConstructionAnalyzeAllDashboard(ConstructionQueryModel query);
        // // Thống kê tỷ lệ dự án theo mức độ ưu tiên
        // Task<Response<List<ConstructionAnalyzeByPriorityOrInvestor>>> ChartPercentPriorityInConstruction(ConstructionQueryModel query);
        // // Thống kê tỷ lệ dự án theo chủ đầu tư
        // Task<Response<List<ConstructionAnalyzeByPriorityOrInvestor>>> ChartPercentInvestorInConstruction(ConstructionQueryModel query);
        // // Số lượng dự án theo chủ đầu tư
        // Task<Response<List<ConstructionAnalyzeByPriorityOrInvestor>>> ChartConstructionQuantityByInvestor(ConstructionQueryModel query);
        // // Top 5 dự án có giá trị nghiệm thu (trước VAT) lớn nhất
        // Task<Response<List<TopFiveConstructionHasBigQuality>>> ChartTopFiveConstructionHasBigQuality(ContractQueryModel query);
        // // Top 5 dự án có tiến độ chậm nhất
        // Task<Response<List<TopFiveInvestorHasLowQuality>>> TopFiveInvestorHasLowQuality(ConstructionQueryModel query);
        // // Top 5 chủ đầu tư theo công nợ
        // Task<Response<List<TopAmountAnalyze>>> TopCashbookTransactionAnalyze(DebtReportQueryModel query);
        // // Top 5 dự án có giá trị nghiệm thu (trước VAT) lớn nhất
        // Task<Response<List<TopConstructionHasIssue>>> ChartTopConstructionHasIssue(ConstructionQueryModel query);
        // // Thống kê hợp đồng theo nhiều tiêu chí
        // Task<Response<AnalyzeRevenueContract>> ChartRevenueContract(AnalyzeContractQueryModel query);
        // // Số lượng và giá trị hợp đồng (trước VAT) theo chủ đầu tư
        // Task<Response<List<AnalyzeContractAmount>>> ChartAnalyzeContractAmount(AnalyzeContractQueryModel query);
        // // Tỉ lệ số lượng hợp đồng đã phê duyệt
        // Task<Response<List<AnalyzePercent>>> ChartAnalyzeContractApprovePercent(AnalyzeContractQueryModel query);
        // // Tỷ lệ giá trị hợp đồng (trước VAT) đã phê duyệt
        // Task<Response<List<AnalyzePercent>>> ChartAnalyzeRevenueContractApprovePercent(AnalyzeContractQueryModel query);
        // // Tổng sản lượng dự kiến và giá trị nghiệm thu theo chủ đầu tư
        // Task<Response<List<AnalyzeByInvestor>>> ChartAnalyzeByInvestor(AnalyzeContractQueryModel query);
        #endregion
        Task<Response<ConstructionViewModel>> ToggleTemplateStageIsDone(Guid constructionId, Guid templateStageId, RequestUser currentUser);
        Task<Response<ConstructionViewModel>> UpdateTemplateStages(Guid constructionId, List<jsonb_TemplateStage> templateStages, RequestUser currentUser);
        Task<Response<List<jsonb_TemplateStage>>> GetTemplateStagesWithIsDoneStatus(Guid constructionId);

        Task<Response> CheckOverloadedEmployeesAsync(
            CheckOverloadModel model);
    }
}

