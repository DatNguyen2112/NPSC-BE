using NSPC.Common;
using NSPC.Data.Data.Entity.AdvanceRequest;
using NSPC.Data.Data.Entity.JsonbEntity;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.AdvanceRequest
{
    public class AdvanceRequestViewModel
    {
        /// <value>ID</value>
        public Guid Id { get; set; }

        /// <value>Mã yêu cầu tạm ứng</value>
        public string Code { get; set; }

        /// <value>Nội dung</value>
        public string Content { get; set; }

        /// <value>Id công trình/dự án</value>
        public Guid ConstructionId { get; set; }

        public ConstructionViewModel Construction { get; set; }

        /// <value>Mã độ ưu tiên</value>
        public string PriorityLevelCode { get; set; }

        /// <value>Tên độ ưu tiên</value>
        public string PriorityLevelName { get; set; }

        /// <value>Màu độ ưu tiên</value>
        public string PriorityLevelColor { get; set; }

        /// <value>Hạn xử lý</value>
        public DateTime DueDate { get; set; }

        /// <value>Ghi chú</value>
        public string Note { get; set; }

        /// <value>Mã trạng thái</value>
        public string StatusCode { get; set; }

        /// <value>Tên trạng thái</value>
        public string StatusName { get; set; }

        /// <value>Màu trạng thái</value>
        public string StatusColor { get; set; }

        public List<AdvanceRequestItemsViewModel> AdvanceRequestItems { get; set; } = new List<AdvanceRequestItemsViewModel>();

        /// <value>Tổng tiền AdvanceRequestItems</value>
        public decimal TotalLineAmount { get; set; } = 0M;

        /// <value>% Thuế</value>
        public decimal VatPercent { get; set; } = 0M;

        /// <value>Cần thanh toán</value>
        public decimal TotalAmount { get; set; } = 0M;

        /// <value>Kiểm tra quá hạn xử lý</value>
        public bool IsOverdue { get; set; }

        /// <value>Lịch sử xử lý</value>
        [Column(TypeName = "jsonb")]
        public List<jsonb_AdvanceRequestHistory> AdvanceRequestHistories { get; set; } = new List<jsonb_AdvanceRequestHistory>();

        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class AdvanceRequestCreateUpdateModel
    {
        ///// <value>ID</value>
        //public Guid Id { get; set; }

        ///// <value>Mã yêu cầu tạm ứng</value>
        //public string Code { get; set; }

        /// <value>Nội dung</value>
        public string Content { get; set; }

        /// <value>Id công trình/dự án</value>
        public Guid ConstructionId { get; set; }

        //public sm_Construction sm_Construction { get; set; }

        /// <value>Mã độ ưu tiên</value>
        public string PriorityLevelCode { get; set; }

        ///// <value>Tên độ ưu tiên</value>
        //public string PriorityLevelName { get; set; }

        ///// <value>Màu độ ưu tiên</value>
        //public string PriorityLevelColor { get; set; }

        /// <value>Hạn xử lý</value>
        public DateTime DueDate { get; set; }

        /// <value>Ghi chú</value>
        public string Note { get; set; }

        /// <value>Mã trạng thái</value>
        //public string StatusCode { get; set; }

        ///// <value>Tên trạng thái</value>
        //public string StatusName { get; set; }

        ///// <value>Màu trạng thái</value>
        //public string StatusColor { get; set; }

        public List<AdvanceRequestItemsCreateUpdateModel> AdvanceRequestItems { get; set; } = new List<AdvanceRequestItemsCreateUpdateModel>();

        ///// <value>Tổng tiền AdvanceRequestItems</value>
        //public decimal TotalLineAmount { get; set; } = 0M;

        /// <value>% Thuế</value>
        public decimal VatPercent { get; set; } = 0M;

        ///// <value>Cần thanh toán</value>
        //public decimal TotalAmount { get; set; } = 0M;

        ///// <value>Kiểm tra quá hạn xử lý</value>
        //[NotMapped]
        //public bool IsOverdue => DueDate < DateTime.Now && !string.IsNullOrEmpty(StatusCode) && StatusCode != "COMPLETED";

        ///// <value>Lịch sử xử lý</value>
        //[Column(TypeName = "jsonb")]
        //public List<jsonb_AdvanceRequestHistory> AdvanceRequestHistories { get; set; } = new List<jsonb_AdvanceRequestHistory>();
    }

    /////////////////////// AdvanceRequestItems ///////////////////////
    public class AdvanceRequestItemsViewModel
    {
        public Guid Id { get; set; }

        /// <value>Số thứ tự</value>
        public int LineNumber { get; set; }

        /// <value>Mục tạm ứng</value>
        public string AdvancePurpose { get; set; }

        /// <value>Đơn vị</value>
        public string Unit { get; set; }

        /// <value>Số lượng</value>
        public decimal Quantity { get; set; } = 0M;

        /// <value>Đơn giá</value>
        public decimal UnitPrice { get; set; } = 0M;

        /// <value>Thành tiền</value>
        public decimal LineAmount { get; set; } = 0M;

        /// <value>Ghi chú</value>
        public string Note { get; set; }
    }

    public class AdvanceRequestItemsCreateUpdateModel
    {
        //public Guid Id { get; set; }

        /// <value>Số thứ tự</value>
        public int LineNumber { get; set; }

        /// <value>Mục tạm ứng</value>
        public string AdvancePurpose { get; set; }

        /// <value>Đơn vị</value>
        public string Unit { get; set; }

        /// <value>Số lượng</value>
        public decimal Quantity { get; set; } = 0M;

        /// <value>Đơn giá</value>
        public decimal UnitPrice { get; set; } = 0M;

        ///// <value>Thành tiền</value>
        //public decimal LineAmount { get; set; } = 0M;

        /// <value>Ghi chú</value>
        public string Note { get; set; }
    }

    public class AdvanceRequestQueryModel : PaginationRequest
    {
        /// <value>Id công trình/dự án</value>
        public Guid? ConstructionId { get; set; }

        /// <value>Hạn xử lý</value>
        public DateTime? DueDate { get; set; }

        /// <value>Ngày tạo</value>
        public DateTime? CreatedOnDate { get; set; }

        /// <value>Mã trạng thái</value>
        public string StatusCode { get; set; }

        /// <value>Mã độ ưu tiên</value>
        public string PriorityLevelCode { get; set; }

        /// <value>Cần thanh toán</value>
        public decimal? TotalAmount { get; set; }
        
        public DateTime?[] DateRange { get; set; }
    }
}
