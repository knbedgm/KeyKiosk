namespace KeyKiosk.Data;

public class WorkOrderTask
{
	public int Id { get; init; }
	public required string Title { get; init; }
	public required string Details { get; set; }
	public DateTimeOffset? StartDate { get; set; }
	public DateTimeOffset? EndDate { get; set; }
	public WorkOrderTaskStatusType Status { get; set; }
	public int CostCents { get; set; }
	public int HoursForCompletion { get; set; }

    public int WorkOrderId { get; set; }
    public required virtual WorkOrder WorkOrder { get; set; }
}
public enum WorkOrderTaskStatusType
{
    Created,
    WorkStarted,
    WorkFinished,
}