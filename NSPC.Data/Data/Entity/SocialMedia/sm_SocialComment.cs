using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    /// <value>Bảng comment mạng xã hội</value>
    [Table("sm_SocialComment")]
    public class sm_SocialComment : BaseSocialEntity<sm_SocialComment>
    {
        /// <value>ID bài viết</value>
        [Required]
        public Guid PostId { get; set; }

        /// <value>ID comment cha (nếu là comment trả lời)</value>
        public Guid? ParentCommentId { get; set; }

        /// <value>Foreign key tới bài viết</value>
        [ForeignKey(nameof(PostId))]
        public sm_SocialPost Post { get; set; }

        /// <value>Foreign key tới comment cha</value>
        [ForeignKey(nameof(ParentCommentId))]
        public sm_SocialComment ParentComment { get; set; }

        /// <value>Danh sách comment con (replies)</value>
        public virtual ICollection<sm_SocialComment> ChildComments { get; set; } = new List<sm_SocialComment>();
    }
}
