using System;
using System.Threading.Tasks;
using NSPC.Common;

namespace NSPC.Business
{
    public interface ICodeTypeHandler
    {
        Task<Response> GetAdminPageAsync(CodeTypeQueryModel query);
        Task<Response<Pagination<CodeTypeListModel>>> GetListPageAsync(CodeTypeQueryModel query);
        Task<Response> GetById(Guid id);
        Task<Response> Delete(Guid id);
        Task<Response> CreateCodeType(CodeTypeCreateUpdateModel param);
        Task<Response> UpdateCodeType(CodeTypeCreateUpdateModel param, Guid id);
        Task<Response> CreateCustomerLabel(CustomerLabelCreateUpdateModel param);
        Task<Response> UpdateCustomerLabel(CustomerLabelCreateUpdateModel param, string labelCode);
        Task<Response> GetByLabelCode(string Code);


        
    }
}