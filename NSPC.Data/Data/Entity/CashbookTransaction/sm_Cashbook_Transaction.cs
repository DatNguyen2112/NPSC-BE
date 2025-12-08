using NSPC.Data.Data.Entity.AdvanceRequest;
using NSPC.Data.Data.Entity.Contract;
using NSPC.Data.Data.Entity.DuAn;
using NSPC.Data.Data.Entity.NhaCungCap;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Data.Data.Entity.CashbookTransaction
{
    [Table("sm_Cashbook_Transaction")]
    public class sm_Cashbook_Transaction : BaseTableService<sm_Cashbook_Transaction>
    {
        [Key]
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string PurposeCode { get; set; }
        public string PurposeName { get; set; }
        public Guid EntityId { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string EntityTypeCode { get; set; }
        public string EntityTypeName { get; set; }
        public string EntityUrl { get; set; }
        public Guid? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual mk_DuAn Mk_DuAn { get; set; }
        public Guid? OriginalDocumentId { get; set; }
        public string OriginalDocumentType { get; set; }
        public string OriginalDocumentCode { get; set; }
        public decimal Amount { get; set; } = 0M;
        public decimal? OpeningBalance { get; set; } // Tồn cuối kỳ
        public decimal? ClosingBanlance { get; set; } //Tồn cuối kỳ
        public string PaymentMethodCode { get; set; }
        public string Reference { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string IsActive { get; set; }
        public string TransactionTypeCode { get; set; }
        public bool IsDebt { get; set; }  // có cho thay đổi công nợ đối tượng nộp/nhận hay không? 
        [Column(TypeName = "jsonb")]
        public List<jsonb_Attachment> Attachments { get; set; }
        public Guid? ContractId { get; set; }
        [ForeignKey("ContractId")]
        public virtual sm_Contract sm_Contract { get; set; }

        public Guid? ConstructionId { get; set; }
        [ForeignKey("ConstructionId")]
        public virtual sm_Construction sm_Construction { get; set; }

        public Guid? AdvanceRequestId { get; set; }
        [ForeignKey("AdvanceRequestId")]
        public virtual sm_AdvanceRequest sm_AdvanceRequest { get; set; }
    }
}
