using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.NhomVatTu
{
    public class NhomVatTuViewModel
    {
        public Guid Id { get; set; }
        public string MaNhom { get; set; }
        public string TenNhom { get; set; }
        public string GhiChu { get; set; }
    }
    public class NhomVatTuCreateUpdateModel
    {
        public string MaNhom { get; set; }
        public string TenNhom { get; set; }
        public string GhiChu { get; set; }
    }
    public class NhomVatTuQueryModel : PaginationRequest
    {
        public string MaNhom { get; set; }
        public string TenNhom { get; set; }
        public string GhiChu { get; set; }
    }
}
