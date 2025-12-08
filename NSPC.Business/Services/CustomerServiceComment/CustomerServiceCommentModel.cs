using NSPC.Common;

namespace NSPC.Business.Services
{
    public class CustomerServiceCommentCreateModel
    {
        public List<string> TagIds { get; set; } = new List<string>();
        public string Content { get; set; }
        public Guid ConstructionId { get; set; }
    }

    public class CustomerServiceCommentViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid ConstructionId { get; set; }
        public List<string> TagIds { get; set; } = new List<string>();
        public string AvatarUrl  { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
        public DateTime CreatedOnDate { get; set; } = DateTime.Now;
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class CustomerServiceQueryModel : PaginationRequest
    {
        public Guid? ConstructionId { get; set; }
        public Guid? ParticipantId { get; set; }
        public string Content { get; set; }
    }
}

