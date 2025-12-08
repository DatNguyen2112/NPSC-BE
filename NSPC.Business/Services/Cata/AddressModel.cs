using NSPC.Common;
using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace NSPC.Business
{
    public class CataProvinceModel
    {
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int TotalDistrictShipDelay { get; set; }
        public int TotalDistrictShipStop { get; set; }
        public string ProvinceName_VP { get; set; }
        [JsonIgnore]
        public DateTime? DelayShipFromDate { get; set; }
        [JsonIgnore]
        public DateTime? StopShipFromDate { get; set; }
        [JsonIgnore]
        public DateTime? DelayShipToDate { get; set; }
        [JsonIgnore]
        public DateTime? StopShipToDate { get; set; }
    }

    public class CataDistrictModel
    {
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string Description { get; set; }
        public int ProvinceCode { get; set; }
        public string DistrictName_VP { get; set; }
        public string DistrictFullName { get; set; }
        [JsonIgnore]
        public DateTime? DelayShipFromDate { get; set; }
        [JsonIgnore]
        public DateTime? StopShipFromDate { get; set; }
        [JsonIgnore]
        public DateTime? DelayShipToDate { get; set; }
        [JsonIgnore]
        public DateTime? StopShipToDate { get; set; }
    }

    public class CataCommueModel
    {
        public int CommuneCode { get; set; }
        public string CommuneName { get; set; }
        public int DistrictCode { get; set; }
        public string CommuneFullName { get; set; }
        public string WardName_VP { get; set; }
    }

    public class Province_VP
    {
        public int PROVINCE_ID { get; set; }
        public string PROVINCE_CODE { get; set; }
        public string PROVINCE_NAME { get; set; }
    }

    public class District_VP
    {
        public int DISTRICT_ID { get; set; }
        public string DISTRICT_VALUE { get; set; }
        public string DISTRICT_NAME { get; set; }
        public int PROVINCE_ID { get; set; }
    }

    public class Ward_VP
    {
        public int WARDS_ID { get; set; }
        public string WARDS_NAME { get; set; }
        public int DISTRICT_ID { get; set; }
        public string VNPAsciiName { get; set; }
        public string VTPAsciiName { get; set; }
        public string OldVTPName { get; set; }
        public bool IsUpdate { get; set; }
    }

    public class AddressQueryModel : PaginationRequest
    {
        public int? ParentId { get; set; }
    }

    public class ShippingPriceModel
    {
        public decimal ShippingPrice { get; set; }
    }

    public class ProvinceUpdateModel
    {
        public int? ProvinceId { get; set; }
        public DateTime? DelayShipFromDate { get; set; }
        public DateTime? StopShipFromDate { get; set; }
        public DateTime? DelayShipToDate { get; set; }
        public DateTime? StopShipToDate { get; set; }
    }

    public class DistrictUpdateModel
    {
        public int? DistrictId { get; set; }
        public DateTime? DelayShipFromDate { get; set; }
        public DateTime? StopShipFromDate { get; set; }
        public DateTime? DelayShipToDate { get; set; }
        public DateTime? StopShipToDate { get; set; }
    }

    public class CommuneListViewModel
    {
        public int CommuneCode { get; set; }
        public string CommuneName { get; set; }
    }
    public class DistrictListViewModel
    {
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
    }
    public class ProvinceListViewModel
    {
        public string ProvinceName { get; set; }
        public int ProvinceCode { get; set; }
    }
}