using NSPC.Business.Services.InvestorType;
using NSPC.Common;

namespace NSPC.Business.Services
{
    public class InvestorCreateModel
    {
        public string Code  { get; set; }
        public string Name  { get; set; }
        public Guid InvestorTypeId  { get; set; }
    }
    
    public class InvestorViewModel
    {
       public Guid Id  { get; set; }
       public string Code { get; set; }
       public string Name { get; set; }
       public Guid InvestorTypeId { get; set; }
       public InvestorTypeViewModel InvestorType { get; set; }
       public Guid CreatedByUserId { get; set; }
       public Guid? LastModifiedByUserId { get; set; }
       public DateTime? LastModifiedOnDate { get; set; }
       public DateTime CreatedOnDate { get; set; }
       public string CreatedByUserName { get; set; }
       public string LastModifiedByUserName { get; set; }
    }
    
    public class InvestorQueryModel: PaginationRequest
    {
        public string Code  { get; set; }
        public string Name   { get; set; }
    }
}

