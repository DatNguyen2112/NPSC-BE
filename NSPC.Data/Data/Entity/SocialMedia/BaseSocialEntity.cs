using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    /// <summary>
    /// Enum cho các loại reaction
    /// </summary>
    public enum SocialReactionType
    {
        Like,
        Haha,
        Love,
        Wow,
        Sad,
        Angry
    }

    /// <summary>
    /// Class cho reaction object trong jsonb
    /// </summary>
    public class jsonb_SocialReaction
    {
        /// <summary>
        /// ID người react
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Tên người react
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Loại reaction
        /// </summary>
        public SocialReactionType Reaction { get; set; }

        /// <summary>
        /// Thời gian react
        /// </summary>
        public DateTime ReactedDate { get; set; }
    }

    /// <summary>
    /// Base class chung cho các entity social media có content, attachments và reactions
    /// </summary>
    public abstract class BaseSocialEntity<T> : BaseTableService<T> where T : BaseSocialEntity<T>
    {
        [Key]
        public Guid Id { get; set; }

        /// <value>Nội dung</value>
        [Required]
        public string Content { get; set; }

        /// <value>Danh sách tệp đính kèm</value>
        [Column(TypeName = "jsonb")]
        public List<jsonb_Attachment> Attachments { get; set; } = new();

        /// <value>Danh sách reactions</value>
        [Column(TypeName = "jsonb")]
        public List<jsonb_SocialReaction> Reactions { get; set; } = new();

        [ForeignKey(nameof(CreatedByUserId))]
        public idm_User CreatedByUser { get; set; }

        [ForeignKey(nameof(LastModifiedByUserId))]
        public idm_User LastModifiedByUser { get; set; }
    }
}
