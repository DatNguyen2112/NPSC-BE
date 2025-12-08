using NSPC.Data.Data.Entity.CashbookTransaction;
using NSPC.Data.Data.Entity.Quotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Data.Entity;

namespace NSPC.Data
{
    [Table("sm_Customer")]

    public class sm_Customer : BaseTableService<sm_Customer>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public string Note { get; set; }
        public List<string> ListPersonInCharge { get; set; }
        public decimal DebtAmount { get; set; }
        /// <summary>
        /// Tổng chi tiêu
        /// </summary>
        public decimal ExpenseAmount { get; set; } = 0;
        /// <summary>
        /// Tổng đơn hàng
        /// </summary>
        public decimal OrderCount { get; set; } = 0;
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Fax { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public DateTime? Birthdate { get; set; }
        public string LinkFacebook { get; set; }
        public string LinkTiktok { get; set; }
        public string LinkTelegram { get; set; }
        
        /// <summary>
        /// Loại khách hàng
        /// </summary>
        public string CustomerType { get; set; }
        /// <summary>
        /// Nguồn tiếp thị
        /// </summary>
        public List<string> CustomerSource { get; set; }
        
        
        public string InitialRequirement { get; set; }
        public string InformationToCopy { get; set; }
        public int TotalQuotationCount { get; set; }
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int WardCode { get; set; }
        public string WardName { get; set; }
        public bool IsActive { get; set; }
        public string CustomerGroupCode { get; set; }
        // public ICollection<sm_LichSuChamSoc> sm_LichSuChamSoc { get; set; }
        // public ICollection<sm_CustomerServiceComment> sm_CustomerServiceComment { get; set; } = new List<sm_CustomerServiceComment>();
        [ForeignKey("CreatedByUserId")]
        public virtual idm_User CreatedByUser { get; set; }
        public DateTime? LastCareOnDate { get; set; }
        public int TotalCareTimes { get; set; }
        public string PaymentMethod { get; set; }
    }
}
