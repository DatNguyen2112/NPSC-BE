using NSPC.Data.Data.Entity.Quotation;
using NSPC.Data.Data.Entity.VatTu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.Bom
{
    [Table("sm_Bom")]
    public class sm_Bom : BaseTableService<sm_Bom>
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Id sản phẩmza
        /// </summary>
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual sm_Product Product { get; set; }
        /// <summary>
        /// Tổng tiền các nguyên vật liệu trong BOM
        /// </summary>
        public decimal SubTotalAmount { get; set; } = 0M;
        /// <summary>
        /// Tổng tiền cuối cùng
        /// </summary>
        public decimal TotalAmount { get; set; } = 0M;
        /// <summary>
        /// Tổng tiền thuế của các nguyên vật liệu trong BOM
        /// </summary>
        public decimal TotalVatAmount { get; set; } = 0M;
        /// <summary>
        /// Tổng số lượng nguyên vật liệu trong BOM
        /// </summary>
        public decimal TotalQuantity { get; set; } = 0M;
        public virtual ICollection<sm_Materials> Materials { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Mã
        /// </summary>
        // Validate không được có dấu cách và ký tự đặc biệt (trừ dấu gạch ngang)
        [RegularExpression(@"^[a-zA-Z0-9_-]*$", ErrorMessage = "Mã không được chứa ký tự đặc biệt")]
        public string Code { get; set; }
        /// <summary>
        /// Danh sách các chi phí khác
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_OtherCost> ListOtherCost { get; set; }
        /// <summary>
        /// Tổng tiền chi phí khác
        /// </summary>
        public decimal TotalOtherExpenses { get; set; } = 0M;
    }
}
