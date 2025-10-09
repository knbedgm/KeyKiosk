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
    }
}
