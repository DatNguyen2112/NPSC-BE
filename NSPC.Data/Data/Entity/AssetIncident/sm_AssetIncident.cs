using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NSPC.Data.Data.Entity.Asset;
using NSPC.Data.Data.Entity.AssetHistory;

namespace NSPC.Data.Data.Entity.AssetIncident;

[Table("sm_AssetIncident")]
public class sm_AssetIncident : BaseTableService<sm_AssetIncident>
{
    /// <value>ID</value>
    [Key]
    public Guid Id { get; set; }

    /// <value>ID tài sản</value>
    [Required]
    public Guid AssetId { get; set; }

    /// <summary>
    /// Navigation property to sm_Asset
    /// </summary>
    [ForeignKey(nameof(AssetId))]
    public sm_Asset Asset { get; set; }

    /// <value>Loại sự cố</value>
    [Required]
    public AssetBusinessOperation IncidentType { get; set; }

    /// <value>Ngày xảy ra sự cố</value>
    [Required]
    public DateTime IncidentDate { get; set; }

    /// <value>Giá trị đền bù</value>
    public decimal? CompensationAmount { get; set; }

    /// <value>Ghi chú</value>
    [MaxLength(255)]
    public string Description { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public idm_User CreatedByUser { get; set; }

    [ForeignKey(nameof(LastModifiedByUserId))]
    public idm_User LastModifiedByUser { get; set; }
}