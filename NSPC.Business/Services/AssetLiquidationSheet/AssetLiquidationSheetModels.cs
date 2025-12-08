using NSPC.Common;

namespace NSPC.Business.AssetLiquidationSheet;

public class AssetLiquidationSheetViewModel
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public Guid AssetId { get; set; }
    public string AssetName { get; set; }
    public DateTime LiquidationDate { get; set; }
    public string DecisionNumber { get; set; }
    public Guid? LiquidatorId { get; set; }
    public string LiquidatorName { get; set; }
    public decimal? LiquidationValue { get; set; }
    public string LiquidationReason { get; set; }
    public Guid CreatedByUserId { get; set; }
    public Guid? LastModifiedByUserId { get; set; }
    public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
    public DateTime CreatedOnDate { get; set; } = DateTime.Now;
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserName { get; set; }
}

public class AssetLiquidationSheetQueryModel : PaginationRequest
{
    public string DecisionNumber { get; set; }
    public List<DateTime?> LiquidationDate { get; set; } = new();
    public Guid? LiquidatorId { get; set; }
}

public class AssetLiquidationSheetCreateUpdateModel
{
    public Guid AssetId { get; set; }
    public DateTime LiquidationDate { get; set; }
    public string DecisionNumber { get; set; }
    public Guid? LiquidatorId { get; set; }
    public decimal? LiquidationValue { get; set; }
    public string LiquidationReason { get; set; }
}