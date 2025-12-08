using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("sm_LaiXe")]

    public class sm_LaiXe : BaseTableService<sm_LaiXe>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Guid Id { get; set; }
        public Guid? IdPhuongTien { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenTaiXe { get; set; }
        public string MaTaiXe { get; set; }
        public string Cccd { get; set; }
        public string Gplx { get; set; }
        [ForeignKey("IdPhuongTien")]
        public virtual sm_PhuongTien PhuongTien { get; set; }
        public bool Active { get; set; }
        public Guid? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public idm_User User { get; set; }
    }
}
