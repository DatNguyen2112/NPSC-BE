//using NSPC.Business.Services.QuanLyKho;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public interface ISalesOrderHandler
    {
        Task<Response<SalesOrderViewModel>> Create(SalesOrderCreateUpdateModel model, RequestUser byUser);
        Task<Response<SalesOrderViewModel>> Update(Guid id, SalesOrderCreateUpdateModel model, RequestUser byUser);
        Task<Response<SalesOrderViewModel>> RejectOrder(Guid id);
        Task<Response<SalesOrderViewModel>> ExportOrder(Guid id, RequestUser byUser);
        Task<Response<SalesOrderViewModel>> ChargePaymentOrder(Guid id, SalesOrderCreateUpdateModel model, RequestUser byUser);
        Task<Response<SalesOrderViewModel>> GetById(Guid id);
        Task<Response<Pagination<SalesOrderViewModel>>> GetPage(SalesOrderQueryModel query);
        Task<Response<SalesOrderViewModel>> Delete(Guid id);
        Task<Response<SalesOrderSummaryModel>> GetSalesOrderSummary(
            SalesOrderQueryModel query);
        Task<Response<string>> ExportExcelList();
        Task<Response<string>> ExportExcelListCurrentPage(SalesOrderQueryModel query);
    }
}
