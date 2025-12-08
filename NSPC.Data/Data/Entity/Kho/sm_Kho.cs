using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    public class sm_Kho : BaseTableService<sm_Kho>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public string LoaiKho { get; set; }
        public bool IsCuaHang { get; set; }
        public string GhiChu { get; set; }
        public int? ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int? DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int? CommuneCode { get; set; }
        public string CommuneName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual idm_User CreatedByUser { get; set; }
        public Guid? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual sm_Customer Customer { get; set; }
        public decimal? Binh { get; set; }
        public decimal? VoBinh { get; set; }
        public decimal? GasDu { get; set; }
        public int? Order { get; set; }
        public bool IsInitialized { get; set; }
        public virtual sm_PhuongTien PhuongTien { get; set; }
    }
}
