using Newtonsoft.Json;
using NSPC.Business.Services.DuAn;
using NSPC.Business.Services.NhaCungCap;
//using NSPC.Business.Services.QuanLyPhieu;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data.Entity.NhaCungCap;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services.Contract;

namespace NSPC.Business.Services
{
    public class PurchaseOrderCreateUpdateModel
    {
        /// <summary>
        /// Tham chiếu đơn
        /// </summary>
        [StringLength(128)]
        public string Reference { get; set; }
        
        /// <summary>
        /// Mã phiếu
        /// </summary>
        [StringLength(64)]
        public string OrderCode { get; set; }

        /// <summary>
        /// Mã kho
        /// </summary>
        [StringLength(64)]
        public string WareCode { get; set; }

        public string Address { get; set; }

        /// <summary>
        /// Lý do nhập hàng
        /// </summary>
        public string PurchaseReason { get; set; }

        /// <summary>
        /// Tỉ lệ chiết khấu
        /// </summary>
        public decimal DiscountPercent { get; set; } = 0M;

        /// <summary>
        /// Tiền chiết khấu
        /// </summary>
        public decimal DiscountAmount { get; set; } = 0M;

        /// <summary>
        /// Loại chiết khấu (percent/value)
        /// </summary>
        [StringLength(10)]
        public string DiscountType { get; set; }

        /// <summary>
        /// Tags
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Mã loại thanh toán dự kiến
        /// </summary>
        public string PaymentMethodCode { get; set; }


        /// <summary>
        /// Danh sách chi phí khác
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_OtherCost> ListOtherCost { get; set; }

        /// <summary>
        /// Danh sách thanh toán
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_HalfPayment> ListPayment { get; set; }

        /// <summary>
        /// Id của Supplier
        /// </summary>
        public Guid SupplierId { get; set; }
        
        /// <summary>
        /// Id dự án
        /// </summary>
        public Guid? ProjectId { get; set; }
        
        /// <summary>
        /// Id công trình/dự án
        /// </summary>
        public Guid? ConstructionId  { get; set; }
        
        /// <summary>
        /// Id Yêu cầu vật tư
        /// </summary>
        public Guid? MaterialRequestId  { get; set; }
        
        /// <summary>
        /// Id hợp đồng/phụ lục
        /// </summary>
        public Guid? ContractId  { get; set; }

        /// <summary>
        /// Trạng thái đơn nhập: đã hoàn thành, đã hủy, đang giao dịch
        /// </summary>
        public string StatusCode { get; set; }

        public string LyDoTuChoi { get; set; }
        public Guid? ActionMadeByUserId { get; set; }
        public string ActionMadeByUserName { get; set; }
        public DateTime? ActionMadeOnDate { get; set; }

        public List<PurchaseOrderItemCreateUpdateModel> Items { get; set; }
    }

    public class PurchaseOrderViewModel
    {
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

        /// <summary>
        /// Đối tượng kho
        /// </summary>
        public CodeTypeListModel Ware { get; set; }

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

        /// <summary>
        /// Tổng tiền của các sản phẩm sau chiết khấu sản phẩm
        /// </summary>
        public decimal? SubTotal { get; set; }

        /// <summary>
        /// Tổng thành tiền khách phải trả
        /// </summary>
        public decimal Total { get; set; }
        
        /// <summary>
        /// Tổng SL SP
        /// </summary>
        public decimal TotalQuantity { get; set; } = 0;

        /// <summary>
        /// Số tiền thuế VAT
        /// </summary>
        public decimal? VATAmount { get; set; }

        public decimal? TotalDiscountsAccounts { get; set; }
        public decimal? TotalTaxAccount { get; set; }

        /// <summary>
        /// Tổng tiền chi phí khác
        /// </summary>
        public decimal? TotalOtherCost { get; set; }

        /// <summary>
        /// Tiền khách đã trả
        /// </summary>
        public decimal? PaidAmount { get; set; }

        /// <summary>
        /// Tiền còn lại cần thu
        /// </summary>
        public decimal? RemainingAmount { get; set; }

        /// <summary>
        /// Mã trạng thái nhập kho
        /// </summary>
        public string ImportStatusCode { get; set; }

        /// <summary>
        /// Mã trạng thái thanh toán
        /// </summary>
        public string PaymentStatusCode { get; set; }

        /// <summary>
        /// Ghi chú cho đơn
        /// </summary>
        public string Note { get; set; }


        /// <summary>
        /// Danh sách chi phí khác
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_OtherCost> ListOtherCost { get; set; }

        /// <summary>
        /// Danh sách thanh toán
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_HalfPayment> ListPayment { get; set; }

        /// <summary>
        /// Supplier ID
        /// </summary>
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Supplier
        /// </summary>
        public NhaCungCapViewModel Supplier { get; set; }

        /// <summary>
        /// Trạng thái đơn nhập: đã hoàn thành, đã hủy, đang giao dịch
        /// </summary>
        public string StatusCode { get; set; }

        public string LyDoTuChoi { get; set; }
        public Guid? ActionMadeByUserId { get; set; }
        public string ActionMadeByUserName { get; set; }
        public DateTime? ActionMadeOnDate { get; set; }


        public List<PurchaseOrderItemViewModel> Items { get; set; }

        public bool IsReturned { get; set; }
        
        /// <summary>
        /// Id công trình
        /// </summary>
        public Guid? ConstructionId  { get; set; }
        
        /// <summary>
        /// Id Yêu cầu vật tư
        /// </summary>
        public Guid? MaterialRequestId  { get; set; }
        
        /// <summary>
        /// Mã yêu cầu vật tư
        /// </summary>
        public string MaterialRequestCode  { get; set; }
        
        /// <summary>
        /// Thông tin dự án
        /// </summary>
        public ConstructionViewModel Construction { get; set; }
        
        /// <summary>
        /// Dự án ID
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// Thông tin dự án
        /// </summary>
        public DuAnViewModel Project { get; set; }
        
        /// <summary>
        /// Id công trình
        /// </summary>
        public Guid? ContractId  { get; set; }
        
        /// <summary>
        /// Thông tin dự án
        /// </summary>
        public ContractViewModel Contract { get; set; }

        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }


        public string[] AllowedActions { get; set; }
    }

    public class PurchaseOrderQueryModel : PaginationRequest
    {
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public string CreatedIn { get; set; }
        public string WareCode { get; set; }
        public Guid? ProjectId { get; set; }
        public string OrderCode { get; set; }
        public string TotalAmount { get; set; }
        public string StatusCode { get; set; }
        public string ImportStatusCode { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime?[] DateRange { get; set; }
        public List<string> PaymentStatusCodes { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid? ConstructionId { get; set; }
    }
}
