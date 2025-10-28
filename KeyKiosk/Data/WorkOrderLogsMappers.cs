namespace KeyKiosk.Data
{
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
