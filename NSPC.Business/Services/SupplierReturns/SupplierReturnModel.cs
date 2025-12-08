using NSPC.Common;
using NSPC.Data.Entity;
using NSPC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services.DuAn;

namespace NSPC.Business.Services
{
    public class SupplierReturnCreateUpdateModel
    {
        /// <summary>
        /// Mã đơn, sinh tự động
        /// </summary>
        [StringLength(64)]
        public string OrderCode { get; set; }

        /// <summary>
        /// Lý don trả hàng
        /// </summary>
        [StringLength(128)]
        public string ReasonCode { get; set; }

        /// <summary>
        /// Mã kho
        /// </summary>
        [StringLength(64)]
        public string WareCode { get; set; }

        /// <summary>
        /// Id khách hàng hoặc nhà cung cấp
        /// </summary>
        public Guid EntityId { get; set; }
        
        /// <summary>
        /// Id dự án
        /// </summary>
        public Guid? ProjectId { get; set; }
        
        /// <summary>
        /// Id công trình
        /// </summary>
        public Guid? ConstructionId { get; set; }
        
        /// <summary>
        /// Id hợp đồng
        /// </summary>
        public Guid? ContractId { get; set; }

        /// <summary>
        /// Mã khách hàng hoặc mã nhà cung cấp
        /// </summary
        [StringLength(64)]
        public string EntityCode { get; set; }

        /// <summary>
        /// Tên khách hàng hoặc tên nhà cung cấp
        /// </summary
        [StringLength(128)]
        public string? EntityName { get; set; }

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
        public decimal RemainingRefundAmount { get; set; }

        /// <summary>
        /// Danh sách sản phẩm cần trả
        /// </summary
        public List<SupplierReturnItemCreateUpdateModel> OrderItems { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary
        [StringLength(128)]
        public string Note { get; set; }
    }

    public class SupplierReturnViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mã đơn, sinh tự động
        /// </summary>
        [StringLength(64)]
        public string OrderCode { get; set; }

        /// <summary>
        /// Lý don trả hàng
        /// </summary>
        [StringLength(128)]
        public string ReasonCode { get; set; }

        /// <summary>
        /// Mã kho
        /// </summary>
        [StringLength(64)]
        public CodeTypeListModel Ware { get; set; }
        
        /// <summary>
        /// Id dự án
        /// </summary>
        public Guid? ProjectId { get; set; }
        
        /// <summary>
        /// Id công trình
        /// </summary>
        public Guid? ConstructionId { get; set; }
        
        public string ConstructionName  { get; set; }
        
        /// <summary>
        /// Id hợp đồng
        /// </summary>
        public Guid? ContractId { get; set; }
        
        public string ContractName { get; set; }
        
        /// <summary>
        /// Dự án
        /// </summary>
        public DuAnViewModel Project { get; set; }

        /// <summary>
        /// Lý do trả hàng
        /// </summary>
        [StringLength(128)]
        public CodeTypeListModel Reason { get; set; }

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
        public string? EntityName { get; set; }

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
        public decimal RemainingRefundAmount { get; set; }

        /// <summary>
        /// Danh sách sản phẩm cần trả
        /// </summary
        public List<SupplierReturnItemViewModel> OrderItems { get; set; }

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

        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class RefundSupplierCreateModel
    {
        /// <summary>
        /// Lịch sử thanh toán
        /// </summary
        [Column(TypeName = "jsonb")]
        public List<jsonb_HalfPayment> ListPayment { get; set; }
    }

    public class SupplierReturnQueryModel : PaginationRequest
    {
        public string OrderCode { get; set; }
        public string OriginalDocumentCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPhone { get; set; }
        public DateTime?[] DateRange { get; set; }
        public string ReasonCode { get; set; }
        public string EntityTypeCode { get; set; }
        public Guid? EntityId { get; set; }
    }

}
