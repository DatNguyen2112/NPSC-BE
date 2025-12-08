using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.JsonbEntity
{
    public class jsonb_AdvanceRequestHistory
    {
        /// <value>Id lịch sử</value>
        public Guid Id { get; set; }

        /// <value>Title thực hiện</value>
        public string Action { get; set; }

        /// <value>Lý do từ chối</value>
        public string RejectionReason { get; set; }

        /// <value>Ngày tạo</value>
        public DateTime CreatedOnDate { get; set; }

        /// <value>Người tạo</value>
        public string CreatedByUserName { get; set; }
    }
}
