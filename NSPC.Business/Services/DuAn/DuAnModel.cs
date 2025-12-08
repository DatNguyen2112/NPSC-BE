using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.InventoryNote;
using NSPC.Business.Services.NguyenVatLieu;
//using NSPC.Business.Services.QuanLyKho;
using NSPC.Business.Services.Quotation;
using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.DuAn
{
    public class DuAnViewModel
    {
        public Guid Id { get; set; }
        public string MaDuAn { get; set; }
        public string TenDuAn { get; set; }
        public decimal TongHopThu { get; set; }
        public decimal TongHopChi { get; set; }
        public List<QuotationViewModelInProject> BaoGia { get; set; }
        public List<CashbookTransactionViewModelInProject> Thu { get; set; }
        public List<CashbookTransactionViewModelInProject> Chi { get; set; }
        public List<InventoryNoteViewModelInProject> PhieuNhapKho { get; set; }
        public List<InventoryNoteViewModelInProject> PhieuXuatKho { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }
    
    public class DuAnCreateUpdateModel
    {
        public string MaDuAn { get; set; }
        public string TenDuAn { get; set; }
    }

    public class DuAnQueryModel : PaginationRequest
    {
        public string MaDuAn { get; set; }
        public string TenDuAn { get; set; }
        public decimal? TongHopThu { get; set; }
        public decimal? TongHopChi { get; set; }
    }
}
