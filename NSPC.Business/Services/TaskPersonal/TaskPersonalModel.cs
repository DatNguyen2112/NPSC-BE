using NSPC.Business.Services.TaskUsageHistory;
using NSPC.Common;
using NSPC.Data;

namespace NSPC.Business.Services
{
    public class TaskPersonalViewModel
    {
        public Guid Id { get; set; }
        
        public string Code { get; set; }
        
        /// <value>Tên công việc</value>
        public string Name { get; set; }
        
        /// <value>Ngày bắt đầu thực hiện</value>
        public DateTime? StartDateTime { get; set; }

        /// <value>Ngày kết thúc thực hiện</value>
        public DateTime? EndDateTime { get; set; }
        
        /// <value>Loại công việc</value>
        public string TaskType { get; set; }
        
        public CodeTypeListModel TaskTypeModel { get; set; }
        
        /// <value>Độ ưu tiên</value>
        public string PriorityLevel { get; set; }
        
        public string? Status { get; set; }
        
        /// <value>Ghi chú</value>
        public string Note { get; set; }
        
        public List<SubTaskPersonalViewModel> SubTaskPersonals { get; set; }
    }

    public class TaskPersonalCreateModel
    {
        /// <value>Tên công việc</value>
        public string Name { get; set; }
        
        /// <value>Ngày bắt đầu thực hiện</value>
        public DateTime? StartDateTime { get; set; }

        /// <value>Ngày kết thúc thực hiện</value>
        public DateTime? EndDateTime { get; set; }
        
        /// <value>Loại công việc</value>
        public string TaskType { get; set; }
        
        /// <value>Độ ưu tiên</value>
        public string? PriorityLevel { get; set; }
        
        /// <value>Ghi chú</value>
        public string Note { get; set; }
        
        public List<SubTaskPersonalCreateModel> SubTaskPersonals { get; set; }
    }

    public class SubTaskPersonalCreateModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public int LineNo { get; set; }

        /// <value>Đã hoàn thành/ Chưa hoàn thành</value>
        public bool IsCompleted { get; set; } = false;

        /// <value>Ngày hết hạn</value>
        public DateTime? DueDate { get; set; }
    }

    public class SubTaskPersonalViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public int LineNo { get; set; }

        /// <value>Đã hoàn thành/ Chưa hoàn thành</value>
        public bool IsCompleted { get; set; }

        /// <value>Ngày hết hạn</value>
        public DateTime? DueDate { get; set; }
    }

    public class TaskPersonalQueryModel : PaginationRequest
    {
        public string PriorityLevel { get; set; }
        public DateTime?[] DueDateRange { get; set; }
        public string TaskType { get; set; }
        public string Status { get; set; }
    }
}

