using NSPC.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    [Table("sm_Comments")]
    public class sm_Comments: BaseTableService<sm_Comments>
    {
        [Key]
        public Guid Id  { get; set; }    
        
        /// <summary>
        /// Id Bài viết
        /// </summary>
        public Guid PostId  { get; set; }
        
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content  { get; set; }
        
        /// <summary>
        /// Gán người dùng trong comment
        /// </summary>
        public List<string> TagUserIdsInComment { get; set; } = new List<string>();
        
        /// <summary>
        /// Comment của comment cha
        /// </summary>
        public virtual ICollection<sm_CommentItems> sm_CommentItems { get; set; } = new List<sm_CommentItems>();
        
        /// <summary>
        /// Ảnh gán trên comment
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_Attachment> CommentAttachments { get; set; }
        
        /// <summary>
        /// Trạng thái comment
        /// </summary>
        public string PostStatusCodeInComment { get; set; }
        
        /// <summary>
        /// Khoá ngoại PostId
        /// </summary>
        [ForeignKey("PostId")]
        public virtual sm_SocialMediaPost sm_SocialMediaPost { get; set; }
        
        // FK Người tạo
        [ForeignKey("CreatedByUserId")]
        public virtual idm_User CreatedByUser { get; set; }
    }
}
