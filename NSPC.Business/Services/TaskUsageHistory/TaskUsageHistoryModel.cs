using NSPC.Common;
namespace NSPC.Business.Services.TaskUsageHistory
{
    public class TaskUsageHistoryViewModel
    {
        public Guid Id { get; set; }
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string NameSubtask { get; set; }
        //public TaskViewModel Task { get; set; }
    }
    public class TaskUsageHistoryQueryModel : PaginationRequest
    {

    }
}
