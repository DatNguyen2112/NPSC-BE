using NSPC.Business.Services.NguyenVatLieu;
using NSPC.Business.Services.NhomVatTu;
using NSPC.Business.Services.VatTu;
using NSPC.Common;
using NSPC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.Bom
{
    public class BomViewModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Mã
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Sản phẩm
        /// </summary>
        public ProductViewModelInBom Product { get; set; }
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
        /// <summary>
        /// Danh sách các chi phí khác
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_OtherCost> ListOtherCost { get; set; }
        /// <summary>
        /// Tổng tiền các chi phí khác
        /// </summary>
        public decimal TotalOtherExpenses { get; set; } = 0M;
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        public List<MaterialViewModel> Materials { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }
    public class BomCreateUpdateModel
    {
        /// <summary>
        /// Mã
        /// </summary>
        [RegularExpression(@"^[a-zA-Z0-9_-]*$", ErrorMessage = "Mã không được chứa ký tự đặc biệt")]
        public string Code { get; set; }
        public Guid ProductId { get; set; }
        /// <summary>
        /// Danh sách chi phí khác
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_OtherCost> ListOtherCost { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        public List<MaterialCreateUpdateModel> Materials { get; set; }
    }

    public class MaterialViewModel
    {
        public Guid Id { get; set; }
        public int LineNumber { get; set; }
        public Guid MaterialId { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string MaterialUnit { get; set; }
        public decimal LineVatPercent { get; set; } = 0M;
        public decimal LineVatAmount { get; set; } = 0M;
        public decimal Quantity { get; set; } = 0M;
        public decimal UnitPrice { get; set; } = 0M;
        public decimal LineAmount { get; set; } = 0M;
        public Guid BomId { get; set; }
    }

    public class MaterialCreateUpdateModel
    {
        public Guid MaterialId { get; set; }
        public int LineNumber { get; set; } = 0;
        public string MaterialUnit { get; set; }
        public decimal Quantity { get; set; } = 0M;
        public decimal UnitPrice { get; set; } = 0M;
        public Guid BomId { get; set; }
    }

    public class ProductViewModelInBom
    {

        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public bool IsActive { get; set; }
        public bool IsOrder { get; set; }
        public decimal PurchaseUnitPrice { get; set; }
        public decimal InitialStockQuantity { get; set; }
        public decimal? InitalInventory { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
    }

    public class BomQueryModel : PaginationRequest
    {
        public DateTime?[] DateRange { get; set; }
    }
}
