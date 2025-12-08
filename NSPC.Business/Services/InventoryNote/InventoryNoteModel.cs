using NSPC.Business.Services.Contract;
using NSPC.Business.Services.Quotation;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.InventoryNote
{
    /// <summary>
    /// Inventory Note View Model
    /// </summary>
    public class InventoryNoteViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public Guid? EntityId { get; set; } // ID đối tượng: Khách hàng || Nhà cung cấp

        public string EntityCode { get; set; }

        public string EntityName { get; set; }

        public string EntityTypeCode { get; set; } // Mã loại đối tượng: value ---> customer || supplier

        public string EntityTypeName { get; set; } // Tên loại đối tượng: value ---> Khách hàng || Nhà cung cấp

        public Guid? OriginalDocumentId { get; set; }

        public string OriginalDocumentType { get; set; }

        public string OriginalDocumentCode { get; set; }

        public string TransactionTypeCode { get; set; }

        public string TransactionTypeName { get; set; }

        public DateTime TransactionDate { get; set; }

        public string InventoryCode { get; set; } // Mã kho

        public string InventoryName { get; set; } // Tên kho

        public Guid? ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string Note { get; set; }
        
        public decimal TotalQuantity { get; set; }

        public string TypeCode { get; set; } // Loại phiếu: value ---> inventory_import, inventory_export

        public string StatusCode { get; set; } // Mã trạng thái: value ---> DRAFT, COMPLETED, CANCELLED

        public string StatusName { get; set; } // Tên trạng thái: value ---> Nháp, Hoàn thành, Đã hủy

        public List<InventoryNoteItemViewModel> InventoryNoteItems { get; set; }

        public Guid? ContractId { get; set; }
        public ContractViewModel Contract { get; set; }

        public Guid? ConstructionId { get; set; }
        public ConstructionViewModel Construction { get; set; }
        
        /// <summary>
        /// Id Yêu cầu vật tư
        /// </summary>
        public Guid? MaterialRequestId  { get; set; }
        
        /// <summary>
        /// Mã yêu cầu vật tư
        /// </summary>
        public string MaterialRequestCode  { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid? LastModifiedByUserId { get; set; }

        public DateTime? LastModifiedOnDate { get; set; }

        public DateTime CreatedOnDate { get; set; }

        public string CreatedByUserName { get; set; }

        public string LastModifiedByUserName { get; set; }
    }

    /// <summary>
    /// Inventory Note Create/Update Model
    /// </summary>
    public class InventoryNoteCreateUpdateModel
    {
        [StringLength(64, ErrorMessage = "Mã phiếu không được vượt quá 64 ký tự.")]
        public string Code { get; set; }

        public Guid? EntityId { get; set; } // ID đối tượng: Khách hàng || Nhà cung cấp

        [StringLength(64, ErrorMessage = "Mã đối tượng không được vượt quá 64 ký tự.")]
        public string EntityCode { get; set; }

        [StringLength(256, ErrorMessage = "Tên đối tượng không được vượt quá 256 ký tự.")]
        public string EntityName { get; set; }

        [StringLength(64, ErrorMessage = "Mã loại đối tượng không được vượt quá 64 ký tự.")]
        public string EntityTypeCode { get; set; } // Mã loại đối tượng: value ---> customer || supplier

        [StringLength(128, ErrorMessage = "Tên loại đối tượng không được vượt quá 128 ký tự.")]
        public string EntityTypeName { get; set; } // Tên loại đối tượng: value ---> Khách hàng || Nhà cung cấp

        public Guid? OriginalDocumentId { get; set; }

        [StringLength(64, ErrorMessage = "Loại tài liệu gốc không được vượt quá 64 ký tự.")]
        public string OriginalDocumentType { get; set; }

        [StringLength(64, ErrorMessage = "Mã tài liệu gốc không được vượt quá 64 ký tự.")]
        public string OriginalDocumentCode { get; set; }

        [StringLength(64, ErrorMessage = "Mã loại giao dịch không được vượt quá 64 ký tự.")]
        public string TransactionTypeCode { get; set; }

        public DateTime? TransactionDate { get; set; } // Ngày nhập/xuất kho

        [StringLength(64, ErrorMessage = "Mã kho không được vượt quá 64 ký tự.")]
        public string InventoryCode { get; set; } // Mã kho

        public Guid? ProjectId { get; set; }

        [StringLength(256, ErrorMessage = "Tên dự án không được vượt quá 256 ký tự.")]
        public string ProjectName { get; set; }

        [StringLength(512, ErrorMessage = "Ghi chú không được vượt quá 512 ký tự.")]
        public string Note { get; set; }

        [StringLength(64, ErrorMessage = "Loại phiếu không được vượt quá 64 ký tự.")]
        public string TypeCode { get; set; } // Loại phiếu: value ---> inventory_import, inventory_export
        
        /// <summary>
        /// Id Yêu cầu vật tư
        /// </summary>
        public Guid? MaterialRequestId  { get; set; }

        public List<InventoryNoteItemCreateUpdateModel> InventoryNoteItems { get; set; }

        public Guid? ContractId { get; set; }

        public Guid? ConstructionId { get; set; }

        public Guid CreatedByUserId { get; set; }
    }

    /// <summary>
    /// Inventory Note Item View Model
    /// </summary>
    public class InventoryNoteItemViewModel
    {
        public Guid Id { get; set; }

        public int LineNumber { get; set; }

        public Guid ProductId { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string Unit { get; set; }

        public decimal Quantity { get; set; }

        public string Note { get; set; }
    }

    /// <summary>
    /// Inventory Note Item Create/Update Model
    /// </summary>
    public class InventoryNoteItemCreateUpdateModel
    {
        public int LineNumber { get; set; }

        public Guid ProductId { get; set; }

        [StringLength(64, ErrorMessage = "Mã sản phẩm không được vượt quá 64 ký tự.")]
        public string ProductCode { get; set; }

        [StringLength(512, ErrorMessage = "Tên sản phẩm không được vượt quá 512 ký tự.")]
        public string ProductName { get; set; }

        [StringLength(128, ErrorMessage = "Đơn vị không được vượt quá 128 ký tự.")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Số lượng phải là một giá trị không âm.")]
        public decimal Quantity { get; set; }

        [StringLength(128, ErrorMessage = "Ghi chú không được vượt quá 128 ký tự.")]
        public string Note { get; set; }
    }

    public class InventoryNoteViewModelInProject
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Guid EntityId { get; set; } // ID đối tượng: Khách hàng || Nhà cung cấp
        public string EntityName { get; set; }
        public string EntityTypeName { get; set; }
        public DateTime TransactionDate { get; set; }
        public string InventoryCode { get; set; } // Mã kho
        public string InventoryName { get; set; } // Tên kho
        public string StatusCode { get; set; } // Mã trạng thái: value ---> DRAFT, COMPLETED, CANCELLED
        public string StatusName { get; set; } // Tên trạng thái: value ---> Nháp, Hoàn thành, Đã hủy
        public DateTime CreatedOnDate { get; set; }
        public string TransactionTypeCode { get; set; }
        public string TransactionTypeName { get; set; }
    }

    /// <summary>
    /// Inventory Note Query Model
    /// </summary>
    public class InventoryNoteQueryModel : PaginationRequest
    {
        public List<string> EntityTypeCodes { get; set; }
        public string TypeCode { get; set; }
        public string StatusCode { get; set; }
        public DateTime?[] DateRange { get; set; }

        public Guid? ConstructionId { get; set; }
    }
}
