using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.Bom
{
    [Table("sm_Materials")]
    public class sm_Materials : BaseTableService<sm_Materials>
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int LineNumber { get; set; }
        /// <summary>
        /// Id nguyên vật liệu
        /// </summary>
        public Guid MaterialId { get; set; }
        /// <summary>
        /// Mã nguyên vật liệu
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// Tên nguyên vật liệu
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string MaterialUnit { get; set; }
        /// <summary>
        /// % thuế nguyên vật liệu dòng
        /// </summary>
        public decimal LineVatPercent { get; set; } = 0M;
        /// <summary>
        /// số tiền thuế nguyên vật liệu dòng
        /// </summary>
        public decimal LineVatAmount { get; set; } = 0M;
        /// <summary>
        /// Số lượng
        /// </summary>
        public decimal Quantity { get; set; } = 0M;
        /// <summary>
        /// Đơn giá
        /// </summary>
        public decimal UnitPrice { get; set; } = 0M;
        /// <summary>
        /// Tổng tiền dòng
        /// </summary>
        public decimal LineAmount { get; set; } = 0M;
        /// <summary>
        /// Id bảng BOM
        /// </summary>
        public Guid BomId { get; set; }
        [ForeignKey("BomId")]
        public virtual sm_Bom sm_Bom { get; set; }
    }
}
