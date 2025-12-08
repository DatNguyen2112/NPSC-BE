using NSPC.Business.Services.QuanLyLoaiXe;
using NSPC.Common;
namespace NSPC.Business.Services.QuanLyPhuongTien
{
    public class PhuongTienViewModel
    {
        public Guid Id { get; set; }
        public string BienSoXe { get; set; }
        public string SoKhung { get; set; }
        public string SoMay { get; set; }
        public string HangSanXuat { get; set; }
        public string Model { get; set; }
        public string NamSanXuat { get; set; }
        public string TaiTrong { get; set; }
        public Guid? IdTaiXe { get; set; }
        public string TaiXe { get; set; }
        public bool Active { get; set; }
        public Guid? LoaiXeId { get; set; }
        public LoaiXeViewModel LoaiXe { get; set; }
    }
    public class PhuongTienCreateUpdateModel
    {
        public string BienSoXe { get; set; }
        public string SoKhung { get; set; }
        public string SoMay { get; set; }
        public string HangSanXuat { get; set; }
        public string Model { get; set; }
        public string NamSanXuat { get; set; }
        public string TaiTrong { get; set; }
        public bool Active { get; set; }
        public Guid LoaiXeId { get; set; }

    }
    public class PhuongTienQueryModel : PaginationRequest
    {
        public bool? IsKhongTaiXe { get; set; }
        public Guid? IdTaiXe { get; set; }

    }
}
