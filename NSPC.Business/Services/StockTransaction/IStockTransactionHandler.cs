using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.StockTransaction
{
    public interface IStockTransactionHandler
    {
        Task<Response<Pagination<StockTransactionViewModel>>> GetPage(StockTransactionQueryModel query);
        Task<Response<StockTransactionViewModel>> Create(StockTransactionCreateModel model);
        Task<Response<List<StockHistoryViewModel>>> GetById(Guid id);
        Task<Response<List<StockTransactionViewModel>>> GetByIdAndWareCode(Guid id, string wareCode);
        Task<Response<List<StockTransactionViewModel>>> GetByWareCode(string wareCode);
        Task<Response<List<StockHistoryViewModel>>> GetStockHistoryOfEachWareHouse(StockTransactionQueryModel query);
        Task<Response<string>> ExportListToExcel(StockTransactionQueryModel query);
    }
}
