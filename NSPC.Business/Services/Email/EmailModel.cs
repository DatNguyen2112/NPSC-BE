using System;
using System.Collections.Generic;
using NSPC.Common;
using NSPC.Data;
using Newtonsoft.Json;

namespace NSPC.Business
{
    public class EmailTemplateViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        [JsonIgnore]
        public string TitleTemplate { get; set; }

        [JsonIgnore]
        public string BodyTemplate { get; set; }

        public List<EmailTemplateTranslationModel> Translations { get; set; }
    }

    public class EmailTemplateTranslationModel
    {
        public string TitleTemplate { get; set; }
        public string BodyTemplate { get; set; }
        public string Language { get; set; }
    }

    public class EmailTemplateQueryModel : PaginationRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
    }

    public class EmailTemplateCreateUpdateModel
    {
        [JsonIgnore]
        public Guid? Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public List<EmailTemplateTranslationModel> Translations { get; set; }
    }

    // Email subcribe model
    public class EmailSubscriptionModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public DateTime? SubscribeDate { get; set; }
        public DateTime? UnsubscribeDate { get; set; }
        public int TotalEmailSentCount { get; set; }
        public Guid? PartnerId { get; set; }
    }

    public class EmailSubscribeModel
    {
        public string Email { get; set; }
        public Guid? PartnerId { get; set; }
    }

    public class EmailSubscribeQueryModel : PaginationRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public Guid? PartnerId { get; set; }
    }
}