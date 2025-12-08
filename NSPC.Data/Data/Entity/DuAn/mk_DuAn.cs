using NSPC.Data.Entity;
using NSPC.Data.Data.Entity.Quotation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Data.Entity.CashbookTransaction;
using NSPC.Data.Data.Entity.InventoryNote;

namespace NSPC.Data.Data.Entity.DuAn
{
    [Table("mk_DuAn")]
    public class mk_DuAn : BaseTableService<mk_DuAn>
    {
        [Key]
        public Guid Id { get; set; }
        public virtual ICollection<sm_Quotation> sm_Quotation { get; set; }
        public virtual ICollection<sm_InventoryNote> sm_InventoryNote { get; set; }
        public virtual ICollection<sm_Cashbook_Transaction> sm_Cashbook_Transaction { get; set; }
        public virtual ICollection<sm_PurchaseOrder> sm_PurchaseOrder { get; set; }
        [Required(ErrorMessage = "Mã dự án không được để trống.")]
        [MaxLength(100)] // Optional: Giới hạn độ dài nếu cần
        public string MaDuAn { get; set; }
        [Required(ErrorMessage = "Tên dự án không được để trống.")]
        [MaxLength(200)] // Optional: Giới hạn độ dài nếu cần
        public string TenDuAn { get; set; }
        public decimal TongHopThu { get; set; } = 0M;
        public decimal TongHopChi { get; set; } = 0M;
    }
}
