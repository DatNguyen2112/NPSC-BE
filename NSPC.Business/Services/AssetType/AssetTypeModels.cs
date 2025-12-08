using NSPC.Common;

namespace NSPC.Business;

public class AssetTypeViewModel
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public Guid AssetGroupId { get; set; }
    public string AssetGroupName { get; set; }
    public string Description { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public DateTime CreatedOnDate { get; set; }
    public Guid? LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    public DateTime? LastModifiedOnDate { get; set; }
}

public class AssetTypeQueryModel : PaginationRequest
{
    public string Code { get; set; }
    public new string Name { get; set; }
    public Guid? AssetGroupId { get; set; }
}

public class AssetTypeCreateUpdateModel
{
    public string Code { get; set; }
    public string Name { get; set; }
    public Guid AssetGroupId { get; set; }
    public string Description { get; set; }
}