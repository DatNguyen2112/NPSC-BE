using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Data.Data.Entity.Contract;
using NSPC.Data.Data.Entity.DuAn;

namespace NSPC.Data.Entity
{
    // Bảng DB Trả hàng NCC / Khách trả hàng
    [Table("sm_Return_Order")]
    public class sm_Return_Order : BaseTableService<sm_Return_Order>
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Mã đơn, sinh tự động
        /// </summary>
        [StringLength(64)]
        public string OrderCode { get; set; }

        /// <summary>
        /// Mã kho
        /// </summary>
        [StringLength(64)]
        public string WareCode { get; set; }

        /// <summary>
        /// Lý don trả hàng
        /// </summary>
        [StringLength(128)]
        public string ReasonCode { get; set; }

        /// <summary>
        /// Id khách hàng hoặc nhà cung cấp
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Mã khách hàng hoặc mã nhà cung cấp
        /// </summary
        [StringLength(64)]
        public string EntityCode { get; set; }

        /// <summary>
        /// Tên khách hàng hoặc tên nhà cung cấp
        /// </summary
        [StringLength(128)]
        public string EntityName { get; set; }

        /// <summary>
        /// Mã Loại người dùng (customer hoặc supplier)
        /// </summary
        [StringLength(64)]
        public string EntityTypeCode { get; set; }

        /// <summary>
        /// Tên Loại người dùng (khách hàng hoặc nhà cung cấp)
        /// </summary
        [StringLength(128)]
        public string EntityTypeName { get; set; }

        /// <summary>
        /// Id giấy chưng từ gốc (id của đơn hàng hoặc bán hàng)
        /// </summary
        public Guid OriginalDocumentId { get; set; }

        /// <summary>
        /// Mã giấy chưng từ gốc (Mã đơn hàng hoặc bán hàng)
        /// </summary
        [StringLength(64)]
        public string OriginalDocumentCode { get; set; }

        /// <summary>
        /// Lịch sử thanh toán
        /// </summary
        [Column(TypeName = "jsonb")]
        public List<jsonb_HalfPayment> ListPayment { get; set; }

        /// <summary>
        /// Tổng tiền cần hoàn trả cho cho khách / NCC
        /// </summary
        public decimal RefundSubTotal { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary
        public decimal? PaidAmount { get; set; }

        /// <summary>
        /// Tổng tiền còn lại sau khi thanh toán
        /// </summary
        public decimal? RemainingRefundAmount { get; set; }

        /// <summary>
        /// Danh sách sản phẩm cần trả
        /// </summary
        public virtual ICollection<sm_Return_Order_Item> OrderItems { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary
        [StringLength(128)]
        public string Note { get; set; }

        /// <summary>
        /// Trạng thái đơn hàng
        /// </summary
        public string StatusCode { get; set; }

        /// <summary>
        /// Trạng thái hoàn tiền
        /// </summary
        public string RefundStatusCode { get; set; }

        /// <summary>
        /// Ngày hủy đơn
        /// </summary
        public string CancelledOnDate { get; set; }
        
        /// <summary>
        /// Dự án
        /// </summary>
        public Guid? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual mk_DuAn mk_DuAn { get; set; }
        
        /// <summary>
        /// Id Công trình
        /// </summary>
        public Guid? ConstructionId  { get; set; }
        [ForeignKey("ConstructionId")]
        public sm_Construction sm_Construction { get; set; }
        
        /// <summary>
        /// Id Hợp đồng
        /// </summary>
        public Guid? ContractId  { get; set; }
        [ForeignKey("ContractId")]
        public sm_Contract sm_Contract { get; set; }
    }
}
