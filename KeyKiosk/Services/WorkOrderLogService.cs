using KeyKiosk.Components;
using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

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
            WorkOrderLogEvent.WorkOrderLogEventType? eventType = null)
        {
            var logs = await GetFilteredLogsAsync(username, workOrderId, status, vehiclePlate, search, eventType);

            var sb = new StringBuilder();
            sb.AppendLine("ID,DateTime,UserName,EventType,WorkOrderId,Status,VehiclePlate,Details");

            foreach (var log in logs) //creates a CSV header row
            {
                sb.AppendLine($"{log.ID},{log.DateTime},{log.UserName},{log.EventType},{log.workOrder.Id},{log.workOrder.Status},{log.workOrder.VehiclePlate},{log.workOrder.Details}");
            }

            return sb.ToString(); //returns the CSV as a string
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
