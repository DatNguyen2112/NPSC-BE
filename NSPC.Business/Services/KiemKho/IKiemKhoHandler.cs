using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface IKiemKhoHandler
    {
        Task<Response<KiemKhoDetailViewModel>> Create(KiemKhoCreateUpdateModel model);
        Task<Response<KiemKhoDetailViewModel>> Update(Guid id, KiemKhoCreateUpdateModel model);
        Task<Response<KiemKhoDetailViewModel>> GetById(Guid id);
        Task<Response<Pagination<KiemKhoViewModel>>> GetPage(KiemKhoQueryModel query);
        Task<Response<KiemKhoDetailViewModel>> Delete(Guid id);
        Task<Response<KiemKhoDetailViewModel>> BalanceInventory(Guid id, KiemKhoCreateUpdateModel model);
        Task<Response> DeleteMany(List<Guid> ids);
        //Task<Response<StockInventoryViewModel>> GetSoLuongTonKhoVatTu(string maVatTu, string maKho, int index);
        Task<Response<List<StockInventoryViewModel>>> Import(string path);
        Task<Response<KiemKhoExcelImport>> Export(Guid id,string path);
    }
}
