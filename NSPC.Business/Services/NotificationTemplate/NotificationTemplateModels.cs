using NSPC.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NSPC.Business
{
    public class NotificationTemplateViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string TitleTemplate { get; set; }
        public string BodyPlainTextTemplate { get; set; }
        public string BodyHtmlTemplate { get; set; }
        public string Language { get; set; }
        [JsonIgnore]
        public List<NotificationTemplateTranslationModel> Translations { get; set; }
    }

    public class NotificationTemplateAdminViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public List<NotificationTemplateTranslationModel> Translations { get; set; }
    }

    public class NotificationTemplateTranslationModel
    {
        public string TitleTemplate { get; set; }
        public string BodyPlainTextTemplate { get; set; }
        public string BodyHtmlTemplate { get; set; }
        public string Language { get; set; }
    }

    public class NotificationTemplateCreateUpdateModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public List<NotificationTemplateTranslationModel> Translations { get; set; }
    }

    public class NotificationTemplateQueryModel : PaginationRequest
    {
        public string Type { get; set; }
        public string Language { get; set; }
    }
}