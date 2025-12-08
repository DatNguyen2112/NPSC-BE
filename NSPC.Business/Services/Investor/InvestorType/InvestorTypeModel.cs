using NSPC.Common;

namespace NSPC.Business.Services.InvestorType
{
    public class InvestorTypeCreateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    
    public class InvestorTypeViewModel
    {
       public Guid Id { get; set; }
       public string Code { get; set; }
       public string Name { get; set; }
       public List<InvestorDTO> Investor { get; set; } =  new List<InvestorDTO>();
       public Guid CreatedByUserId { get; set; }
       public Guid? LastModifiedByUserId { get; set; }
       public DateTime? LastModifiedOnDate { get; set; }
       public DateTime CreatedOnDate { get; set; }
       public string CreatedByUserName { get; set; }
       public string LastModifiedByUserName { get; set; }
    }

    public class InvestorDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class InvestorTypeQueryModel: PaginationRequest
    {
        public string Code  { get; set; }
        public string Name   { get; set; }
    }
}

