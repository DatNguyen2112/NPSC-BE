using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.BHXH
{
    public class BHXHViewModel
    {
        public Guid Id { get; set; }
        public decimal? BHXHNguoiLaoDong { get; set; }
        public decimal? BHYTNguoiLaoDong { get; set; }
        public decimal? BHTNNguoiLaoDong { get; set; }
        public decimal? BHXHNguoiSuDungLaoDong { get; set; }
        public decimal? BHYTNguoiSuDungLaoDong { get; set; }
        public decimal? BHTNNguoiSuDungLaoDong { get; set; }
        public decimal? TongNguoiLaoDong { get; set; }
        public decimal? TongNguoiSuDungLaoDong { get; set; }
        public decimal? TongTatCa { get; set; }
    }
    public class BHXHCreateUpdateModel
    {
        public decimal? BHXHNguoiLaoDong { get; set; }
        public decimal? BHYTNguoiLaoDong { get; set; }
        public decimal? BHTNNguoiLaoDong { get; set; }
        public decimal? BHXHNguoiSuDungLaoDong { get; set; }
        public decimal? BHYTNguoiSuDungLaoDong { get; set; }
        public decimal? BHTNNguoiSuDungLaoDong { get; set; }
    }
    public class BHXHQueryModel : PaginationRequest
    {
    }
}
