using NSPC.Common;
using static NSPC.Common.Helper;


namespace NSPC.Business.Services
{
    public interface IWarehouseTransferNoteHandler
    {
        Task<Response<WarehouseTransferNoteViewModel>> Create(WarehouseTransferNoteCreateModel model, RequestUser byUser);
        Task<Response<WarehouseTransferNoteViewModel>> Update(Guid id, WarehouseTransferNoteCreateModel model, RequestUser byUser);
        Task<Response<WarehouseTransferNoteViewModel>> GetById(Guid id);
        Task<Response> CancelMultipleAsync(List<Guid> ids);
        Task<Response<Pagination<WarehouseTransferNoteViewModel>>> GetPage(WarehouseTransferNoteQuery query);
        Task<Response<WarehouseTransferNoteViewModel>> TransferWarehouse(Guid id, RequestUser byUser);
        Task<Response<string>> ExportListToExcel(WarehouseTransferNoteQuery query);
    }
}

