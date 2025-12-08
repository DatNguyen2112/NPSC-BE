using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.BangTinhLuong
{
    [Table("mk_CacKhoanTroCap")]
    public class mk_CacKhoanTroCap : BaseTableService<mk_CacKhoanTroCap>
    {
        public Guid Id { get; set; }
        public decimal? AnCa { get; set; }
        public decimal? DienThoai { get; set; }
        public decimal? TrangPhuc { get; set; }
    }
}
