using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    [Table("sm_IssueActivityLog")]
    public class sm_IssueActivityLog : BaseTableService<sm_IssueActivityLog>
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Nguời thao tác
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Ảnh người thao tác
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// Mô tả action
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Mã phiếu có trong công trình dự án (gán link trên FE)
        /// </summary>
        public string CodeLinkDescription { get; set; }

        /// <summary>
        /// Id phiếu có trong công trình dự án
        /// </summary>
        public Guid OrderId { get; set; }

        [Column(TypeName = "jsonb")]
        public List<jsonb_Attachment> AttachmentsResolve { get; set; }

        public string ContentResovle { get; set; }
        public string ReasonCancel { get; set; }
        public string ReasonReopen { get; set; }

        /// <summary>
        /// Id công trình
        /// </summary>
        public Guid? ConstructionId { get; set; }
        [ForeignKey("ConstructionId")]
        public virtual sm_Construction sm_Construction { get; set; }
    }
}
