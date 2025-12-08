using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Data.Data.Entity.DuAn;
using NSPC.Data.Entity;

namespace NSPC.Data
{
    [Table("sm_LichSuChamSoc")]
    public class sm_LichSuChamSoc : BaseTableService<sm_LichSuChamSoc>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string GhiChu { get; set; }
        public int DanhGia { get; set; }
        
        /// <summary>
        /// Mã chăm sóc
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Loại chăm sóc
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Nội dung
        /// </summary>
        public string CustomerServiceContent { get; set; }
        
        /// <summary>
        /// Khoảng thời gian thực hiện`
        /// </summary>
        public DateTime?[] DateRange { get; set; }
        
        /// <summary>
        /// Danh sách người tham gia
        /// </summary>
        public List<string> Participants { get; set; }
        
        /// <summary>
        /// Mức độ ưu tiên
        /// </summary>
        public string Priority { get; set; }
        
        /// <summary>
        /// Màu sắc mức độ ưu tiên
        /// </summary>
        public string PriorityColor { get; set; }
        
        /// <summary>
        /// Trạng thái
        /// </summary>
        public string StatusCode { get; set; }
        
        public Guid CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual sm_Customer sm_Customer { get; set; }
        
        /// <summary>
        /// Dự án
        /// </summary>
        public Guid? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual mk_DuAn mk_DuAn { get; set; }
        
        [ForeignKey("CreatedByUserId")]
        public virtual idm_User CreatedByUser { get; set; }
        
        // public ICollection<sm_CustomerServiceComment> sm_CustomerServiceComment { get; set; } = new List<sm_CustomerServiceComment>();
    }
}
