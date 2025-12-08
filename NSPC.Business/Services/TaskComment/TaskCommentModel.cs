using NSPC.Common;

namespace NSPC.Business.Services
{
    public class TaskCommentCreateModel
    {
        public List<string> TagIds { get; set; } = new List<string>();
        public string Content { get; set; }
        public Guid TaskId { get; set; }
    }

    public class TaskCommentViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid TaskId { get; set; }
        public List<string> TagIds { get; set; } = new List<string>();
        public string AvatarUrl { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
        public DateTime CreatedOnDate { get; set; } = DateTime.Now;
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class TaskQueryModel : PaginationRequest
    {
        public Guid? TaskId { get; set; }
        public Guid? ParticipantId { get; set; }
        public string Content { get; set; }
    }
}

