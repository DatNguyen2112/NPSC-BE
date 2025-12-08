namespace NSPC.Business
{
    public class RightMapRoleViewModel
    {
        public string RightName { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? RightId { get; set; }
        public string RoleCode { get; set; }
        public string RightCode { get; set; }
    }

    public class RightMapRoleAssignModel
    {
        public Guid RoleId { get; set; }
        public List<Guid> RightIds { get; set; }
    }

    public class RightMapConfigViewModel
    {
        public string GroupCode { get; set; }
        public List<string> RightCodes { get; set; }
    }

    public class RightMapConfig
    {
        public List<RightMapConfigViewModel> Rights { get; set; }
    }
}