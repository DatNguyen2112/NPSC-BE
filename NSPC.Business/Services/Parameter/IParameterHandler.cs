using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface IParameterHandler
    {
        Task<Response> FindByNameAsync(string name);

        Task<Response> FindAsync(Guid id);

        Task<Response> CreateAsync(ParameterCreateModel model, Guid applicationId, Guid userId);

        Task<Response> UpdateAsync(Guid id, ParameterUpdateModel model, Guid applicationId, Guid userId);

        Task<Response> DeleteAsync(Guid id);

        Task<Response> DeleteRangeAsync(List<Guid> listId);

        Task<Response> GetAllAsync();

        Task<Response> GetAllAsync(ParameterQueryModel query);

        Task<Response> GetPageAsync(ParameterQueryModel query);

        Response GetAll();
        Task SaveEntity(ParameterModel model);
    }
}