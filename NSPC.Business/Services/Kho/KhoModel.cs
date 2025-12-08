using NSPC.Common;

namespace NSPC.Business.Services.Business
{
    public class KhoViewModel
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public string DiaChiFull { get; set; }
        public string LoaiKho { get; set; }
        public bool IsCuaHang { get; set; }
        public UserModel CreatedByUser { get; set; }
        public Guid? KhachHangId { get; set; }
        public KhachHangViewModel KhachHang { get; set; }
        public string GhiChu { get; set; }
        public decimal? Binh { get; set; }
        public decimal? VoBinh { get; set; }
        public decimal? GasDu { get; set; }
        public int? ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int? DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int? CommuneCode { get; set; }
        public string CommuneName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Order { get; set; }
        public bool IsInitialized { get; set; }
    }

    public class KhoCreateUpdateModel
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public string LoaiKho { get; set; }
        public string GhiChu { get; set; }
        public int? ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int? DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int? CommuneCode { get; set; }
        public string CommuneName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Order { get; set; }
    }
    public class KhoQueryModel : PaginationRequest
    {
        public string LoaiKho { get; set; }
        public Guid? KhachHangId { get; set; }
        public Guid? Id { get; set; }
        public bool UnLimeted { get; set; } = false;
        public bool? IsInitialized { get; set; }
    }
}