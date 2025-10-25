using Microsoft.AspNetCore.Mvc;

namespace KeyKiosk.Data
{
    // server-side DTO used by the UI and API
    public class WorkOrderEventDto
    {
        public int ID { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string EventType { get; set; } = "";
        public int WorkOrderId { get; set; }
        public string? WorkOrderDetails { get; set; }
        public string? WorkOrderVehiclePlate { get; set; }
        public string? WorkOrderStatus { get; set; }
        public int? TaskId { get; set; }
        public string? TaskTitle { get; set; }
        public int? CostCents { get; set; }
        public object? Payload { get; set; }
    }
}
