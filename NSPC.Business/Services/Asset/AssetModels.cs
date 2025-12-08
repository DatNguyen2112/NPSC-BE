using NSPC.Business.Services.AssetAllocation;
using NSPC.Common;
using NSPC.Data;

namespace NSPC.Business;

public class AssetViewModel
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public Guid AssetTypeId { get; set; }
    public string AssetTypeName { get; set; }
    public Guid AssetLocationId { get; set; }
    public string AssetLocationName { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; }
    public Guid? MeasureUnitId { get; set; }
    public string MeasureUnitName { get; set; }
    public string Description { get; set; }
    public DateTime PurchasedDate { get; set; }
    public string Status { get; set; }
    public string Serial { get; set; }
    public decimal OriginalPrice { get; set; }
    public string OriginBrand { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public decimal? DepreciationValue { get; set; }
    public int? DepreciationTime { get; set; }
    public string DepreciationUnit { get; set; }
    public DateTime? DepreciationStartDate { get; set; }
    public decimal? AccumulatedDepreciation { get; set; }
    public decimal? RemainingValue { get; set; }
    public Guid CreatedByUserId { get; set; }
    public Guid? LastModifiedByUserId { get; set; }
    public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
    public DateTime CreatedOnDate { get; set; } = DateTime.Now;
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserName { get; set; }
    public List<string> AllowedOperations { get; set; } = new();
}

public class AssetDetailViewModel : AssetViewModel
{
    public List<AttachmentViewModel> Images { get; set; } = new();
    public List<AttachmentViewModel> Documents { get; set; } = new();
    public AssetAllocationViewModel Allocation { get; set; }
    public List<AssetUsageHistoryViewModel> AssetUsageHistories { get; set; } = new();
}

public class AssetQueryModel : PaginationRequest
{
    public new string Name { get; set; }
    public string Code { get; set; }
    public Guid? AssetTypeId { get; set; }
    public List<DateTime?> PurchasedDate { get; set; } = new();
    public Guid? AssetLocationId { get; set; }
    public Guid? UserId { get; set; }
    public string Status { get; set; }
}

public class AssetCreateUpdateModel
{
    public string Name { get; set; }
    public Guid AssetTypeId { get; set; }
    public Guid AssetLocationId { get; set; }
    public Guid? MeasureUnitId { get; set; }
    public int Quantity { get; set; } = 1;
    public string Description { get; set; }
    public DateTime? PurchasedDate { get; set; }
    public string Serial { get; set; }
    public decimal OriginalPrice { get; set; }
    public string OriginBrand { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public decimal? DepreciationValue { get; set; }
    public int? DepreciationTime { get; set; }
    public string DepreciationUnit { get; set; }
    public DateTime? DepreciationStartDate { get; set; }
    public List<jsonb_Attachment> Images { get; set; } = new();
    public List<jsonb_Attachment> Documents { get; set; } = new();
}

public class AssetReportDamageModel
{
    public DateTime IncidentDate { get; set; }
    public string Description { get; set; }
}

public class AssetReportDestroyedModel
{
    public DateTime IncidentDate { get; set; }
    public string Description { get; set; }
}

public class AssetReportLostModel
{
    public DateTime IncidentDate { get; set; }
    public decimal CompensationAmount { get; set; }
    public string Description { get; set; }
}

public class AssetAllocateModel
{
    public Guid AssetLocationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExecutionDate { get; set; }
    public string Description { get; set; }
}
public class AssetApprovalModel
{
    public bool IsAllocate { get; set; }
    public string Description { get; set; }
    public string Code { get; set; }
    public Guid UserId { get; set; }
}
