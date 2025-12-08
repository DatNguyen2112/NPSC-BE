using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.AdvanceRequest
{
    [Table("sm_AdvanceRequestItems")]
    public class sm_AdvanceRequestItems : BaseTableService<sm_AdvanceRequestItems>
    {
        [Key]
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
}
