using NSPC.Common;
using NSPC.Data;
using System.Text.Json.Serialization;

namespace NSPC.Business
{
    public class TaskManagementViewModel 
    {
        public Guid Id { get; set; }
        //public Guid? ParentId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Guid? ConstructionId { get; set; }
        public string Status { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
        public List<TaskManagementAssigneeViewModel> Assignees { get; set; }
        public List<TaskManagementCommentViewModel> Comments { get; set; }
        public List<TaskManagementHistoryViewModel> Histories { get; set; }
        public List<TaskManagementMileStoneViewModel> MileStones { get; set; }
    }

    public class TaskSummanryModel
    {
        public int TotalTask { get; set; }
        public int TotalInprogressTask { get; set; }
        public int TotalFinishedTask { get; set; }
        public int TotalPausedTask { get; set; }
        public int TotalDraftTask { get; set; }
    }
    public class TaskManagementCreateUpdateModel 
    {
        //public Guid? ParentId { get; set; }
        public List<Guid> UserIds { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Guid? ConstructionId { get; set; }
        public string Status { get; set; }  
        public List<jsonb_Attachment> Attachments { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }       
    }

    public class TaskManagementQueryModel : PaginationRequest
    {
        public string Title { get; set; }
        public string Status { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ConstructionId { get; set; }
    }

    public class TaskManagementAssigneeViewModel
    {
        public Guid Id { get; set; }
        public Guid TaskManagementId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class TaskManagementAssigneeCreateUpdateModel
    {
        public Guid TaskManagementId { get; set; }
        public Guid UserId { get; set; }       
    }

    public class TaskManagementAssigneeQueryModel : PaginationRequest
    {
        public Guid? TaskManagementId { get; set; }
    }

    public class TaskManagementCommentViewModel
    {
        public Guid Id { get; set; }
        public Guid TaskManagementId { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class TaskManagementCommentCreateUpdateModel
    {
        public Guid TaskManagementId { get; set; }
        public string Content { get; set; }
    }       

    public class TaskManagementCommentQueryModel : PaginationRequest
    {
        public Guid? TaskManagementId { get; set; }
    }

    public class TaskManagementHistoryViewModel
    {
        public Guid Id { get; set; }
        public Guid TaskManagementId { get; set; }
        public Guid? TaskManagementCommentReplyId { get; set; }
        public string Action { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class TaskManagementHistoryCreateUpdateModel
    {
        public Guid TaskManagementId { get; set; }
        public Guid? TaskManagementCommentReplyId { get; set; }
        public string Action { get; set; }       
    }

    public class TaskManagementHistoryQueryModel : PaginationRequest
    {
        public Guid? TaskManagementId { get; set; }
    }

    public class TaskManagementMileStoneViewModel
    {
        public Guid Id { get; set; }
        public Guid TaskManagementId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class TaskManagementMileStoneCreateUpdateModel
    {
        public Guid TaskManagementId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class TaskManagementMileStoneQueryModel : PaginationRequest
    {

    }


}
