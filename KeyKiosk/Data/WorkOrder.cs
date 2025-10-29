namespace KeyKiosk.Data;

public class WorkOrder
{
	public int Id { get; init; }
	public required string CustomerName { get; set; }
	public required string VehiclePlate { get; set; }
	public DateTimeOffset? StartDate { get; set; }
	public DateTimeOffset? EndDate { get; set; }
	public WorkOrderStatusType Status { get; set; }
	public required string Details { get; set; }
	public virtual required IList<WorkOrderTask> Tasks { get; set; }
	public virtual required IList<WorkOrderPart> Parts { get; set; }
	public int TotalCostCents { get => Tasks.Sum(t => t.CostCents) + Parts.Sum(p => p.CostCents); }
}

public enum WorkOrderStatusType
{
	Created,
	WorkStarted,
	WorkFinished,
	Closed,
}
