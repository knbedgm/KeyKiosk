using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace KeyKiosk.Services
{
    /// <summary>
    /// Service that encapsulates reading, exporting, and writing work order log events.
    /// Responsibilities: - Reads raw rows from the existing Postgres table mapped by ApplicationDbContext (WorkOrderLogsRaw / WorkOrderLogRaw) 
    /// and projects them to WorkOrderEventDto for UI/API consumers via the WorkOrderLogsMappers.ToWorkOrderEventDto mapper.
    /// </summary>
    public class WorkOrderLogService
    {
        private readonly ApplicationDbContext _context;

        public WorkOrderLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Resolve or attach a User from id/name so required navigation can be set
        private async Task<User> ResolveOrAttachUserAsync(int? userId, string? userName)
        {
            if (userId.HasValue)
            {
                var existing = await _context.Users.FindAsync(userId.Value);
                if (existing != null) return existing;

                var stub = new User { Id = userId.Value, Name = userName ?? "unknown" };
                _context.Users.Attach(stub);
                return stub;
            }

            return new User { Name = userName ?? "system" };
        }

        /// <summary>
        /// Return a paged, filtered list of WorkOrderEventDto mapped from WorkOrderLogsRaw rows. 
        /// Purpose: read WorkOrderLogsRaw rows, apply filters 
        /// (username, workOrderId, status, vehiclePlate, free-text search, optional enum eventType),
        /// paginate, and map to DTOs for UI or API.
        /// </summary>
        public async Task<List<WorkOrderEventDto>> GetFilteredLogDtosAsync(
            string? username = null,
            int? workOrderId = null,
            string? status = null,
            string? vehiclePlate = null,
            string? search = null,
            WorkOrderLogEvent.WorkOrderLogEventType? eventType = null,
            int page = 1,
            int pageSize = 25)
        {
            var query = _context.WorkOrderLogsRaw.AsQueryable();

            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(e => EF.Functions.ILike(e.UserName ?? string.Empty, $"%{username}%"));

            if (workOrderId.HasValue)
                query = query.Where(e => e.workOrderId == workOrderId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => EF.Functions.ILike(e.Status ?? string.Empty, $"%{status}%"));

            if (!string.IsNullOrWhiteSpace(vehiclePlate))
                query = query.Where(e => EF.Functions.ILike(e.VehiclePlate ?? string.Empty, $"%{vehiclePlate}%"));

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    EF.Functions.ILike(e.UserName ?? string.Empty, $"%{search}%")
                    || EF.Functions.ILike(e.Details ?? string.Empty, $"%{search}%")
                    || EF.Functions.ILike(e.CustomerName ?? string.Empty, $"%{search}%")
                    || EF.Functions.ILike(e.TaskDetailsChangedEvent_Details ?? string.Empty, $"%{search}%"));
            }

            if (eventType.HasValue)
            {
                var et = eventType.Value.ToString();
                query = query.Where(e => EF.Functions.ILike(e.EventType ?? string.Empty, $"%{et}%"));
            }

            var skip = (page - 1) * pageSize;

            var rows = await query
                .OrderByDescending(e => e.DateTime)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return rows.Select(r => r.ToWorkOrderEventDto()).ToList();
        }

        /// <summary>
        /// Export filtered events to a CSV string for download.
        /// Mirrors GetFilteredLogDtosAsync filters, materializes matching rows, then builds a CSV with safe quoting for string fields.
        /// </summary>
        public async Task<string> ExportLogsToCsvAsync(
            string? username = null,
            int? workOrderId = null,
            string? status = null,
            string? vehiclePlate = null,
            string? search = null,
            WorkOrderLogEvent.WorkOrderLogEventType? eventType = null)
        {
            var query = _context.WorkOrderLogsRaw.AsQueryable();

            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(e => EF.Functions.ILike(e.UserName ?? string.Empty, $"%{username}%"));

            if (workOrderId.HasValue)
                query = query.Where(e => e.workOrderId == workOrderId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => EF.Functions.ILike(e.Status ?? string.Empty, $"%{status}%"));

            if (!string.IsNullOrWhiteSpace(vehiclePlate))
                query = query.Where(e => EF.Functions.ILike(e.VehiclePlate ?? string.Empty, $"%{vehiclePlate}%"));

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    EF.Functions.ILike(e.UserName ?? string.Empty, $"%{search}%")
                    || EF.Functions.ILike(e.Details ?? string.Empty, $"%{search}%")
                    || EF.Functions.ILike(e.CustomerName ?? string.Empty, $"%{search}%")
                    || EF.Functions.ILike(e.TaskDetailsChangedEvent_Details ?? string.Empty, $"%{search}%"));
            }

            if (eventType.HasValue)
            {
                var et = eventType.Value.ToString();
                query = query.Where(e => EF.Functions.ILike(e.EventType ?? string.Empty, $"%{et}%"));
            }

            var rows = await query.OrderByDescending(e => e.DateTime).ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("ID,DateTime,UserName,workOrderId,EventType,CustomerName,VehiclePlate,Details,Status,TaskId,TaskDetails");

            foreach (var e in rows)
            {
                string Safe(string? s) => s is null ? "" : $"\"{s.Replace("\"", "\"\"")}\"";
                sb.AppendLine(string.Join(",",
                    e.ID.ToString(),
                    e.DateTime.ToString("o"),
                    Safe(e.UserName),
                    e.workOrderId?.ToString() ?? "",
                    Safe(e.EventType),
                    Safe(e.CustomerName),
                    Safe(e.VehiclePlate),
                    Safe(e.Details),
                    Safe(e.Status),
                    e.TaskId?.ToString() ?? "",
                    Safe(e.TaskDetailsChangedEvent_Details)
                ));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Insert a WorkOrderLogsRaw row and convenience wrappers for common event types (TaskAdded, TaskRemoved, TaskDetailsChanged, StatusChanged).
        /// inserts the provided WorkOrderLogsRaw and SaveChangesAsync.
        /// </summary>
        public async Task CreateLogAsync(WorkOrderLogsRaw row)
        {
            if (row.ID == 0) row.ID = 0; // ID is DB-managed if identity; keep as-is
            if (row.DateTime == default) row.DateTime = DateTimeOffset.UtcNow;

            _context.WorkOrderLogsRaw.Add(row);
            await _context.SaveChangesAsync();
        }

        // TaskAdded event writer into WorkOrderLog
        public async Task AddTaskAddedLogAsync(
            WorkOrder workOrder,
            WorkOrderTask task,
            int? userId = null,
            string? userName = null)
        {
            var row = new WorkOrderLogsRaw
            {
                DateTime = DateTimeOffset.UtcNow,
                UserId = userId,
                UserName = userName ?? "system",
                workOrderId = workOrder.Id,
                EventType = WorkOrderLogEvent.WorkOrderLogEventType.TaskAdded.ToString(),
                CustomerName = null,
                VehiclePlate = workOrder.VehiclePlate,
                Details = workOrder.Details,
                TaskId = task?.Id,
                TaskDetailsChangedEvent_Details = task?.Title
            };

            await CreateLogAsync(row);
        }

        // TaskRemoved event writer into WorkOrderLog
        public async Task AddTaskRemovedLogAsync(
            WorkOrder workOrder,
            WorkOrderTask task,
            int? userId = null,
            string? userName = null)
        {
            var row = new WorkOrderLogsRaw
            {
                DateTime = DateTimeOffset.UtcNow,
                UserId = userId,
                UserName = userName ?? "system",
                workOrderId = workOrder.Id,
                EventType = WorkOrderLogEvent.WorkOrderLogEventType.TaskRemoved.ToString(),
                TaskId = task?.Id,
                TaskDetailsChangedEvent_Details = task?.Title
            };

            await CreateLogAsync(row);
        }

        // TaskDetailsChanged writer into WorkOrderLog
        public async Task AddTaskDetailsChangedLogAsync(
            WorkOrder workOrder,
            WorkOrderTask task,
            string details,
            int? costCents = null,
            int? userId = null,
            string? userName = null)
        {
            var row = new WorkOrderLogsRaw
            {
                DateTime = DateTimeOffset.UtcNow,
                UserId = userId,
                UserName = userName ?? "system",
                workOrderId = workOrder.Id,
                EventType = WorkOrderLogEvent.WorkOrderLogEventType.TaskDetailsChanged.ToString(),
                Details = details,
                CostCents = costCents,
                TaskId = task?.Id,
                TaskDetailsChangedEvent_Details = details
            };

            await CreateLogAsync(row);
        }

        // StatusChanged writer into WorkOrderLog
        public async Task AddStatusChangedLogAsync(
            WorkOrder workOrder,
            WorkOrderStatusType newStatus,
            int? userId = null,
            string? userName = null)
        {
            var row = new WorkOrderLogsRaw
            {
                DateTime = DateTimeOffset.UtcNow,
                UserId = userId,
                UserName = userName ?? "system",
                workOrderId = workOrder.Id,
                EventType = WorkOrderLogEvent.WorkOrderLogEventType.StatusChanged.ToString(),
                Status = newStatus.ToString()
            };

            await CreateLogAsync(row);
        }

        /// <summary>
        /// TPH Add*EventAsync helpers retained for compatibility with code that writes WorkOrderLogEvent subclasses.
        /// methods still create WorkOrderLogEvent subclass instances and
        /// SaveChanges via EF TPH mapping; keep them until callers are migrated to the new raw-table writers.
        /// </summary>

        public async Task AddCreateEventAsync(WorkOrder workOrder, int? userId = null, string? userName = null)
        {
            var user = await ResolveOrAttachUserAsync(userId, userName);

            var evt = new WorkOrderLogEvent.CreateEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                User = user,
                workOrder = workOrder
            };

            _context.Set<WorkOrderLogEvent>().Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddStatusChangedEventAsync(WorkOrder workOrder, WorkOrderStatusType newStatus, int? userId = null, string? userName = null)
        {
            var user = await ResolveOrAttachUserAsync(userId, userName);

            var evt = new WorkOrderLogEvent.StatusChangedEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                User = user,
                workOrder = workOrder,
                Status = newStatus
            };

            _context.Set<WorkOrderLogEvent>().Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddDetailsChangedEventAsync(WorkOrder workOrder, string customerName, string vehiclePlate, string details, int? userId = null, string? userName = null)
        {
            var user = await ResolveOrAttachUserAsync(userId, userName);

            var evt = new WorkOrderLogEvent.DetailsChangedEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                User = user,
                workOrder = workOrder,
                CustomerName = customerName,
                VehiclePlate = vehiclePlate,
                Details = details
            };

            _context.Set<WorkOrderLogEvent>().Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddTaskAddedEventAsync(WorkOrder workOrder, WorkOrderTask task, int? userId = null, string? userName = null)
        {
            var user = await ResolveOrAttachUserAsync(userId, userName);

            var evt = new WorkOrderLogEvent.TaskAddedEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                User = user,
                workOrder = workOrder,
                Task = task
            };

            _context.Set<WorkOrderLogEvent>().Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddTaskRemovedEventAsync(WorkOrder workOrder, WorkOrderTask task, int? userId = null, string? userName = null)
        {
            var user = await ResolveOrAttachUserAsync(userId, userName);

            var evt = new WorkOrderLogEvent.TaskRemovedEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                User = user,
                workOrder = workOrder,
                Task = task
            };

            _context.Set<WorkOrderLogEvent>().Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddTaskStatusChangedEventAsync(WorkOrder workOrder, WorkOrderTask task, WorkOrderTaskStatusType status, int? userId = null, string? userName = null)
        {
            var user = await ResolveOrAttachUserAsync(userId, userName);

            var evt = new WorkOrderLogEvent.TaskStatusChangedEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                User = user,
                workOrder = workOrder,
                Task = task,
                Status = status
            };

            _context.Set<WorkOrderLogEvent>().Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddTaskDetailsChangedEventAsync(WorkOrder workOrder, WorkOrderTask task, string details, int costCents, int? userId = null, string? userName = null)
        {
            var user = await ResolveOrAttachUserAsync(userId, userName);

            var evt = new WorkOrderLogEvent.TaskDetailsChangedEvent
            {
                DateTime = DateTimeOffset.UtcNow,
                User = user,
                workOrder = workOrder,
                Task = task,
                Details = details,
                CostCents = costCents
            };

            _context.Set<WorkOrderLogEvent>().Add(evt);
            await _context.SaveChangesAsync();
        }
    }
}
