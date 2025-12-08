using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("sm_LoaiXe")]

    public class sm_LoaiXe : BaseTableService<sm_LoaiXe>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string TenLoaiXe { get; set; }
        public string MoTa { get; set; }
        public virtual ICollection<sm_PhuongTien> listPhuongTien { get; set; }
    }
}
