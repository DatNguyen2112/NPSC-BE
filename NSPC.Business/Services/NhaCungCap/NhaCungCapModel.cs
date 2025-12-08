using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Data;

namespace NSPC.Business.Services.NhaCungCap
{
    public class NhaCungCapViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int WardCode { get; set; }
        public string WardName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Fax { get; set; }
        public string TaxCode { get; set; }
        public string Website { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// Danh sách khởi tạo kho ban đầu
        /// </summary
        public List<jsonb_AccountBanking> ListAccountBanking { get; set; }
        public decimal TotalDebtAmount { get; set; }
        /// <summary>
        /// Tổng chi tiêu
        /// </summary>
        public decimal ExpenseAmount { get; set; } = 0;
        /// <summary>
        /// Tổng đơn hàng
        /// </summary>
        public decimal OrderCount { get; set; } = 0;
        public CodeTypeListModel SupplierGroup { get; set; }
        public string NguoiPhuTrach { get; set; }
        
        public string PaymentMethod { get; set; }
    }
    public class NhaCungCapCreateUpdateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int WardCode { get; set; }
        public string WardName { get; set; }
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Danh sách khởi tạo kho ban đầu
        /// </summary
        public List<jsonb_AccountBanking> ListAccountBanking { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Fax { get; set; }
        public string TaxCode { get; set; }
        public string Website { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }
        public decimal TotalDebtAmount { get; set; }
        public string SupplierGroupCode { get; set; }
        public string NguoiPhuTrach { get; set; }
        
        public string PaymentMethod { get; set; }
    }

    public class MultipleDeleteModel
    {
        public object Deleted { get; set; }
        public object CannotDelete { get; set; }
        public string Message { get; set; }
    }
    
    public class NhaCungCapQueryModel : PaginationRequest
    {
        public string Code { get; set; }
        public string SupplierGroupCode { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public DateTime?[] DateRange { get; set; }
    }
}
