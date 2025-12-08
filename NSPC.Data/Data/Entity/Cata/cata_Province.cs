using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("cata_Province")]
    public class cata_Province : BaseTableDefault
    {
        [Key]
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int Order { get; set; }
        public int ProvinceShippingType { get; set; }
        public int ProvinceId_VP { get; set; }
        public string ProvinceCode_VP { get; set; }
        public string ProvinceName_VP { get; set; }
        public int ServiceType_VP { get; set; }
        public int TotalDistrictShipDelay { get; set; }
        public int TotalDistrictShipStop { get; set; }
        public DateTime? DelayShipFromDate { get; set; }
        public DateTime? StopShipFromDate { get; set; }
        public DateTime? DelayShipToDate { get; set; }
        public DateTime? StopShipToDate { get; set; }
    }
}