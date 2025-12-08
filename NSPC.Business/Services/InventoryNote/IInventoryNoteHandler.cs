using NSPC.Business.Services.Quotation;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.InventoryNote
{
    public interface IInventoryNoteHandler
    {
        Task<Response<InventoryNoteViewModel>> Create(InventoryNoteCreateUpdateModel model, RequestUser byUser);
        Task<Response<InventoryNoteViewModel>> CreateInventoryExportAutomatic(InventoryNoteCreateUpdateModel model, RequestUser byUser);
        Task<Response<InventoryNoteViewModel>> Update(Guid id, InventoryNoteCreateUpdateModel model, RequestUser byUser);
        Task<Response<InventoryNoteViewModel>> GetById(Guid id);
        Task<Response<Pagination<InventoryNoteViewModel>>> GetPage(InventoryNoteQueryModel query);
        Task<Response> DeleteMultipleAsync(List<Guid> ids);
        Task<Response> CancelMultipleAsync(List<Guid> ids);
        Task<Response> InventoryTransactionAsync(List<Guid> ids, RequestUser byUser);
        Task<Response> CreateExportSalesOrder(Guid id, RequestUser byUser);
        Task<Response> CreateImportPurchaseOrder(Guid id, RequestUser byUser);
        Task<Response> CreateCustomerReturn(Guid id, RequestUser byUser);
        Task<Response> CreateSupplierReturn(Guid id, RequestUser byUser);
        Task<Response<string>> ExportExcelList(string type);
        Task<Response<string>> ExportExcelListCurrentPage(string type, InventoryNoteQueryModel query);
    }
}
