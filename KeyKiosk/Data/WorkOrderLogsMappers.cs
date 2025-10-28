namespace KeyKiosk.Data
{
    /// <summary>
    /// Helper methods that convert database raw rows into application DTOs used by UI and API layers.
    /// WorkOrderLogsRaw database record into a WorkOrderEventDto ready for presentation or API return.
    /// Usage: called by WorkOrderLogService after retrieving WorkOrderLogsRaw rows to produce DTO lists for controllers or Blazor pages.
    /// </summary>
    public static class WorkOrderLogsMappers
    {
        public static WorkOrderEventDto ToWorkOrderEventDto(this WorkOrderLogsRaw r)
        {
            return new WorkOrderEventDto
            {
                ID = r.ID,
                DateTime = r.DateTime,
                UserId = r.UserId,
                UserName = r.UserName,
                EventType = r.EventType ?? "",
                WorkOrderId = r.workOrderId ?? 0,
                WorkOrderDetails = r.Details,
                WorkOrderVehiclePlate = r.VehiclePlate,
                WorkOrderStatus = r.Status,
                TaskId = r.TaskId,
                TaskTitle = r.TaskDetailsChangedEvent_Details, // best-effort mapping; adjust as needed
                CostCents = r.CostCents,
                Payload = null
            };
        }
    }
}
