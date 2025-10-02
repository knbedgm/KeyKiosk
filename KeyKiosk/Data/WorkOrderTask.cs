namespace KeyKiosk.Data
{
    public class WorkOrderTask
    {
        public int Id { get; set; }

        // Description of the task (instead of another CustomerName)
        public required string Description { get; set; }

        public int CostCents { get; set; }
        public WorkOrderTaskStatusType Status { get; set; }

        // Foreign key to WorkOrder
        public int WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; } = null!;
    }

    public enum WorkOrderTaskStatusType
    {
        Created,
        WorkStarted,
        WorkFinished,
        Completed,
    }
}
