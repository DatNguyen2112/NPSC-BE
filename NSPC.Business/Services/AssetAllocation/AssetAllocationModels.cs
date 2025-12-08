using NSPC.Common;

namespace NSPC.Business.Services.AssetAllocation;

public class AssetAllocationViewModel
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetName { get; set; }
    public string Operation { get; set; }
    public DateTime ExecutionDate { get; set; }
    public Guid? FromLocationId { get; set; }
    public string FromLocationName { get; set; }
    public Guid? ToLocationId { get; set; }
    public string ToLocationName { get; set; }
    public Guid? FromUserId { get; set; }
    public string FromUserName { get; set; }
    public Guid? ToUserId { get; set; }
    public string ToUserName { get; set; }
    public string Description { get; set; }
    public Guid CreatedByUserId { get; set; }
    public Guid? LastModifiedByUserId { get; set; }
    public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
    public DateTime CreatedOnDate { get; set; } = DateTime.Now;
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserName { get; set; }
    public string Status { get; set; }
    public UserModel ToUser { get; set; }
    public AssetLocationViewModel ToLocation { get; set; }
}
public class AssetAllocationQueryModel : PaginationRequest
{
    public Boolean IsActive { get; set; }
}