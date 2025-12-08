using NSPC.Common;

namespace NSPC.Business;

public class AssetLocationViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; }
    public string Description { get; set; }
    public DateTime CreatedOnDate { get; set; }
    public string CreatedByUserName { get; set; }
    public DateTime? LastModifiedOnDate { get; set; }
    public string LastModifiedByUserName { get; set; }
    public List<AssetLocationViewModel> Children { get; set; } = new();
}

public class AssetLocationCreateUpdateModel
{
    public string Name { get; set; }
    public string Code { get; set; }
    public Guid? ParentId { get; set; }
    public string Description { get; set; }
}

public class AssetLocationQueryModel : PaginationRequest
{
    public string FullTextSearch { get; set; }
    public Guid? ParentId { get; set; }
}