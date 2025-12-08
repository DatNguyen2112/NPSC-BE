namespace NSPC.Data;

public class jsonb_MaterialRequest
{
    public int LineNo { get; set; } = 0;
    public string Code { get; set; }
    public string Name { get; set; }
    public string Unit { get; set; }
    public decimal MaterialRequestQuantity { get; set; } = 0M;
    public decimal PlanQuantity  { get; set; } = 0M;
    public decimal BalanceQuantity  { get; set; } = 0M;
    public Guid ProductId { get; set; }
    public string LineNote { get; set; }
}