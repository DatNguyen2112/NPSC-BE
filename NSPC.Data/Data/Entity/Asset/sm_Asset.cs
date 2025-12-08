using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Data.Entity.AssetCategories;
using NSPC.Data.Data.Entity.AssetHistory;
using NSPC.Data.Data.Entity.AssetLocation;

namespace NSPC.Data.Data.Entity.Asset;

public enum AssetStatus
{
    /// <summary>
    /// Đang sử dụng
    /// </summary>
    InUse,

    /// <summary>
    /// Ngừng sử dụng
    /// </summary>
    OutOfUse,

    /// <summary>
    /// Đang bảo trì
    /// </summary>
    UnderMaintenance,

    /// <summary>
    /// Đã thanh lý
    /// </summary>
    Liquidated,

    /// <summary>
    /// Đã hỏng
    /// </summary>
    Damaged,

    /// <summary>
    /// Đã mất
    /// </summary>
    Lost,

    /// <summary>
    /// Đã huỷ
    /// </summary>
    Destroyed
}

public enum DepreciationUnit
{
    /// <summary>
    /// Tháng
    /// </summary>
    Month,

    /// <summary>
    /// Năm
    /// </summary>
    Year
}

[Table("sm_Asset")]
public class sm_Asset : BaseTableService<sm_Asset>
{
    /// <value>ID</value>
    [Key]
    public Guid Id { get; set; }

    /// <value>Mã tài sản</value>
    [Required]
    public string Code { get; set; }

    /// <value>Tên tài sản</value>
    [Required, MaxLength(100)]
    public string Name { get; set; }

    /// <value>ID loại tài sản</value>
    [Required]
    public Guid AssetTypeId { get; set; }

    /// <summary>
    /// Navigation property to sm_AssetType
    /// </summary>
    [ForeignKey(nameof(AssetTypeId))]
    public sm_AssetType AssetType { get; set; }

    /// <value>ID vị trí tài sản</value>
    [Required]
    public Guid AssetLocationId { get; set; }

    /// <summary>
    /// Navigation property to sm_AssetLocation
    /// </summary>
    [ForeignKey(nameof(AssetLocationId))]
    public sm_AssetLocation AssetLocation { get; set; }

    /// <value>ID người sử dụng</value>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Navigation property to idm_User
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public idm_User User { get; set; }

    /// <value>ID đơn vị tính</value>
    public Guid? MeasureUnitId { get; set; }

    /// <summary>
    /// Navigation property to sm_MeasureUnit
    /// </summary>
    [ForeignKey(nameof(MeasureUnitId))]
    public sm_MeasureUnit MeasureUnit { get; set; }

    /// <value>Mô tả</value>
    [MaxLength(255)]
    public string Description { get; set; }

    /// <value>Ngày mua</value>
    public DateTime? PurchasedDate { get; set; }

    /// <value>Trạng thái</value>
    public AssetStatus Status { get; set; }

    /// <value>Số serial</value>
    [MaxLength(100)]
    public string Serial { get; set; }

    /// <value>Nguyên giá</value>
    public decimal OriginalPrice { get; set; }

    /// <value>Hãng sản xuất</value>
    [MaxLength(100)]
    public string OriginBrand { get; set; }

    /// <value>Ngày sản xuất</value>
    public DateTime? ManufactureDate { get; set; }

    /// <value>Giá trị khấu hao</value>
    public decimal? DepreciationValue { get; set; }

    /// <value>Thời gian khấu hao</value>
    public int? DepreciationTime { get; set; }

    /// <value>Đơn vị khấu hao (tháng/năm)</value>
    [Required]
    public DepreciationUnit DepreciationUnit { get; set; } = DepreciationUnit.Month;

    /// <value>Ngày bắt đầu khấu hao</value>
    public DateTime? DepreciationStartDate { get; set; }

    /// <value>Khấu hao luỹ kế</value>
    public decimal? AccumulatedDepreciation { get; set; }

    /// <value>Giá trị còn lại</value>
    public decimal? RemainingValue { get; set; }

    /// <value>Danh sách hình ảnh</value>
    [Column(TypeName = "jsonb")]
    public List<jsonb_Attachment> Images { get; set; } = new();

    /// <value>Danh sách tài liệu</value>
    [Column(TypeName = "jsonb")]
    public List<jsonb_Attachment> Documents { get; set; } = new();

    [ForeignKey(nameof(CreatedByUserId))]
    public idm_User CreatedByUser { get; set; }

    [ForeignKey(nameof(LastModifiedByUserId))]
    public idm_User LastModifiedByUser { get; set; }

    public virtual ICollection<sm_AssetMaintenanceSheet> AssetMaintenanceSheets { get; set; }
    public virtual ICollection<sm_AssetUsageHistory> AssetUsageHistories { get; set; }
}