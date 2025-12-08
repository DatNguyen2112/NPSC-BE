using NSPC.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NSPC.Business
{
    public class AttachmentListViewModel
    {
        public Guid Id { get; set; }
        public string DocType { get; set; }
        public string FileUrl { get; set; }
        public string Description { get; set; }
    }
    public class AttachmentViewModel
    {
        public Guid Id { get; set; }
        public string DocType { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileUrl { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }
    }
    public class AttachmentDetailViewModel
    {
        public Guid Id { get; set; }
        public string DocType { get; set; }
        public string DocTypeName { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public Guid? EntityId { get; set; }
        public string EntityType { get; set; }
        [JsonIgnore]
        public Guid CreatedByUserId { get; set; }
        public string Description { get; set; }
    }

    public class AttachmentCreateUpdateModel
    {
        public string DocType { get; set; }
        public string DocTypeName { get; set; }
        public string EntityType { get; set; }
        public string File { get; set; }
        public string Prefix { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public Guid CreatedByUserId { get; set; }
        [JsonIgnore]
        public DateTime CreatedOnDate { get; set; }
    }

    public class AttachmentCreateUpdateModel_V1
    {
        public string DocType { get; set; }
        public string DocTypeName { get; set; }
        public string EntityType { get; set; }
        public string File { get; set; }
        public string Prefix { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public Guid CreatedByUserId { get; set; }
        [JsonIgnore]
        public DateTime CreatedOnDate { get; set; }
    }


    public class AttachmentQueryModel : PaginationRequest
    {
        public string DocType { get; set; }
        public Guid? EntityId { get; set; }
    }

    
    public class AttachmentTemplate
    {
        public string EntityType { get; set; }
        public string DocType { get; set; }
        public string DocTypeName { get; set; }
        public int Order { get; set; }
        public int MaxQuantity { get; set; }
        public int MinQuantity { get; set; }
    }
}