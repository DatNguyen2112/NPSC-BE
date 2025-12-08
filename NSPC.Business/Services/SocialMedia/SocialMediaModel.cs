using NSPC.Data;
using NSPC.Data.Entity;
using NSPC.Common;

namespace NSPC.Business.Services
{
    #region Social Post Models

    public class SocialPostCreateUpdateModel
    {
        /// <value>Nội dung bài viết</value>
        public string Content { get; set; }

        /// <value>Danh sách tệp đính kèm</value>
        public List<jsonb_Attachment> Attachments { get; set; } = new();
    }

    public class SocialPostViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; } = new();
        public List<SocialReactionViewModel> Reactions { get; set; } = new();
        public List<SocialCommentViewModel> Comments { get; set; } = new();
        public BaseUserModel CreatedByUser { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class SocialPostDetailViewModel : SocialPostViewModel
    {
        // Có thể thêm thông tin chi tiết khác nếu cần
    }

    public class SocialPostQueryModel
    {
        public int Skip { get; set; } = 0;
        public int Size { get; set; } = 20;
        public string Sort { get; set; } = "-CreatedOnDate";
    }

    public class SocialPostListResponse
    {
        public List<SocialPostViewModel> Posts { get; set; } = new();
        public int TotalPosts { get; set; }
        public bool HasMore { get; set; }
    }

    #endregion

    #region Social Comment Models

    public class SocialCommentCreateUpdateModel
    {
        /// <value>Nội dung comment</value>
        public string Content { get; set; }

        /// <value>ID bài viết (bắt buộc khi tạo comment cho post)</value>
        public Guid? PostId { get; set; }

        /// <value>ID comment cha (bắt buộc khi tạo comment trả lời)</value>
        public Guid? ParentCommentId { get; set; }

        /// <value>Danh sách tệp đính kèm</value>
        public List<jsonb_Attachment> Attachments { get; set; } = new();
    }

    public class SocialCommentViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid PostId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; } = new();
        public List<SocialReactionViewModel> Reactions { get; set; } = new();
        public List<SocialCommentViewModel> ChildComments { get; set; } = new();
        public BaseUserModel CreatedByUser { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    #endregion

    #region Reaction Models

    public class SocialReactionViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Reaction { get; set; }
        public DateTime ReactedDate { get; set; }
    }

    public class UpdateReactionModel
    {
        /// <value>ID của post hoặc comment</value>
        public Guid TargetId { get; set; }

        /// <value>true = post, false = comment</value>
        public bool IsPost { get; set; }

        /// <value>Loại reaction: Like, Haha, Love, Wow, Sad, Angry</value>
        public string Reaction { get; set; }

        /// <value>true = add/update, false = remove</value>
        public bool IsAdd { get; set; }
    }

    #endregion
}