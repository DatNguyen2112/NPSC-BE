using NSPC.Business.Services.WorkingDay;
using NSPC.Common;
using NSPC.Data.Data.Entity.ChamCong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.ChamCong
{
    public class ChamCongItemViewModel
    {
        public Guid Id { get; set; }
        public string MaSo { get; set; }
        public string HoVaTen { get; set; }
        public string ChucVu { get; set; }
        public DateTime? Date { get; set; }
        public decimal? NgayCong { get; set; }
        public List<NgayTrongThangViewModel> NgayTrongThang { get; set; }
        public decimal? LamThemNgayThuong { get; set; }
        public decimal? LamThemChuNhat { get; set; }
        public decimal? LamThemNgayLe { get; set; }
        public int Order { get; set; }
        public decimal? LuongCoBan { get; set; }
        public decimal? AnCa { get; set; }
        public decimal? DienThoai { get; set; }
        public decimal? TrangPhuc { get; set; }
        public Guid IdChamCong { get; set; }
        public int? Stt { get; set; }
    }
    public class ChamCongItemCreateUpdateModel
    {
        public Guid Id { get; set; }
        public string MaSo { get; set; }
        public string HoVaTen { get; set; }
        public string ChucVu { get; set; }
        public List<NgayTrongThangCreateUpdateModel> NgayTrongThang { get; set; }
        public decimal? NgayCong { get; set; }
        public decimal? LamThemNgayThuong { get; set; }
        public decimal? LamThemChuNhat { get; set; }
        public decimal? LamThemNgayLe { get; set; }
        public int Order { get; set; }
        public decimal? LuongCoBan { get; set; }
        public decimal? AnCa { get; set; }
        public decimal? DienThoai { get; set; }
        public decimal? TrangPhuc { get; set; }
    }
    public class NgayTrongThangViewModel
    {
        public Guid Id { get; set; }
        public int Ngay { get; set; }
        public string ThuTrongTuan { get; set; }
        public string LoaiNgay { get; set; }
        public string Cong { get; set; }
    }
    public class NgayTrongThangCreateUpdateModel
    {
        public int Ngay { get; set; }
        public string ThuTrongTuan { get; set; }
        public string LoaiNgay { get; set; }
        public string Cong { get; set; }
    }
    public class ChamCongItemQueryModel : PaginationRequest
    {
    }
}
