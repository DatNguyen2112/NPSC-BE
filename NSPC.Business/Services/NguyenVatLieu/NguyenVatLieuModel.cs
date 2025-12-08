using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.NguyenVatLieu
{
    public class NguyenVatLieuViewModel
    {
        public Guid Id { get; set; }
        public string MaNguyenVatLieu { get; set; }
        public string TenNguyenVatLieu { get; set; }
        public string DonViTinh { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public Guid IdBom { get; set; }
        public Guid IdVatTu { get; set; }
        public decimal? TongTien { get; set; }
    }
    public class NguyenVatLieuCreateUpdateModel
    {
        public Guid IdVatTu { get; set; }
        public Guid Id { get; set; }

        public string DonViTinh { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? TongTien { get; set; }
    }
    public class NguyenVatLieuQueryModel : PaginationRequest
    {
        public string MaNguyenVatLieu { get; set; }
        public string TenNguyenVatLieu { get; set; }
        public string DonViTinh { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public Guid IdBom { get; set; }
    }
}
