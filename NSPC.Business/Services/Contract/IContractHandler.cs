using Microsoft.AspNetCore.Http;
using NSPC.Common;

namespace NSPC.Business.Services.Contract
{
    public interface IContractHandler
    {
        Task<Response<ContractViewModel>> Create(ContractCreateUpdateModel model);
        Task<Response<ContractViewModel>> Update(Guid id, ContractCreateUpdateModel model);
        Task<Response<Pagination<ContractViewModel>>> GetPage(ContractQueryModel query);
        Task<Response<Dictionary<string, int>>> CountByStatus(ContractQueryModel query);
        Task<Response<ContractDetailViewModel>> GetById(Guid id);
        Task<Response> Delete(Guid id);
        Task<(Response, Stream, string)> ExportExcelFile(ContractQueryModel query);
        Task<Response> ImportExcelFile(IFormFile formFile, bool overwrite);
        Task<Response<Pagination<DebtReportViewModel>>> GetPageDebtReport(DebtReportQueryModel query);
    }
}