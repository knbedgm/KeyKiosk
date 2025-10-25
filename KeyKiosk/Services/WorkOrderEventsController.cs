using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/workorders/{workOrderId}/events")]
public class WorkOrderEventsController : ControllerBase
{
    private readonly WorkOrderLogService _logService;
    public WorkOrderEventsController(WorkOrderLogService logService) { _logService = logService; }

    [HttpGet]
    public async Task<IActionResult> GetEvents(int workOrderId, int page = 1, int pageSize = 25, string? search = null, string? eventType = null)
    {
        LogEvent.LogEventType? parsed = null;
        if (!string.IsNullOrEmpty(eventType) && Enum.TryParse<LogEvent.LogEventType>(eventType, out var t)) parsed = t;
        var dtos = await _logService.GetFilteredLogDtosAsync(workOrderId: workOrderId, search: search, eventType: parsed, page: page, pageSize: pageSize);
        return Ok(dtos);
    }
}
