using NSPC.Business.Services.InventoryNote;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface IPurchaseOrderHandler
    {
        Task<Response<PurchaseOrderViewModel>> Create(PurchaseOrderCreateUpdateModel model, RequestUser currentUser);
        Task<Response<PurchaseOrderViewModel>> Update(Guid id, PurchaseOrderCreateUpdateModel model, RequestUser currentUser);
        Task<Response<PurchaseOrderViewModel>> Delete(Guid id);
        Task<Response<PurchaseOrderViewModel>> GetById(Guid id);
        Task<Response<Pagination<PurchaseOrderViewModel>>> GetPage(PurchaseOrderQueryModel query);
        Task<Response> ImportApply(List<PurchaseOrderCreateUpdateModel> model, string typePhieu);
        Task<Response<string>> ExportExcel(Guid idPhieu);
        Task<Response<PurchaseOrderViewModel>> Reciept(Guid id, RequestUser currentUser);
        Task<Response<PurchaseOrderViewModel>> RejectOrder(Guid id);
        Task<Response<PurchaseOrderViewModel>> ChargePaymentOrder(Guid id, PurchaseOrderCreateUpdateModel model, RequestUser currentUser);
        //Task<Response<SaleOrderViewModel>> UpdateStatus(Guid id, TrangThaiPhieuNhapXuatModel model)
        Task<Response<string>> ExportListToExcel(PurchaseOrderQueryModel query);

    }
}
