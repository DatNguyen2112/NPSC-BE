using NSPC.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    [Table("sm_SocialMediaPost")]
    public class sm_SocialMediaPost: BaseTableService<sm_SocialMediaPost>
    { 
        [Key]
       public Guid Id  { get; set; }
       
       /// <summary>
       /// Tiêu đề
       /// </summary>
       public string ContentPostTitle { get; set; }
       
       /// <summary>
       /// Nội dung bài viết
       /// </summary>
       public string ContentPostBody { get; set; }
       
       
       /// <summary>
       /// Trạng thái bài viết (Like, Haha, Wow, Sad, Angry, Love)
       /// </summary>
       public string PostStatusCode { get; set; }
       
       /// <summary>
       /// Ảnh gán trên bài viết
       /// </summary>
       [Column(TypeName = "jsonb")]
       public List<jsonb_Attachment> PostAttachments { get; set; }
       
       /// <summary>
       /// Comment của bài viết
       /// </summary>
       public virtual ICollection<sm_Comments> sm_Comments { get; set; } = new List<sm_Comments>();
       
       /// <summary>
       /// Người dùng được tag vào bài viết
       /// </summary>
       public List<string> TagUserIds  { get; set; } =  new List<string>();
       
       // FK Người tạo
       [ForeignKey("CreatedByUserId")]
       public virtual idm_User CreatedByUser { get; set; }
    }
}

