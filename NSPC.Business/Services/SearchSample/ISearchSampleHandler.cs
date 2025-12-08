using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface ISearchSampleHandler
    {
        Task<Response<SearchSampleModel>> Create(SearchSampleCreateUpdateModel model);
        Task<Response<SearchSampleModel>> Update(Guid id, SearchSampleCreateUpdateModel model);
        Task<Response<SearchSampleModel>> GetById(Guid id);
        Task<Response<Pagination<SearchSampleModel>>> GetPage(SearchSampleQueryModel query);
        Task<Response> Delete(Guid id);
    }
}
