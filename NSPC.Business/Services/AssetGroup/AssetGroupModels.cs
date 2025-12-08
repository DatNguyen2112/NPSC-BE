using NSPC.Common;

namespace NSPC.Business;

public class AssetGroupViewModel
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public DateTime CreatedOnDate { get; set; }
    public Guid? LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    public DateTime? LastModifiedOnDate { get; set; }
}

public class AssetGroupCreateUpdateModel
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class AssetGroupQueryModel : PaginationRequest
{
    public string Code { get; set; }
    public string Name { get; set; }
}