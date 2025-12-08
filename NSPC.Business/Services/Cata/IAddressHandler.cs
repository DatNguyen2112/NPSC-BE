using NSPC.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface IAddressHandler
    {
        Task<Response> GetTinh(AddressQueryModel query);

        Task<Response> GetHuyen(AddressQueryModel query);

        Task<Response> GetPhuong(AddressQueryModel query);

        Task<Response> UpdateProvince(float provinceId, ProvinceUpdateModel model);
        Task<Response> UpdateDistrict(float districtId, DistrictUpdateModel model);
        Task<Response> UpdateProvinceMany(List<ProvinceUpdateModel> model);
        Task<Response> UpdateDistrictMany(List<DistrictUpdateModel> model);
        Task<string> GetNotifyMessage();
      //  Task AutoUpdateShippingStatus();
    }
}