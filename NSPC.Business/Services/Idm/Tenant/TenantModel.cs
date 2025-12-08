using Newtonsoft.Json;
using NSPC.Common;
using NSPC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public class TenantModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SubDomain { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Plan { get; set; }
        public int MaxUsers { get; set; }
        public string CompanyName { get; set; }
        public string MST { get; set; }
        public string WebUrl { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public AttachmentViewModel Logo { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
    }
    public class TenantCreateModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SubDomain { get; set; }
        public string CompanyName { get; set; }
        public string MST { get; set; }
        public string Password { get; set; }
        public List<jsonb_Attachment> Attachments { get; set; }
    }

    public class TenantUpdateModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string MST { get; set; }
        public string PhoneNumber { get; set; }

        public List<jsonb_Attachment> Attachments { get; set; }
    }

    public class TenantQueryModel : PaginationRequest
    {
        public string Code { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SubDomain { get; set; }

        public DateTime?[] DateRange { get; set; }  
    }
}
