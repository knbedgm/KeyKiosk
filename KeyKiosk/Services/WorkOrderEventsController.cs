using KeyKiosk.Components;
using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///API controller that exposes endpoints for retrieving work order events for a single work order.
/// GET api/workorders/{workOrderId}/events.
/// attempts to parse the incoming eventType string into the enum WorkOrderLogEvent.
/// Which alllows WorkOrderLogEventType to pass the parsed enum or null to the service.
/// calls WorkOrderLogService.GetFilteredLogDtosAsync with the parsed filters and returns the service result as the list of DTOs.
/// </summary>
[ApiController]
[Route("api/workorders/{workOrderId}/events")]
public class WorkOrderEventsController : ControllerBase
{
    private readonly WorkOrderLogService _logService;
    public WorkOrderEventsController(WorkOrderLogService logService) { _logService = logService; }

    [HttpGet]
    public async Task<IActionResult> GetEvents(int workOrderId, int page = 1, int pageSize = 25, string? search = null, string? eventType = null)
    {
        WorkOrderLogEvent.WorkOrderLogEventType? parsed = null;
        if (!string.IsNullOrEmpty(eventType)
            && Enum.TryParse<WorkOrderLogEvent.WorkOrderLogEventType>(eventType, ignoreCase: true, out var t))
        {
            parsed = t;
        }

        var dtos = await _logService.GetFilteredLogDtosAsync(
            workOrderId: workOrderId,
            search: search,
            eventType: parsed,
            page: page,
            pageSize: pageSize);

        return Ok(dtos);
    }
}
