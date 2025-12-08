using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data.Entity.NguyenVatLieu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.CauHinhNhanSu
{
    [Table("mk_CauHinhNhanSu")]
    public class mk_CauHinhNhanSu: BaseTableService<mk_CauHinhNhanSu>
    {
        [Key]
        //public Guid Id { get; set; }
        public Guid? IdUser { get; set; }
        [ForeignKey("IdUser")]
        public virtual idm_User idm_User { get; set; }
        public decimal? LuongCoBan { get; set; }
        public decimal? AnCa { get; set; }
        public decimal? DienThoai { get; set; }
        public decimal? TrangPhuc { get; set; }
    }
}
