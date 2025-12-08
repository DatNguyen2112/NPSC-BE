using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    /// <value>Bảng bài viết mạng xã hội</value>
    [Table("sm_SocialPost")]
    public class sm_SocialPost : BaseSocialEntity<sm_SocialPost>
    {
        /// <value>Danh sách comment của bài viết</value>
        public virtual ICollection<sm_SocialComment> Comments { get; set; } = new List<sm_SocialComment>();
    }
}