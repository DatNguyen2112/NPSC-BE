using NSPC.Data.Data.Entity.Bom;
using NSPC.Data.Data.Entity.NguyenVatLieu;
using NSPC.Data.Data.Entity.NhomVatTu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.VatTu
{
    [Table("sm_Product")]
    public class sm_Product : BaseTableService<sm_Product>
    {
        [Key]
        public Guid Id { get; set; }
        public virtual ICollection<mk_NguyenVatLieu> mk_NguyenVatLieu { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public bool IsActive { get; set; }
        public bool IsOrder { get; set; }
        public decimal PurchaseUnitPrice { get; set; }
        public decimal InitialStockQuantity { get; set; }
        public decimal SellingUnitPrice { get; set; }
        //public decimal? IntitalInventory {  get; set; } // Tồn kho ban đầu 
        public string Description { get; set; }
        [Column(TypeName = "jsonb")]
        public List<jsonb_Attachment> Attachments { get; set; }

        public string Note { get; set; }

        public Guid? ProductGroupId { get; set; }
        [ForeignKey("ProductGroupId")]
        public virtual mk_NhomVatTu mk_NhomVatTu { get; set; }

        /// <summary>
        /// Cho áp dụng thuế
        /// </summary>
        public bool IsVATApplied { get; set; } = false;

        /// <summary>
        /// Thuế % nhập hàng
        /// </summary>
        public decimal ImportVATPercent { get; set; } = 0M;

        /// <summary>
        /// Thuế % bán hàng
        /// </summary>
        public decimal ExportVATPercent { get; set; } = 0M;
        
        /// <summary>
        /// Barcode / Mã sản phẩm
        /// </summary>
        public string Barcode { get; set; }
        
        /// <summary>
        /// Danh sách khởi tạo kho ban đầu
        /// </summary
        [Column(TypeName = "jsonb")]
        public List<jsonb_WareCodes> ListWareCodes { get; set; }

        /// <summary>
        /// Số lượng kế hoạch
        /// </summary>
        public decimal PlanQuantity { get; set; } = 0M;

        /// <summary>
        /// Số lượng thực tế
        /// </summary>
        public decimal ActualQuantity { get; set; } = 0M;
        
        /// <summary>
        /// Số lượng chênh lệch (Thực tế - kế hoạch)
        /// </summary>
        public decimal BalanceQuantity  { get; set; } = 0M;


        /// <summary>
        /// Số lượng có thể bán của tổng tồn các kho cộng lại
        /// </summary>
        public decimal SellableQuantity { get; set; } = 0M;

        /// <summary>
        /// Mã thuế nhập hàng
        /// </summary>
        [StringLength(32)]
        public string ImportVATCode { get; set; }

        /// <summary>
        /// Mã thuế bán hàng
        /// </summary>
        [StringLength(32)]
        /// 
        public string ExportVATCode { get; set; }
        
    }
}
