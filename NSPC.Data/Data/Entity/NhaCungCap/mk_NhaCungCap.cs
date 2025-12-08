using NSPC.Data.Data.Entity.CashbookTransaction;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.NhaCungCap
{
    [Table("sm_Supplier")]
    public class sm_Supplier : BaseTableService<sm_Supplier>
    {
        [Key]
        public Guid Id { get; set; }
        public virtual ICollection<sm_PurchaseOrder> mk_QuanLyKho { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public string Email {  get; set; }
        public string Fax {  get; set; }
        public string Website {  get; set; }
        public string NguoiPhuTrach {  get; set; }
        public bool   IsActive { get; set; }
        public string Address { get; set; }
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int WardCode { get; set; }
        public string WardName { get; set; }
        public string SupplierGroupCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public decimal TotalDebtAmount { get; set; }
        
        /// <summary>
        /// Danh sách tài khoản
        /// </summary
        [Column(TypeName = "jsonb")]
        public List<jsonb_AccountBanking> ListAccountBanking { get; set; }
        
        /// <summary>
        /// Tổng chi tiêu
        /// </summary>
        public decimal ExpenseAmount { get; set; } = 0;
        
        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public string PaymentMethod { get; set; }
        
        /// <summary>
        /// Tổng đơn hàng
        /// </summary>
        public decimal OrderCount { get; set; } = 0;
    }
}
