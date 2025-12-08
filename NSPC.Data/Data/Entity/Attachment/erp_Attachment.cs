using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    public class erp_Attachment : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }
        public string DocType { get; set; }
        public string DocTypeName { get; set; }
        public Guid? EntityId { get; set; }
        [MaxLength(15)]
        public string EntityType { get; set; }
        public string FilePath { get; set; }
        public Guid CreatedByUserId { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? DeletedOnDate { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public long FileSize { get; set; }
        [StringLength(256)]
        public string OriginalFileName { get; set; }
        [StringLength(15)]
        public string StatusCode { get; set; }
        
        public string UpdateFrequency { get; set; }
        public string Language { get; set; }
        public string CopyRight { get; set; }
        public bool IsPrivate { get; set; }
        public string License { get; set; }
        public Guid OrganizationId { get; set; }

        // temp: file moi up len
        // inuse: file duoc su dung trong 1 entity nao do
        // delete_confirmed: file bi danh dau xoa, khi entity bi xoa
        // deleted: file da duoc xoa

    }
}