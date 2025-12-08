using NSPC.Common;
using NSPC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services;
using NSPC.Business.Services.DuAn;

namespace NSPC.Business
{
    public class LichSuChamSocViewModel
    {
        public Guid Id { get; set; }
        public string GhiChu { get; set; }
        public int DanhGia { get; set; }
        public Guid CustomerId { get; set; }
        
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
        /// Người tham gia
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
        
        public Guid? ProjectId { get; set; }
        
        public DuAnViewModel Project { get; set; }
        public KhachHangViewModel KhachHang { get; set; }
        public BaseUserModel CreatedByUser { get; set; }
        // public CustomerServiceCommentViewModel CustomerServiceComment { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
        public DateTime CreatedOnDate { get; set; } = DateTime.Now;
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class LichSuChamSocCreateUpdateModel
    {
        public Guid CustomerId { get; set; }
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
        /// Người tham gia
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
        
        public Guid? ProjectId { get; set; }
    }
    public class LichSuChamSocQueryModel : PaginationRequest
    {
        public Guid? CustomerId { get; set; }
    }
}
