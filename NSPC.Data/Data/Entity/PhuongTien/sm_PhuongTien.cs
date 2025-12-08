using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("sm_PhuongTien")]

    public class sm_PhuongTien : BaseTableService<sm_PhuongTien>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string BienSoXe { get; set; }
        public string SoKhung { get; set; }
        public string SoMay { get; set; }
        public string HangSanXuat { get; set; }
        public string Model { get; set; }
        public string NamSanXuat { get; set; }
        public string TaiTrong { get; set; }
        public sm_LaiXe TaiXe { get; set; }
        public bool Active { get; set; }
        public Guid? WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public sm_Kho Warehouse { get; set; }

        public Guid LoaiXeId { get; set; }
        [ForeignKey(nameof(LoaiXeId))]

        public sm_LoaiXe LoaiXe { get; set; }
    }
}
