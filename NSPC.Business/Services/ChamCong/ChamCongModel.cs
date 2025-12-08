using NSPC.Common;
using NSPC.Data.Data.Entity.ChamCong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.ChamCong
{
    public class ChamCongViewModel
    {
        public Guid Id { get; set; }
        public string TenBangChamCong { get; set; }
        public DateTime Date { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int Cong { get; set; }
        public List<ChamCongItemViewModel> ListChamCong { get; set; }
        public bool KichHoatBangChamCong { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class ChamCongCreateUpdateModel
    {
        public string TenBangChamCong { get; set; }
        public DateTime Date { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int Cong { get; set; }
        public List<ChamCongItemCreateUpdateModel> ListChamCong { get; set; }
        public bool KichHoatBangChamCong { get; set; }
    }
    public class ChamCongIdCreatedModel
    {
        public Guid Id { get; set; }
    }
    public class ChamCongQueryModel : PaginationRequest
    {

        public string TenBangChamCong { get; set; }
        public int? Thang { get; set; }
        public int? Nam { get; set; }
        public bool? KichHoatBangChamCong { get; set; }
    }

    public class ChamCongExcelImport
    {
        public byte[] Data { get; set; }
        //public string MaPhienKiemKho { get; set; }
    }
}
