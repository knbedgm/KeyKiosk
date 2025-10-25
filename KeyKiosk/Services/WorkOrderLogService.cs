using KeyKiosk.Components;
using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace KeyKiosk.Services
{
    public class WorkOrderLogService
    {
        /// <summary>
        /// Contructor, service crteated that injects depenedency injection and use in quieries are stored  
        /// </summary> 
        private readonly ApplicationDbContext _context;

        public WorkOrderLogService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<WorkOrderEventDto>> GetFilteredLogDtosAsync(
        string? username = null,
        int? workOrderId = null,
        string? status = null,
        string? vehiclePlate = null,
        string? search = null,
        LogEvent.LogEventType? eventType = null,
        int page = 1,
        int pageSize = 25)

        {
            var query = _context.Set<LogEvent>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(e => EF.Functions.ILike(e.UserName ?? string.Empty, $"%{username}%"));

            if (workOrderId.HasValue)
                query = query.Where(e => e.WorkOrderId == workOrderId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => EF.Functions.ILike(e.WorkOrderStatus ?? string.Empty, $"%{status}%"));

            if (!string.IsNullOrWhiteSpace(vehiclePlate))
                query = query.Where(e => EF.Functions.ILike(e.WorkOrderVehiclePlate ?? string.Empty, $"%{vehiclePlate}%"));

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e =>
                    EF.Functions.ILike(e.UserName ?? string.Empty, $"%{search}%") ||
                    EF.Functions.ILike(e.WorkOrderDetails ?? string.Empty, $"%{search}%") ||
                    EF.Functions.ILike(e.TaskTitle ?? string.Empty, $"%{search}%"));

            if (eventType.HasValue)
                query = query.Where(e => e.EventType == eventType.Value);

            var skip = (page - 1) * pageSize;

            var rows = await query.OrderByDescending(e => e.DateTime).Skip(skip).Take(pageSize).ToListAsync();

            return rows.Select(e => new WorkOrderEventDto
            {
                ID = e.ID,
                DateTime = e.DateTime,
                UserId = e.UserId,
                UserName = e.UserName,
                EventType = e.EventType.ToString(),
                WorkOrderId = e.WorkOrderId,
                WorkOrderDetails = e.WorkOrderDetails,
                WorkOrderVehiclePlate = e.WorkOrderVehiclePlate,
                WorkOrderStatus = e.WorkOrderStatus,
                TaskId = e.TaskId,
                TaskTitle = e.TaskTitle,
                CostCents = e.CostCents,
                Payload = string.IsNullOrEmpty(e.PayloadJson) ? null : JsonSerializer.Deserialize<object>(e.PayloadJson)
            }).ToList();
        }

        /*
        /// <summary>
        /// Retrieves a filtered list of WorkOrderLogEvent objects from the database.Inputs (all optional):
        /// such as username, work order ID, status, vehicle plate, search term, and event type.  
        /// All parameters are optional. If no parameters are provided, the method returns all logs.
        /// Parameters can be combined(e.g., filter by both username and status).
        /// Designed for UI filtering in Razor pages.
        /// </summary>
        public async Task<List<WorkOrderLogEvent>> GetFilteredLogsAsync(
            string? username = null, //Filters logs by matching part of the username.
            int? workOrderId = null, //Filters logs by a specific Work Order ID.
            string? status = null, //Filters logs by matching the work order status.
            string? vehiclePlate = null, //Filters logs by vehicle plate
            string? search = null, //Searches across username and work order details.
            WorkOrderLogEvent.WorkOrderLogEventType? eventType = null) //Filters logs by event type.
        {
            var query = _context.Set<WorkOrderLogEvent>()
                                .Include(e => e.workOrder)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(e => e.UserName.Contains(username));

            if (workOrderId.HasValue)
                query = query.Where(e => e.workOrder.Id == workOrderId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.workOrder.Status.ToString().Contains(status));

            if (!string.IsNullOrWhiteSpace(vehiclePlate))
                query = query.Where(e =>
                    EF.Functions.ILike(e.workOrder.VehiclePlate, $"%{vehiclePlate}%"));

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e =>
                    EF.Functions.ILike(e.UserName, $"%{search}%") ||
                    EF.Functions.ILike(e.workOrder.Details, $"%{search}%"));

            if (eventType.HasValue)
                query = query.Where(e => e.EventType == eventType.Value);

            return await query.OrderByDescending(e => e.DateTime).ToListAsync(); //orders logs  by the newest first
        }
        */

        /// <summary>
        /// Method reuses the filter logix to export logs as a CSV string filtered by the provided parameters
        ///Intended for data export to CSV/Excel.
        ///Reuses the same filtering logic as GetFilteredLogsAsync.
        ///Can be combined with the JavaScript helper(Site.js) to trigger a file download in the browser.
        /// Inputs(all optional, same as GetFilteredLogsAsync) 
        /// </summary>
        public async Task<string> ExportLogsToCsvAsync(
        string? username = null,
        int? workOrderId = null,
        string? status = null,
        string? vehiclePlate = null,
        string? search = null,
        LogEvent.LogEventType? eventType = null)

        {
            var logs = await GetFilteredLogDtosAsync(username, workOrderId, status, vehiclePlate, search, eventType);

            var sb = new StringBuilder();
            sb.AppendLine("ID,DateTime,UserName,EventType,WorkOrderId,Status,VehiclePlate,TaskId,TaskTitle,CostCents,Details");

            foreach (var log in logs)
            {
                string Safe(string? s) => s is null ? "" : $"\"{s.Replace("\"", "\"\"")}\"";
                sb.AppendLine(string.Join(",", new[]
                {
                    log.ID.ToString(),
                    log.DateTime.ToString("o"),
                    Safe(log.UserName),
                    Safe(log.EventType),
                    log.WorkOrderId.ToString(),
                    Safe(log.WorkOrderStatus),
                    Safe(log.WorkOrderVehiclePlate),
                    log.TaskId?.ToString() ?? "",
                    Safe(log.TaskTitle),
                    log.CostCents?.ToString() ?? "",
                    Safe(log.WorkOrderDetails)
                }));
            }
            return sb.ToString(); //returns the CSV as a string
        }

        //
        // Creation helpers: create and persist LogEvent records.
        // Note: these helpers call SaveChangesAsync for simplicity.
        // If you prefer a single SaveChanges in the caller, change Add* to only _context.Add(evt).
        //
        public async Task AddCreateEventAsync(WorkOrder workOrder, int? userId = null, string? userName = null)
        {
            var evt = new LogEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                UserId = userId,
                UserName = userName ?? "system",
                EventType = LogEvent.LogEventType.Created,
                WorkOrderId = workOrder.Id,
                WorkOrderDetails = workOrder.Details,
                WorkOrderVehiclePlate = workOrder.VehiclePlate,
                WorkOrderStatus = workOrder.Status.ToString()
            };

            _context.LogEvents.Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddStatusChangedEventAsync(WorkOrder workOrder, string oldStatus, string newStatus, int? userId = null, string? userName = null)
        {
            var payload = new { Old = oldStatus, New = newStatus };
            var evt = new LogEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                UserId = userId,
                UserName = userName ?? "system",
                EventType = LogEvent.LogEventType.StatusChanged,
                WorkOrderId = workOrder.Id,
                WorkOrderDetails = workOrder.Details,
                WorkOrderVehiclePlate = workOrder.VehiclePlate,
                WorkOrderStatus = workOrder.Status.ToString(),
                PayloadJson = JsonSerializer.Serialize(payload)
            };

            _context.LogEvents.Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddDetailsChangedEventAsync(WorkOrder workOrder, string? oldDetails, string? newDetails, int? userId = null, string? userName = null)
        {
            var payload = new { Old = oldDetails, New = newDetails };
            var evt = new LogEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                UserId = userId,
                UserName = userName ?? "system",
                EventType = LogEvent.LogEventType.DetailsChanged,
                WorkOrderId = workOrder.Id,
                WorkOrderDetails = workOrder.Details,
                WorkOrderVehiclePlate = workOrder.VehiclePlate,
                WorkOrderStatus = workOrder.Status.ToString(),
                PayloadJson = JsonSerializer.Serialize(payload)
            };

            _context.LogEvents.Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddTaskAddedEventAsync(WorkOrder workOrder, WorkOrderTask task, int? userId = null, string? userName = null)
        {
            var evt = new LogEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                UserId = userId,
                UserName = userName ?? "system",
                EventType = LogEvent.LogEventType.TaskAdded,
                WorkOrderId = workOrder.Id,
                WorkOrderDetails = workOrder.Details,
                WorkOrderVehiclePlate = workOrder.VehiclePlate,
                WorkOrderStatus = workOrder.Status.ToString(),
                TaskId = task.Id,
                TaskTitle = task.Title,
                CostCents = task.CostCents
            };

            _context.LogEvents.Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddTaskRemovedEventAsync(WorkOrder workOrder, WorkOrderTask task, int? userId = null, string? userName = null)
        {
            var payload = new { RemovedTaskId = task.Id, RemovedTaskTitle = task.Title, RemovedCost = task.CostCents };
            var evt = new LogEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                UserId = userId,
                UserName = userName ?? "system",
                EventType = LogEvent.LogEventType.TaskRemoved,
                WorkOrderId = workOrder.Id,
                WorkOrderDetails = workOrder.Details,
                WorkOrderVehiclePlate = workOrder.VehiclePlate,
                WorkOrderStatus = workOrder.Status.ToString(),
                TaskId = task.Id,
                TaskTitle = task.Title,
                CostCents = task.CostCents,
                PayloadJson = JsonSerializer.Serialize(payload)
            };

            _context.LogEvents.Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddTaskDetailsChangedEventAsync(WorkOrder workOrder, WorkOrderTask task, object oldDetails, object newDetails, int? userId = null, string? userName = null)
        {
            var payload = new { Old = oldDetails, New = newDetails };
            var evt = new LogEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                UserId = userId,
                UserName = userName ?? "system",
                EventType = LogEvent.LogEventType.TaskDetailsChanged,
                WorkOrderId = workOrder.Id,
                WorkOrderDetails = workOrder.Details,
                WorkOrderVehiclePlate = workOrder.VehiclePlate,
                WorkOrderStatus = workOrder.Status.ToString(),
                TaskId = task.Id,
                TaskTitle = task.Title,
                CostCents = task.CostCents,
                PayloadJson = JsonSerializer.Serialize(payload)
            };

            _context.LogEvents.Add(evt);
            await _context.SaveChangesAsync();
        }
    }
}
