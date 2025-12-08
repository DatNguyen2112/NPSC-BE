using NSPC.Data.Data.Entity.WorkingDay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.ChamCong
{
    public class NgayTrongThang
    {
        public Guid Id { get; set; }
        public int Ngay { get; set; }
        public string ThuTrongTuan { get; set; }
        public string LoaiNgay { get; set; }
        public string Cong { get; set; }
    }
}
