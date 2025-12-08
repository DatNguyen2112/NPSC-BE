using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data.Data.Entity.Asset;

[Table("sm_AssetLiquidationSheet")]
public class sm_AssetLiquidationSheet : BaseTableService<sm_AssetLiquidationSheet>
{
    /// <value>ID</value>
    [Key]
    public Guid Id { get; set; }

    /// <value>Mã phiếu thanh lý</value>
    [Required]
    public string Code { get; set; }

    /// <value>ID tài sản</value>
    [Required]
    public Guid AssetId { get; set; }

    /// <summary>
    /// Navigation property to sm_Asset
    /// </summary>
    [ForeignKey(nameof(AssetId))]
    public sm_Asset Asset { get; set; }

    /// <value>Ngày thanh lý</value>
    [Required]
    public DateTime LiquidationDate { get; set; }

    /// <value>Số quyết định</value>
    [MaxLength(100)]
    public string DecisionNumber { get; set; }

    /// <value>ID người thực hiện thanh lý</value>
    public Guid? LiquidatorId { get; set; }

    /// <summary>
    /// Navigation property to idm_User
    /// </summary>
    [ForeignKey(nameof(LiquidatorId))]
    public idm_User Liquidator { get; set; }

    /// <value>Giá trị thanh lý</value>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? LiquidationValue { get; set; }

    /// <value>Lý do thanh lý</value>
    [MaxLength(500)]
    public string LiquidationReason { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public idm_User CreatedByUser { get; set; }

    [ForeignKey(nameof(LastModifiedByUserId))]
    public idm_User LastModifiedByUser { get; set; }
}