using NSPC.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NSPC.Business.Services.EInvoice;

namespace NSPC.Business
{
    public class CodeTypeTypeListModel
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string IconClass { get; set; }
    }
    public class SellerTransactionListModel : CodeTypeListModel
    {
        public List<CodeTypeListModel> Childs { get; set; }
        public bool HasChild { get; set; } = false;
    }
    public class CodeTypeListModel
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; }
        public Guid? TenantId { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public List<CodeTypeTranslationModel> Translations { get; set; }
        public List<CodeTypeItemViewModel> CodeTypeItems { get; set; } = new List<CodeTypeItemViewModel>();
        public DateTime? CreatedOnDate { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
    }

    public class CodeTypeViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string IconClass { get; set; }
        public List<CodeTypeTranslationModel> Translations { get; set; }
        public List<CodeTypeItemViewModel> CodeTypeItems { get; set; } = new List<CodeTypeItemViewModel>();
        public DateTime? CreatedOnDate { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
    }

    public class CodeTypeTranslationModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
    }

    public class CodeTypeCreateUpdateModel
    {
        public Guid? ParentId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Code { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string Type { get; set; }
        public string IconClass { get; set; }
        public List<CodeTypeTranslationModel> Translations { get; set; }
        public List<CodeTypeItemCreateUpdateModel> CodeTypeItems { get; set; } = new List<CodeTypeItemCreateUpdateModel>();
    }

    public class CodeTypeQueryModel : PaginationRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        public string Format { get; set; }
        public bool? IsChildrenCustomer { get; set; }
    }
    public class CustomerLabelCreateUpdateModel{
        public string Title { get; set; }
    }

    public class CodeTypeItemViewModel
    {
        public Guid Id { get; set; }

        public int LineNumber {  get; set; }

        public string Code { get; set; }

        public string Title { get; set; }

        public string IconClass { get; set; }

        public Guid CodeTypeId { get; set; }

        public DateTime? CreatedOnDate { get; set; }
    }

    public class CodeTypeItemCreateUpdateModel
    {
        public int LineNumber { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }

        public string IconClass { get; set; }
    }
}