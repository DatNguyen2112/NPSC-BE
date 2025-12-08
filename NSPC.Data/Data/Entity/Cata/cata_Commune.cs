using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("cata_Commune")]
    public class cata_Commune : BaseTableDefault
    {
        [Key]
        public int CommuneCode { get; set; }
        public string CommuneName { get; set; }

        public int DistrictCode { get; set; }
        public int WardId_VP { get; set; }
        public string WardName_VP { get; set; }

        public int DistrictId_VP { get; set; }
        public string VNPAsciiName { get; set; }
        public string VTPAsciiName { get; set; }
        public string OldVTPName {get; set;}
        public int VnpostSyncStatus { get; set; }

    }
}