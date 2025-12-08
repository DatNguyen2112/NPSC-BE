namespace NSPC.Business;

public class ActivityHistoryViewModel
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public string EntityType { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }
    public Guid CreatedByUserId { get; set; }
    public Guid? LastModifiedByUserId { get; set; }
    public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
    public DateTime CreatedOnDate { get; set; } = DateTime.Now;
    public string CreatedByUserName { get; set; }
    public string CreatedByUserFullName { get; set; }
    public string LastModifiedByUserName { get; set; }
    public string LastModifiedByFullName { get; set; }
}