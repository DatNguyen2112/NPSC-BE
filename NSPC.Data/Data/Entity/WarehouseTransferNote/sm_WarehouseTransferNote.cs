using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    [Table("sm_WarehouseTransferNote")]
    public class sm_WarehouseTransferNote : BaseTableService<sm_WarehouseTransferNote>
    {
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Mã phiếu chuyển kho
        /// </summary>
        [StringLength(64)]
        public string TransferNoteCode { get; set; }
        
        /// <summary>
        /// Mã kho xuất
        /// </summary>
        [StringLength(64)]
        public string ExportWarehouseCode { get; set; }
        
        /// <summary>
        /// Tên kho xuất
        /// </summary>
        public string ExportWarehouseName { get; set; }
        
        /// <summary>
        /// Mã kho nhập
        /// </summary>
        [StringLength(64)]
        public string ImportWarehouseCode { get; set; }
        
        /// <summary>
        /// Tên kho nhập
        /// </summary>
        public string ImportWarehouseName { get; set; }
        
        /// <summary>
        /// Ngày dự kiên điều chuyển
        /// </summary>
        public DateTime? TransferredOnDate { get; set; }
        
        /// <summary>
        /// Người điều chuyển
        /// </summary>
        public string TransferredByUserName { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string StatusCode { get; set; }
        
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        
        /// <summary>
        /// Danh sách sản phẩm điều chuyển
        /// </summary>
        public virtual ICollection<sm_WarehouseTransferNoteItem> Items { get; set; }
    }
}