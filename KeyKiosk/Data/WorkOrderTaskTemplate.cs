namespace KeyKiosk.Data;

public class WorkOrderTaskTemplate
{
    public int Id { get; set; }
    public string TaskTitle { get; set; } = "";
    public string TaskDetails { get; set; } = "";
    public int TaskCostCents { get; set; }
    public int ExpectedHoursForCompletion { get; set; }
}