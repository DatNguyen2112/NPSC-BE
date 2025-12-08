using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.BHXH
{
    [Table("mk_BHXH")]
    public class mk_BHXH : BaseTableService<mk_BHXH>
    {
        [Key]
        public Guid Id { get; set; }
        public decimal? BHXHNguoiLaoDong { get; set; }
        public decimal? BHYTNguoiLaoDong { get; set; }
        public decimal? BHTNNguoiLaoDong { get; set; }
        public decimal? BHXHNguoiSuDungLaoDong { get; set; }
        public decimal? BHYTNguoiSuDungLaoDong { get; set; }
        public decimal? BHTNNguoiSuDungLaoDong { get; set; }
        public decimal? TongNguoiLaoDong { get; set; }
        public decimal? TongNguoiSuDungLaoDong { get; set; }
        public decimal? TongTatCa { get; set; }
    }
}
