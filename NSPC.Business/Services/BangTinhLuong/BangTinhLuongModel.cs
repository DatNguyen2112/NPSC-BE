using NSPC.Common;
using NSPC.Data.Data.Entity.ChamCong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.BangTinhLuong
{
    public class BangTinhLuongViewModel
    {
        public Guid Id { get; set; }
        public string TenBangTinhLuong { get; set; }
        public string TenCongTy { get; set; }
        public string DiaChiCongTy { get; set; }
        public string MaSoThue { get; set; }
        public Guid IdBangChamCongActive { get; set; }
        public DateTime ThoiGian { get; set; }
        public int SoNgayCongTrongThang { get; set; }
        public bool KichHoatBangLuong { get; set; }
        public List<BangTinhLuongItemViewModel> BangLuongItem { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }
    public class BangTinhLuongCreateUpdateModel
    {
        //public string TenBangTinhLuong { get; set; }
        public string TenCongTy { get; set; }
        public string DiaChiCongTy { get; set; }
        public string MaSoThue { get; set; }
        public Guid IdBangChamCongActive { get; set; }
        public DateTime ThoiGian { get; set; }
        public int SoNgayCongTrongThang { get; set; }
        public bool KichHoatBangLuong { get; set; }
        public List<BangTinhLuongItemCreateUpdateModel> BangLuongItem { get; set; }
    }
    public class BangTinhLuongIdCreatedModel
    {
        public Guid Id { get; set; }
    }
    public class BangTinhLuongQueryModel : PaginationRequest
    {
        public string TenBangTinhLuong { get; set; }
        public bool? KichHoatBangLuong { get; set; }
    }
}
