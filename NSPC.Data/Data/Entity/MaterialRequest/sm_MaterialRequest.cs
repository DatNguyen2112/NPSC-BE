using NSPC.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Entity
{
    [Table("sm_MaterialRequest")]
    public class sm_MaterialRequest: BaseTableService<sm_MaterialRequest>
    {
        [Key]
        public Guid Id  { get; set; }
        
        /// <summary>
        /// Mã yêu cầu
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content  { get; set; }
        
        /// <summary>
        /// Hạn xử lý
        /// </summary>
        public DateTime DateProcess { get; set; }
        
        /// <summary>
        /// Độ ưu tiên
        /// </summary>
        public string Priority { get; set; }
        
        /// <summary>
        /// Tên độ ưu tiên
        /// </summary>
        public string PriorityName  { get; set; }
        
        /// <summary>
        /// Trạng thái
        /// </summary>
        public string StatusCode { get; set; }
        
        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public string StatusName  { get; set; }
        
        /// <summary>
        /// Thông tin vật tư yêu cầu
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_MaterialRequest> MaterialRequestItem { get; set; }
        
        /// <summary>
        /// Lịch sử xử lý
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<jsonb_HistoryProcess> ListHistoryProcess { get; set; }

        /// <summary>
        /// Thông tin yêu cầu vật tư
        /// </summary>
        public virtual ICollection<sm_MaterialRequestItem> MaterialRequestItems { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note  { get; set; }
        
        /// <summary>
        /// Id công trình/dự án
        /// </summary>
        public Guid ConstructionId  { get; set; }
        [ForeignKey("ConstructionId")]
        public virtual sm_Construction Construction { get; set; }
    }
}

