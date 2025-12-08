namespace NSPC.Data;

public class jsonb_TemplateStage
{
    public Guid Id { get; set; }
    public int StepOrder { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime? ExpiredDate { get; set; }
    public bool IsDone { get; set; }
}