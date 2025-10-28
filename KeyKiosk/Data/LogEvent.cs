namespace KeyKiosk.Data
{
    /// <summary>
    /// model representing a work order log event for internal use or UI binding.
    /// Usage: useful for application code or tests that require a normalized event shape; 
    /// convert from WorkOrderEventDto or WorkOrderLogsRaw as needed.
    /// </summary>
    public class LogEvent
    {
        public int ID { get; set; }
        public DateTimeOffset DateTime { get; set; } = DateTimeOffset.UtcNow;
        public int? UserId { get; set; }
        public string UserName { get; set; } = "";
        public LogEventType EventType { get; set; }
        public int WorkOrderId { get; set; }
        public string? WorkOrderDetails { get; set; }
        public string? WorkOrderVehiclePlate { get; set; }
        public string? WorkOrderStatus { get; set; }
        public int? TaskId { get; set; }
        public string? TaskTitle { get; set; }
        public int? CostCents { get; set; }
        public string? PayloadJson { get; set; }
        public enum LogEventType { Created, StatusChanged, DetailsChanged, TaskAdded, TaskRemoved, TaskStatusChanged, TaskDetailsChanged }
    }
}
