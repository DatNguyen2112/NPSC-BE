using NSPC.Common;

namespace NSPC.Business.Services.QuanLyLaiXe
{
    public class LaiXeViewModel
    {
        public Guid Id { get; set; }
        public Guid? IdPhuongTien { get; set; }
        public string MaTaiXe { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenTaiXe { get; set; }
        public string Cccd { get; set; }
        public string? Gplx { get; set; }
        public string PhuongTien { get; set; }
        public bool? Active { get; set; }

    }
    public class LaiXeCreateUpdateModel
    {
        public DateTime? NgaySinh { get; set; }
        public string TenTaiXe { get; set; }
        public string MaTaiXe { get; set; }
        public string Cccd { get; set; }
        public string Gplx { get; set; }
        public Guid IdPhuongTien { get; set; }
        public bool? Active { get; set; }
    }
    public class LaiXeQueryModel : PaginationRequest
    {
        public string TenTaiXe { get; set; }
        public string MaTaiXe { get; set; }
        public string Cccd { get; set; }
        public string Gplx { get; set; }
        public string PhuongTien { get; set; }
        public bool Active { get; set; }
        public bool ExcludeHaveUser { get; set; }
        public Guid? ForUserId { get; set; }
    }
}
