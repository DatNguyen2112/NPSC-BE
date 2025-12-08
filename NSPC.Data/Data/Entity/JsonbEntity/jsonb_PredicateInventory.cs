namespace NSPC.Data;

public class jsonb_PredicateInventory
{
    public int LineNo { get; set; } = 0;
    public string Code { get; set; }
    public string Name { get; set; }
    public string Unit { get; set; }
    public decimal PlanQuantity { get; set; } = 0M;
    public Guid ProductId { get; set; }
    public string LineNote { get; set; }
}