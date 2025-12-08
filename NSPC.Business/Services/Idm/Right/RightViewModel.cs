using NSPC.Common;

namespace NSPC.Business
{
    public class RightViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public DateTime? CreatedOnDate { get; set; } = DateTime.Now;
        public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
    }

    public class RightCreateUpdateModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
    }

    public class RightQueryModel : PaginationRequest
    {
    }
}