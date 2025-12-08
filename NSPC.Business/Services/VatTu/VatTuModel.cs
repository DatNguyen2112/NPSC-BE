using NSPC.Business.Services.NhomVatTu;
using NSPC.Common;
using NSPC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.VatTu
{
    public class VatTuViewModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public Guid? ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsOrder { get; set; }
        public decimal PurchaseUnitPrice { get; set; }
        public decimal InitialStockQuantity { get; set; }
        
        /// <summary>
        /// Danh sách khởi tạo kho ban đầu
        /// </summary
        public List<jsonb_WareCodes> ListWareCodes { get; set; }

        /// <summary>
        /// Số lượng có thể bán được tính lại sau mỗi lần tạo đơn hàng
        /// </summary>
        public decimal SellableQuantity { get; set; } = 0M;
        
        public decimal SellingUnitPrice { get; set; }
        public decimal? InitalInventory {  get; set; }
        
        /// <summary>
        /// Barcode / Mã sản phẩm
        /// </summary>
        public string Barcode { get; set; }
        
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
        
        public AttachmentViewModel AvatarUrl { get; set; }
        public AttachmentViewModel AttachmentUrl { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public string Note { get; set; }
        public NhomVatTuViewModel NhomVatTu { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
        public decimal ExportVATPercent { get; set; }
        public decimal ImportVATPercent { get; set;}
        public bool IsVATApplied { get; set; }
    }
    public class VatTuCreateUpdateModel
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public decimal PurchaseUnitPrice { get; set; }
        public decimal InitialStockQuantity { get; set; } = 0M;
        public decimal SellingUnitPrice { get; set; }
        public decimal? InitalInventory { get; set; }
        public bool IsActive { get; set; }
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
        /// Danh sách khởi tạo kho ban đầu
        /// </summary
        public List<jsonb_WareCodes> ListWareCodes { get; set; }

        /// <summary>
        /// Số lượng có thể bán được tính lại sau mỗi lần tạo đơn hàng
        /// </summary>
        public decimal SellableQuantity { get; set; } = 0M;
        
        /// <summary>
        /// Barcode / Mã sản phẩm
        /// </summary>
        public string Barcode { get; set; }
        
        public List<jsonb_Attachment> Attachments { get; set; }
        public string Note { get; set; }
        //public Guid IdNhomVatTu { get; set; }
        public Guid? ProductGroupId { get; set; }
        public bool IsOrder { get; set; }
    }
    public class VatTuQueryModel : PaginationRequest
    {
        public string Type { get; set; }
        public string TenVatTu { get; set; }
        public string MaVatTu { get; set; }
        public bool TrangThai { get; set; }
        public string IdNhomVatTu { get; set; }
        public DateTime?[] DateRange { get; set; }
        public bool? IsOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}
