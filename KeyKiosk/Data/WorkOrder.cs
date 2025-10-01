namespace KeyKiosk.Data;

public class WorkOrder
{
	public int Id { get; set; }
	public required string CustomerName { get; set; }
	public DateTimeOffset StartDate { get; set; }
	public DateTimeOffset EndDate { get; set; }
	public WorkOrderStatusType Status { get; set; }
	public required string Details { get; set; }
	public required IList<WorkOrderTask> Tasks { get; set; }
	public int TotalCostCents { get => Tasks.Sum(t => t.CostCents); }
}

public enum WorkOrderStatusType
{
	Created,
	WorkStarted,
	WorkFinished,
	Closed,
}
