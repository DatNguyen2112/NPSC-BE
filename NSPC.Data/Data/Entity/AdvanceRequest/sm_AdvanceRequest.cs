using NSPC.Common;
using NSPC.Data.Data.Entity.JsonbEntity;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.AdvanceRequest
{
    [Table("sm_AdvanceRequest")]
    public class sm_AdvanceRequest : BaseTableService<sm_AdvanceRequest>
    {
        /// <value>ID</value>
        [Key]
        public Guid Id { get; set; }

        /// <value>Mã yêu cầu tạm ứng</value>
        public string Code { get; set; }

        /// <value>Nội dung</value>
        public string Content { get; set; }

        /// <value>Id công trình/dự án</value>
        public Guid ConstructionId { get; set; }
        [ForeignKey("ConstructionId")]
        public virtual sm_Construction sm_Construction { get; set; }

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

        public virtual ICollection<sm_AdvanceRequestItems> AdvanceRequestItems { get; set; } = new List<sm_AdvanceRequestItems>();

        /// <value>Tổng tiền AdvanceRequestItems</value>
        public decimal TotalLineAmount { get; set; } = 0M;

        /// <value>% Thuế</value>
        public decimal VatPercent { get; set; } = 0M;

        /// <value>Cần thanh toán</value>
        public decimal TotalAmount { get; set; } = 0M;

        /// <value>Kiểm tra quá hạn xử lý</value>
        [NotMapped]
        public bool IsOverdue => (DueDate < DateTime.Now && !string.IsNullOrEmpty(StatusCode) && StatusCode != AdvanceRequestConstants.StatusCode.COMPLETED)
            || (DueDate == DateTime.Now && StatusCode != AdvanceRequestConstants.StatusCode.COMPLETED)
            || (DueDate > DateTime.Now && (DueDate - DateTime.Now).Days <= 3 && StatusCode != AdvanceRequestConstants.StatusCode.COMPLETED);

        /// <value>Lịch sử xử lý</value>
        [Column(TypeName = "jsonb")]
        public List<jsonb_AdvanceRequestHistory> AdvanceRequestHistories { get; set; } = new List<jsonb_AdvanceRequestHistory>();
    }
}
