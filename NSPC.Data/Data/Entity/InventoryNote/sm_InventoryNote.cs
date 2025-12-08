using NSPC.Data.Data.Entity.Contract;
using NSPC.Data.Data.Entity.DuAn;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.InventoryNote
{
    [Table("sm_InventoryNote")]
    public class sm_InventoryNote : BaseTableService<sm_InventoryNote>
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(64)]
        public string Code { get; set; }

        public Guid? EntityId { get; set; } // ID đối tượng: Khách hàng || Nhà cung cấp

        [StringLength(64)]
        public string EntityCode { get; set; }

        [StringLength(256)]
        public string EntityName { get; set; }

        [StringLength(64)]
        public string EntityTypeCode { get; set; } // Mã loại đối tượng: value ---> customer || supplier

        [StringLength(128)]
        public string EntityTypeName { get; set; } // Tên loại đối tượng: value ---> Khách hàng || Nhà cung cấp

        public Guid? OriginalDocumentId { get; set; }

        [StringLength(64)]
        public string OriginalDocumentType { get; set; }

        [StringLength(64)]
        public string OriginalDocumentCode { get; set; }

        [StringLength(64)]
        public string TransactionTypeCode { get; set; }

        [StringLength(128)]
        public string TransactionTypeName { get; set; }

        public DateTime? TransactionDate { get; set; }

        [StringLength(64)]
        public string InventoryCode { get; set; } // Mã kho

        [StringLength(256)]
        public string InventoryName { get; set; } // Tên kho

        public Guid? ProjectId { get; set; }

        [StringLength(256)]
        public string ProjectName { get; set; }

        [StringLength(512)]
        public string Note { get; set; }
        
        [Range(0, 1000000)]
        public decimal TotalQuantity { get; set; }

        [StringLength(64)]
        public string TypeCode { get; set; } // Loại phiếu: value ---> InventoryImport, InventoryExport
        
        /// <summary>
        /// Id yêu cầu vật tư
        /// </summary>
        public Guid? MaterialRequestId { get; set; }
        
        /// <summary>
        /// Mã yêu cầu vật tư
        /// </summary>
        public string MaterialRequestCode  { get; set; }

        [StringLength(64)]
        public string StatusCode { get; set; } // Mã trạng thái: value ---> DRAFT, COMPLETED, CANCELLED

        [StringLength(128)]
        public string StatusName { get; set; } // Tên trạng thái: value ---> Nháp, Hoàn thành, Đã hủy

        public virtual ICollection<sm_InventoryNoteItem> InventoryNoteItems { get; set; }

        [ForeignKey("ProjectId")]
        public virtual mk_DuAn mk_DuAn { get; set; }

        public Guid? ContractId { get; set; }
        [ForeignKey("ContractId")]
        public virtual sm_Contract sm_Contract { get; set; }

        public Guid? ConstructionId { get; set; }
        [ForeignKey("ConstructionId")]
        public virtual sm_Construction sm_Construction { get; set; }
    }
}
