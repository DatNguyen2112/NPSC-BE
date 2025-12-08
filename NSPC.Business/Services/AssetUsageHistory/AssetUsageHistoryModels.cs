using NSPC.Common;

namespace NSPC.Business;

public class AssetUsageHistoryViewModel
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetName { get; set; }
    public string AssetCode { get; set; }
    public string Operation { get; set; }
    public string AssetStatus { get; set; }
    public DateTime ExecutionDate { get; set; }
    public Guid? LocationId { get; set; }
    public string LocationName { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; }
    public decimal? Cost { get; set; }
    public string Description { get; set; }
    public Guid? EntityId { get; set; }
    public string EntityType { get; set; }
    public DateTime CreatedOnDate { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
}

public class AssetUsageHistoryQueryModel : PaginationRequest
{
    public Guid? AssetId { get; set; }
    public string Operation { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}