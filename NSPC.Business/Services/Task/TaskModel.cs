using NSPC.Business.Services.TaskUsageHistory;
using NSPC.Common;
using NSPC.Data;
namespace NSPC.Business.Services.WorkItem
{
    public class TaskViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Description { get; set; }
        public string PriorityLevel { get; set; }
        public int PriorityOrder { get; set; }
        public string? Status { get; set; }
        public List<SubTaskViewModel> SubTasks { get; set; }
        public List<TaskApproverViewModel> Approvers { get; set; }
        public List<TaskExecutorViewModel> Executors { get; set; }
        public ConstructionViewModel Construction { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public List<TaskUsageHistoryViewModel> TaskUsageHistories { get; set; }
        public Guid? ConstructionId { get; set; }
    }
    public class TaskCreateUpdateModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Description { get; set; }
        public string? PriorityLevel { get; set; }
        public int? PriorityOrder { get; set; }
        public Guid? ConstructionId { get; set; }
        public Guid? IdTemplateStage { get; set; }
        public List<Guid> ApproverIds { get; set; }
        public List<Guid> ExecutorIds { get; set; }
        public List<jsonb_Attachment> Attachments { get; set; }
        public List<SubTaskCreateUpdateModel> SubTasks { get; set; }
    }
    public class TaskQueryModel : PaginationRequest
    {
        public Guid ConstructionId { get; set; }
        public Guid IdTemplateStage { get; set; }
        public string Status { get; set; }
        public string PriorityLevel { get; set; }
        public DateTime?[] DueDateRange { get; set; }
        public Guid UserId { get; set; }
        public Guid[] UserIdList { get; set; }
        public int Year { get; set; }
    }
    public class SubTaskCreateUpdateModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public List<jsonb_Attachment> Attachments { get; set; }
        public Guid TaskId { get; set; }
        public List<Guid> ExecutorIds { get; set; }
    }
    public class SubTaskViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public List<SubTaskExecutorViewModel> SubTaskExecutors { get; set; }
    }
    public class TaskExecutorViewModel
    {
        public Guid Id { get; set; }
        public UserModel Idm_User { get; set; }
    }
    public class TaskApproverViewModel
    {
        public Guid Id { get; set; }
        public UserModel Idm_User { get; set; }
    }
    public class SubTaskExecutorViewModel
    {
        public Guid Id { get; set; }
        public UserModel Idm_User { get; set; }
    }
    public class RejectTaskModel
    {
        public string Description { get; set; }
    }

    /// <summary>
    /// Model cập nhật trạng thái nhiều công việc
    /// </summary>
    public class UpdateStatusManyModel
    {
        public List<Guid> Ids { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }

    public class TaskStatusSummaryViewModel
    {
        public int Total { get; set; }
        public int NotStarted { get; set; }
        public int InProgress { get; set; }
        public int PendingApproval { get; set; }
        public int Failed { get; set; }
        public int Passed { get; set; }
    }

    public class TaskOverviewSummaryViewModel
    {
        public int TotalTasks { get; set; } // Tổng số công việc
        public int CompletedTasks { get; set; } // Số công việc đã hoàn thành
        public double CompletedTasksPercent { get; set; } // Phần trăm công việc đã hoàn thành
        public int OverdueTasks { get; set; } // Số công việc quá hạn
        public double OverdueTasksPercent { get; set; } // Phần trăm công việc quá hạn
        public int FailedTasks { get; set; } // Số công việc không đạt
        public double FailedTasksPercent { get; set; } // Phần trăm công việc không đạt
        public int InProgressTasks { get; set; } // Số công việc đang thực hiện
        public double InProgressTasksPercent { get; set; } // Phần trăm công việc đang thực hiện
        public int NotStartedTasks { get; set; } // Số công việc chưa bắt đầu
        public double NotStartedTasksPercent { get; set; } // Phần trăm công việc chưa bắt đầu
        public int TotalProjects { get; set; } // Tổng số dự án
        public int ProjectsDesigning { get; set; } // Số dự án đang thiết kế
        public double ProjectsDesigningPercent { get; set; } // Phần trăm dự án đang thiết kế
        public int ProjectsAuthorSupervisor { get; set; } // Số dự án giám sát tác giả
        public double ProjectsAuthorSupervisorPercent { get; set; } // Phần trăm dự án giám sát tác giả
    }

    public class TaskOverviewEachStage
    {
        public double PercentProcess { get; set; } = 0;
        public int TotalTask { get; set; } = 0;
        public int TotalDoneTask { get; set; } = 0;
        public int TotalLateTask { get; set; } = 0;
    }
}
