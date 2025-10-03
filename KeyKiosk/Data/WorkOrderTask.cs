namespace KeyKiosk.Data;

public class WorkOrderTask
{
	public int Id { get; set; }
	public string Description { get; set; } = "";
	public DateTimeOffset StartDate { get; set; }
	public DateTimeOffset EndDate { get; set; }
	public WorkOrderTaskStatusType Status { get; set; }
	public int CostCents { get; set; }

	public int WorkOrderId { get; set; }
	public virtual WorkOrder WorkOrder { get; set; }
}
public enum WorkOrderTaskStatusType
{
	WorkStarted,
	WorkFinished,
}
