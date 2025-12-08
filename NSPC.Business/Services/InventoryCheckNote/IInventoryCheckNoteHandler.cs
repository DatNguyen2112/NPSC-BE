using NSPC.Business.Services.InventoryNote;
using NSPC.Common;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface IInventoryCheckNoteHandler
    {
        Task<Response<InventoryCheckNoteViewModel>> Create(InventoryCheckNoteCreateUpdateModel model, RequestUser byUser);
        Task<Response<InventoryCheckNoteViewModel>> Update(Guid id, InventoryCheckNoteCreateUpdateModel model, RequestUser byUser);
        Task<Response<InventoryCheckNoteViewModel>> GetById(Guid id);
        Task<Response<Pagination<InventoryCheckNoteViewModel>>> GetPage(InventoryCheckNoteQuery query);
        Task<Response> CancelMultipleAsync(List<Guid> ids);
        Task<Response<InventoryCheckNoteViewModel>> BalanceInventory(Guid id, RequestUser byUser);
        Task<Response<string>> ExportListToExcel(InventoryCheckNoteQuery query);
    }
}
