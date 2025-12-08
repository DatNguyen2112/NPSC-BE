using NSPC.Business.Services;
using NSPC.Common;

namespace NSPC.Business
{
    public class KhachHangViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public string Note { get; set; }
        public decimal DebtAmount { get; set; }

        /// <summary>
        /// Tổng chi tiêu
        /// </summary>
        public decimal ExpenseAmount { get; set; } = 0;

        /// <summary>
        /// Tổng đơn hàng
        /// </summary>
        public decimal OrderCount { get; set; } = 0;
        
        /// <summary>
        /// Nguồn tiếp thị
        /// </summary>
        public List<string> CustomerSource { get; set; }
        
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int WardCode { get; set; }
        public string WardName { get; set; }
        public DateTime? Birthdate { get; set; }
        public string LinkFacebook { get; set; }
        public string Fax { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string LTiktok { get; set; }
        public string LinkTelegram { get; set; }
        /// <summary>
        /// Loại khách hàng
        /// </summary>
        public string CustomerType { get; set; }
        
        public bool IsActive { get; set; }
        public List<LichSuChamSocViewModel> LichSuChamSoc { get; set; }
        public List<CustomerServiceCommentViewModel> CustomerServiceComment { get; set; }
        public BaseUserModel CreatedByUser { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
        public DateTime CreatedOnDate { get; set; } = DateTime.Now;
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
        public DateTime? LastCareOnDate { get; set; }
        public int TotalCareTimes { get; set; }
        public int TotalDaysLastCare { get; set; }
        public string InitialRequirement { get; set; }
        public string InformationToCopy { get; set; }
        public int TotalQuotationCount { get; set; }
        public CodeTypeListModel CustomerGroup { get; set; }
        public List<string> ListPersonInCharge { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class KhachHangCreateUpdateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public string Note { get; set; }
        public decimal DebtAmount { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        
        /// <summary>
        /// Loại khách hàng
        /// </summary>
        public string CustomerType { get; set; }
        
        /// <summary>
        /// Nguồn tiếp thị
        /// </summary>
        public List<string> CustomerSource { get; set; }

        public string Fax { get; set; }
        public string Sex { get; set; }
        public bool IsActive { get; set; }
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int WardCode { get; set; }
        public string WardName { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string? Quotation { get; set; }
        public DateTime? Birthdate { get; set; }
        public string LinkFacebook { get; set; }
        public string LinkTiktok { get; set; }
        public string LinkTelegram { get; set; }
        public List<string> LoaiKhachHang { get; set; }
        public string InitialRequirement { get; set; }
        public string CustomerGroupCode { get; set; }
        public List<string> ListPersonInCharge { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class MultipleDeleteModel
    {
        public object Deleted { get; set; }
        public object CannotDelete { get; set; }
        public string Message { get; set; }
    }

    public class CustomerSummaryView
    {
        public decimal? TotalAllCustomerQuantity { get; set; }
        public decimal? TotalNewCustomerQuantity { get; set; } // Mới
        public decimal? TotalRisingCustomerQuantity { get; set; } // Tiềm năng
        public decimal? TotalPendingCustomerQuantity { get; set; } // Hẹn tư vấn
        public decimal? TotalAlreadyContactCustomerQuantity { get; set; } // Đã liên hệ
        public decimal? TotalPendingDiscussCustomerQuantity { get; set; } // Đang đàm phán
        public decimal? TotalCancelCustomerQuantity { get; set; } // Từ chối
        public decimal? TotalLoyalCustomerQuantity { get; set; } // Trung thành
        public decimal? TotalNotResponseCustomerQuantity { get; set; } // Không phản hồi
        public decimal? TotalSignContractCustomerQuantity { get; set; } // Chốt hợp đồng
        public decimal? TotalOtherCustomerQuantity { get; set; } // Khác
    }

    public class KhachHangQueryModel : PaginationRequest
    {
        public string CustomerType { get; set; }
        public string TaxCode { get; set; }
        public string CustomerGroupCode { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public string Sex { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public DateTime?[] DateRange { get; set; }
        public List<string> ListPersonInCharge { get; set; }
    }
}