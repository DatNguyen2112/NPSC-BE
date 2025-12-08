using NSPC.Common;

namespace NSPC.Business;

public class AssetMaintenanceSheetViewModel
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public Guid AssetId { get; set; }
    public string AssetName { get; set; }
    public string MaintenanceType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime MaintenancePeriod { get; set; }
    public Guid? PerformerId { get; set; }
    public string PerformerName { get; set; }
    public string MaintenanceLocation { get; set; }
    public string MaintenancePlace { get; set; }
    public decimal? EstimatedCost { get; set; }
    public string MaintenanceContent { get; set; }
    public string Status { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public DateTime CreatedOnDate { get; set; }
    public Guid? LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    public DateTime? LastModifiedOnDate { get; set; }
}

public class AssetMaintenanceSheetQueryModel : PaginationRequest
{
    public string AssetName { get; set; }
    public string AssetCode { get; set; }
    public List<DateTime?> MaintenancePeriod { get; set; } = new();
    public string MaintenanceType { get; set; }
    public string Status { get; set; }
}

public class AssetMaintenanceSheetCreateUpdateModel
{
    public Guid AssetId { get; set; }
    public string MaintenanceType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime MaintenancePeriod { get; set; }
    public Guid? PerformerId { get; set; }
    public string MaintenanceLocation { get; set; }
    public string MaintenancePlace { get; set; }
    public decimal? EstimatedCost { get; set; }
    public string MaintenanceContent { get; set; }
}

public class AssetMaintenanceSheetCompleteModel
{
    public DateTime CompletedDate { get; set; }
}