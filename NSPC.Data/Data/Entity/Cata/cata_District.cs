using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("cata_District")]
    public class cata_District : BaseTableDefault
    {
        [Key]
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string Description { get; set; }
        public int ProvinceCode { get; set; }
        public int DistrictShippingType { get; set; }
        public int DistrictId_VP { get; set; }
        public string DistrictValue_VP { get; set; }
        public string DistrictName_VP { get; set; }
        public int ProvinceId_VP { get; set; }
        public DateTime? DelayShipFromDate { get; set; }
        public DateTime? StopShipFromDate { get; set; }
        public DateTime? DelayShipToDate { get; set; }
        public DateTime? StopShipToDate { get; set; }
        public int VnpostSyncStatus { get; set; }
    }
}