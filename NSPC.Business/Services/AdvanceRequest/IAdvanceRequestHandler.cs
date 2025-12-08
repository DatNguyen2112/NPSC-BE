using NSPC.Business.Services.EInvoice;
using NSPC.Common;
using NSPC.Data.Data.Entity.JsonbEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.AdvanceRequest
{
    public interface IAdvanceRequestHandler
    {
        Task<Response<AdvanceRequestViewModel>> CreateAsync(AdvanceRequestCreateUpdateModel model, RequestUser currentUser);
        Task<Response<Pagination<AdvanceRequestViewModel>>> GetPageAsync(AdvanceRequestQueryModel query);
        Task<Response<AdvanceRequestViewModel>> GetByIdAsync(Guid id);
        Task<Response<AdvanceRequestViewModel>> UpdateAsync(Guid id, AdvanceRequestCreateUpdateModel model, RequestUser currentUser);
        Task<Response> DeleteAsync(Guid id);
        Task<Response> SendAsync(Guid id, RequestUser currentUser);
        Task<Response> RejectAsync(Guid id, jsonb_AdvanceRequestHistory model, RequestUser currentUser);
        Task<Response> ApproveAsync(Guid id, RequestUser currentUser);
        //Thêm lịch sử xử lý
        Task<Response<AdvanceRequestViewModel>> AddHistoryAsync(Guid id, jsonb_AdvanceRequestHistory model);
    }
}
