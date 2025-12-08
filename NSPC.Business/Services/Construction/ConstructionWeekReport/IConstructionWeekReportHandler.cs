using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.ConstructionWeekReport
{
    public interface IConstructionWeekReportHandler
    {
        Task<Response<ConstructionWeekReportViewModel>> Create(ConstructionWeekReportCreateModel model,
            RequestUser currentUser);
        
        Task<Response<ConstructionWeekReportViewModel>> Update(Guid Id, ConstructionWeekReportCreateModel model,
            RequestUser currentUser);
        
        Task<Response<ConstructionWeekReportViewModel>> GetById(Guid Id, RequestUser currentUser);
        
        Task<Response<ConstructionWeekReportViewModel>> Delete(Guid Id, RequestUser currentUser);
        
        Task<(Response, Stream, string)> ExportExcelFile(ConstructionWeekReportQueryModel model);
        
        // Task<Response<string>> ExportListToExcel(ConstructionWeekReportQueryModel query, RequestUser currentUser);

        Task<Response<Pagination<ConstructionWeekReportViewModel>>> GetPage(ConstructionWeekReportQueryModel query);
    }
}

