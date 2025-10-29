using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace KeyKiosk.Services
{
    public class WorkOrderLogService
    {
        private readonly ApplicationDbContext _context;

        public WorkOrderLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        // -------------------------------
        // WRITE METHODS 
        // -------------------------------
        public async Task LogCreatedAsync(WorkOrder workOrder, User user)
        {
            // Ensure EF treats WorkOrder as existing
            _context.Attach(workOrder);
            _context.Attach(user);

            var log = new WorkOrderLogEvent.CreateEvent
            {
                User = user,
                UserId = user.Id,
                UserName = user.Name,
                workOrder = workOrder,
                DateTime = DateTimeOffset.Now
            };

            _context.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogStatusChangedAsync(WorkOrder workOrder, User user)
        {
            _context.Attach(workOrder);
            _context.Attach(user);

            var log = new WorkOrderLogEvent.StatusChangedEvent
            {
                User = user,
                UserId = user.Id,
                UserName = user.Name,
                workOrder = workOrder,
                Status = workOrder.Status,
                DateTime = DateTimeOffset.Now
            };

            _context.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogDetailsChangedAsync(WorkOrder workOrder, User user)
        {
            _context.Attach(workOrder);
            _context.Attach(user);

            var log = new WorkOrderLogEvent.DetailsChangedEvent
            {
                User = user,
                UserId = user.Id,
                UserName = user.Name,
                workOrder = workOrder,
                CustomerName = workOrder.CustomerName,
                VehiclePlate = workOrder.VehiclePlate,
                Details = workOrder.Details,
                DateTime = DateTimeOffset.Now
            };

            _context.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogTaskAddedAsync(WorkOrderTask task, User user)
        {
            // Attach parent entities first so EF doesn't try to insert them
            if (task.WorkOrder != null) _context.Attach(task.WorkOrder);
            _context.Attach(task);
            _context.Attach(user);

            var log = new WorkOrderLogEvent.TaskAddedEvent
            {
                User = user,
                UserId = user.Id,
                UserName = user.Name,
                workOrder = task.WorkOrder!,
                Task = task,
                DateTime = DateTimeOffset.Now
            };

            _context.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogTaskRemovedAsync(WorkOrderTask task, User user)
        {
            if (task.WorkOrder != null) _context.Attach(task.WorkOrder);
            _context.Attach(task);
            _context.Attach(user);

            var log = new WorkOrderLogEvent.TaskRemovedEvent
            {
                User = user,
                UserId = user.Id,
                UserName = user.Name,
                workOrder = task.WorkOrder!,
                Task = task,
                DateTime = DateTimeOffset.Now
            };

            _context.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogTaskStatusChangedAsync(WorkOrderTask task, User user)
        {
            if (task.WorkOrder != null) _context.Attach(task.WorkOrder);
            _context.Attach(task);
            _context.Attach(user);

            var log = new WorkOrderLogEvent.TaskStatusChangedEvent
            {
                User = user,
                UserId = user.Id,
                UserName = user.Name,
                workOrder = task.WorkOrder!,
                Task = task,
                Status = task.Status,
                DateTime = DateTimeOffset.Now
            };

            _context.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogTaskDetailsChangedAsync(WorkOrderTask task, User user)
        {
            if (task.WorkOrder != null) _context.Attach(task.WorkOrder);
            _context.Attach(task);
            _context.Attach(user);

            var log = new WorkOrderLogEvent.TaskDetailsChangedEvent
            {
                User = user,
                UserId = user.Id,
                UserName = user.Name,
                workOrder = task.WorkOrder!,
                Task = task,
                Details = task.Details,
                CostCents = task.CostCents,
                DateTime = DateTimeOffset.Now
            };

            _context.Add(log);
            await _context.SaveChangesAsync();
        }

        // -------------------------------
        // READ / EXPORT METHODS (unchanged)
        // -------------------------------
        public async Task<List<WorkOrderLogEvent>> GetFilteredLogsAsync(
            string? username = null,
            int? workOrderId = null,
            string? status = null,
            string? vehiclePlate = null,
            string? search = null,
            WorkOrderLogEvent.WorkOrderLogEventType? eventType = null)
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

            return await query.OrderByDescending(e => e.DateTime).ToListAsync();
        }

        public async Task<string> ExportLogsToCsvAsync(
            string? username = null,
            int? workOrderId = null,
            string? status = null,
            string? vehiclePlate = null,
            string? search = null,
            WorkOrderLogEvent.WorkOrderLogEventType? eventType = null)
        {
            var logs = await GetFilteredLogsAsync(username, workOrderId, status, vehiclePlate, search, eventType);

            var sb = new StringBuilder();
            sb.AppendLine("ID,DateTime,UserName,EventType,WorkOrderId,Status,VehiclePlate,Details");

            foreach (var log in logs)
            {
                sb.AppendLine($"{log.ID},{log.DateTime},{log.UserName},{log.EventType},{log.workOrder.Id},{log.workOrder.Status},{log.workOrder.VehiclePlate},{log.workOrder.Details}");
            }

            return sb.ToString();
        }

        public async Task<List<WorkOrderLogEvent>> GetWorkOrderLogsByUsernameDatePeriod(string username, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _context.WorkOrderLog
                                   .Where(l => l.UserName == username && l.DateTime >= startDate && l.DateTime <= endDate)
                                   .Include(l => l.workOrder)
                                   .ToListAsync();
        }
    }
}
