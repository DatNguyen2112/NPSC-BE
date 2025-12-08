using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public class ThongKeTonKhoViewModel
    {
        public Guid IdVatTu { get; set; }
        public string MaKho { get; set; }
        public string TenKho { get; set; }
        public string MaVatTu { get; set; }
        public string TenVatTu { get; set; }
        public string DonViTinh { get; set; }
        public int? SoLuongNhap { get; set; }
        public decimal? GiaTriNhap { get; set; }
        public int? SoLuongXuat { get; set; }
        public decimal? GiaTriXuat { get; set; }
        public Guid? TenantId { get; set; }
        public int? SoLuongTon {
            get
            {
                return SoLuongNhap - SoLuongXuat;
            }
            set {} 
        }
        public decimal? GiaTriTon { 
            get 
            { 
                return GiaTriNhap - GiaTriXuat;
            } set { } }
    }

    public class ThongKeTonKhoQueryModel :PaginationRequest
    {
        public string MaKho { get; set; }
        public string Loai { get; set; }
        public DateTime[] DateRange { get; set; }
        
    }
}
