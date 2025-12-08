using NSPC.Data.Data.Entity.Quotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.InventoryNote
{
    [Table("sm_InventoryNoteItem")]
    public class sm_InventoryNoteItem : BaseTableService<sm_InventoryNoteItem>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int LineNumber { get; set; }

        [StringLength(64)]
        public string ProductCode { get; set; }

        [StringLength(512)]
        public string ProductName { get; set; }

        [StringLength(128)]
        public string Unit { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Số lượng phải là một giá trị không âm.")]
        public decimal Quantity { get; set; }

        [StringLength(128)]
        public string Note { get; set; }

        [Required]
        public Guid InventoryNoteId { get; set; }

        [ForeignKey("InventoryNoteId")]
        public virtual sm_InventoryNote sm_InventoryNote { get; set; }
    }
}
