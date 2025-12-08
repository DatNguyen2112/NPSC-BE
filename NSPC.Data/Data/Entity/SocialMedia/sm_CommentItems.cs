using NSPC.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    [Table("sm_CommentItems")]
    public class sm_CommentItems: BaseTableService<sm_CommentItems>
    {
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Id comment cha
        /// </summary>
        public Guid CommentId { get; set; }
        
        /// <summary>
        /// Nội dung comment con
        /// </summary>
        public string ContentItems { get; set; }
        
        /// <summary>
        /// Ảnh gán trên comment con
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_Attachment> CommentItemsAttachments { get; set; }
        
        /// <summary>
        /// Trạng thái comment con
        /// </summary>
        public string PostStatusCodeInCommentItems { get; set; }
        
        /// <summary>
        /// Gán người dùng ở comment con
        /// </summary>
        public List<string> TagUserIdsInCommentItems { get; set; }
        
        /// <summary>
        /// Khoá ngoại CommentId
        /// </summary>
        [ForeignKey("CommentId")]
        public virtual sm_Comments sm_Comments { get; set; }
        
        // FK Người tạo
        [ForeignKey("CreatedByUserId")]
        public virtual idm_User CreatedByUser { get; set; }
    }
}

