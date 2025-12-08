using NSPC.Data.Data.Entity.DuAn;
using NSPC.Data.Data.Entity.NhaCungCap;
//using NSPC.Data.Data.Entity.QuanLyKho;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Data.Data.Entity.Contract;

namespace NSPC.Data.Entity
{
    // Bảng DB Nhập hàng
    [Table("sm_PurchaseOrder")]
    public class sm_PurchaseOrder : BaseTableService<sm_PurchaseOrder>
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Mã đơn, sinh tự động
        /// </summary>
        [StringLength(64)]
        public string OrderCode { get; set; }

        /// <summary>
        /// Tham chiếu đơn
        /// </summary>
        [StringLength(128)]
        public string Reference { get; set; }

        /// <summary>
        /// Mã kho
        /// </summary>
        [StringLength(64)]
        public string WareCode { get; set; }

        [StringLength(64)]
        public string TypeCode { get; set; }// Mã phiếu
        [StringLength(512)]
        public string TypeName { get; set; }

        public string Address { get; set; }


        /// <summary>
        /// Lý do nhập hàng
        /// </summary>
        public string PurchaseReason { get; set; }


        /// <summary>
        /// Tỉ lệ chiết khấu của đơn
        /// </summary>
        public decimal DiscountPercent { get; set; } = 0M;

        /// <summary>
        /// Tiền chiết khấu của đơn
        /// </summary>
        public decimal DiscountAmount { get; set; } = 0M;

        /// <summary>
        /// Loại chiết khấu (percent/value) của đơn
        /// </summary>
        [StringLength(10)]
        public string DiscountType { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        /// <summary>
        /// Tổng SL SP
        /// </summary>
        public decimal TotalQuantity { get; set; } = 0;
        public decimal? VATAmount { get; set; }
        public decimal? TotalDiscountsAccounts { get; set; }
        public decimal? TotalTaxAccount { get; set; }
        public decimal? TotalOtherCost { get; set; }
        public decimal? PaidAmount { get; set; }

        public decimal? RemainingAmount { get; set; }
        public string ImportStatusCode { get; set; }
        public string PaymentStatusCode { get; set; }
        public string Note { get; set; }

        [Column(TypeName = "jsonb")]
        public List<jsonb_OtherCost> ListOtherCost { get; set; }

        [Column(TypeName = "jsonb")]
        public List<jsonb_HalfPayment> ListPayment { get; set; }

        public Guid SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public virtual sm_Supplier sm_Supplier { get; set; }
        
        /// <summary>
        /// Trạng thái đơn nhập: đã hoàn thành, đã hủy, đang giao dịch
        /// </summary>
        public string StatusCode { get; set; }
        
        /// <summary>
        /// Ngày hủy
        /// </summary>
        public DateTime? CancelledOnDate { get; set; }
        
        /// <summary>
        /// Lý do hủy
        /// </summary>
        [StringLength(128)]
        public string? CancelledReason { get; set; }
        
        /// <summary>
        /// Id yêu cầu vật tư
        /// </summary>
        public Guid? MaterialRequestId { get; set; }
        
        /// <summary>
        /// Mã yêu cầu vật tư
        /// </summary>
        public string MaterialRequestCode { get; set; }
        
        public string LyDoTuChoi { get; set; }
        public Guid? ActionMadeByUserId { get; set; }
        public string ActionMadeByUserName { get; set; }
        public DateTime? ActionMadeOnDate { get; set; }

        public virtual ICollection<sm_PurchaseOrderItem> Items { get; set; }
        
        /// <summary>
        /// Dự án
        /// </summary>
        public Guid? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual mk_DuAn mk_DuAn { get; set; }
        
        /// <summary>
        /// Công trình/Dự án
        /// </summary>
        public Guid? ConstructionId { get; set; }
        [ForeignKey("ConstructionId")]
        public virtual sm_Construction sm_Construction { get; set; }
        
        /// <summary>
        /// Hợp đồng/phụ lục
        /// </summary>
        public Guid? ContractId { get; set; }
        [ForeignKey("ContractId")]
        public virtual sm_Contract sm_Contract { get; set; }

        public bool IsReturned { get; set; } // Đã trả hết hay chưa ?
    }
}
