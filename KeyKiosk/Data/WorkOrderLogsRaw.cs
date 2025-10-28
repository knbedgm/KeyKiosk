using System;
using System.ComponentModel.DataAnnotations;

namespace KeyKiosk.Data
{
    /// <summary>
    /// CLR entity that mirrors the existing WorkOrderLog database table for EF Core reads and writes.
    /// Usage: queried by WorkOrderLogService (filters, ordering, paging) and populated by CreateLogAsync/Add*LogAsync writers.
    /// </summary>
    public class WorkOrderLogsRaw
    {
        [Key] public int ID { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public int? workOrderId { get; set; }
        public string? EventType { get; set; }

        public string? CustomerName { get; set; }
        public string? VehiclePlate { get; set; }
        public string? Details { get; set; }
        public string? Status { get; set; }
        public int? CostCents { get; set; }

        // These fields appear in your JSON
        public string? TaskDetailsChangedEvent_Details { get; set; }
        public int? TaskDetailsChangedEvent_TaskId { get; set; }
        public int? TaskId { get; set; }
        public int? TaskRemovedEvent_TaskId { get; set; }
        public string? TaskStatusChangedEvent_Status { get; set; }
        public int? TaskStatusChangedEvent_TaskId { get; set; }
    }
}
