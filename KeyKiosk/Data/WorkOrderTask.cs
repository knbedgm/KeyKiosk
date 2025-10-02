namespace KeyKiosk.Data
{
	public class WorkOrderTask
	{
		public int Id { get; set; }
		public required string Description { get; set; }
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
		public WorkOrderTaskStatusType Status { get; set; }
		public int CostCents { get; set; }

        public int WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; } = null!;
    }
	public enum WorkOrderTaskStatusType
	{
		WorkStarted,
		WorkFinished,
	}
}
