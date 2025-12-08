using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Data.Data.Entity.XuatNhapTon;
using NSPC.Data.Data.Entity.StockTransaction;

namespace NSPC.Data.Data.Entity.KiemKho
{
    [Table("mk_KiemKho")]
    public class mk_KiemKho : BaseTableService<mk_KiemKho>
    {
        [Key]
        public Guid Id { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string WareCode { get; set; }
        public string CheckInventoryCode { get; set; } // Mã phiếu kiểm kho
        public string Note { get; set; }
        public string Tags { get; set; }
        public string Status { get; set; } // Trạng thái kiểm
        public List<string> ListWare { get; set; }
        public DateTime? ToDate { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual idm_User CreatedByUser { get; set; }
        public virtual ICollection<sm_Stock_Transaction> Sm_Stock_Transactions { get; set; }
    }
}
