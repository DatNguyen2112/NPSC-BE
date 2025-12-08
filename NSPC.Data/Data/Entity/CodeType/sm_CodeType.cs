//using NSPC.Data.Data.Entity.QuanLyKho;
using NSPC.Data.Data.Entity.CodeType;
using NSPC.Data.Data.Entity.EInvoice;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("sm_CodeType")]
    public class sm_CodeType : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int TranslationCount { get; set; }
        public string IconClass { get; set; }
        public ICollection<sm_CodeType_Translation> sm_CodeType_Translation { get; set; }
        public virtual ICollection<sm_CodeType_Item> CodeTypeItems { get; set; } = new List<sm_CodeType_Item>();
    }
}