using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;

namespace KeyKiosk.Services
{
    /// <summary>
    /// Service for WorkOrders and WorkOrderTasks
    /// that automatically writes to the WorkOrderLog.
    /// </summary>
    public class WorkOrderAuditService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly WorkOrderLogService _logService;

        public WorkOrderAuditService(
            ApplicationDbContext dbContext,
            WorkOrderLogService logService)
        {
            _dbContext = dbContext;
            _logService = logService;
        }

        // -------------------------------
        // WorkOrder methods with logging
        // -------------------------------
        public async Task<WorkOrder> AddWorkOrderAsync(WorkOrder workOrder, User currentUser)
        {
            workOrder.Tasks ??= new List<WorkOrderTask>();
            workOrder.Parts ??= new List<WorkOrderPart>();

            _dbContext.WorkOrders.Add(workOrder);
            await _dbContext.SaveChangesAsync();

            await _logService.LogCreatedAsync(workOrder, currentUser);
            return workOrder;
        }

        // Convenience overload for testing
        public async Task<WorkOrder> AddWorkOrderAsync(WorkOrder workOrder)
        {
            var currentUser = await _dbContext.Users.FirstOrDefaultAsync();
            if (currentUser == null)
                throw new InvalidOperationException("No users exist in the database to associate with the log.");

            return await AddWorkOrderAsync(workOrder, currentUser);
        }

        public async Task UpdateWorkOrderAsync(WorkOrder workOrder, User currentUser)
        {
            var tracked = await _dbContext.WorkOrders
                                          .Include(w => w.Tasks)
                                          .Include(w => w.Parts)
                                          .FirstOrDefaultAsync(w => w.Id == workOrder.Id);

            if (tracked == null) return;

            bool statusChanged = tracked.Status != workOrder.Status;
            bool detailsChanged = tracked.CustomerName != workOrder.CustomerName
                               || tracked.VehiclePlate != workOrder.VehiclePlate
                               || tracked.Details != workOrder.Details;

            if (!statusChanged && !detailsChanged)
                return;

            // Update tracked entity
            tracked.CustomerName = workOrder.CustomerName;
            tracked.VehiclePlate = workOrder.VehiclePlate;
            tracked.StartDate = workOrder.StartDate;
            tracked.EndDate = workOrder.EndDate;
            tracked.Status = workOrder.Status;
            tracked.Details = workOrder.Details;

            await _dbContext.SaveChangesAsync();

            // Pass the tracked entity to the log service
            if (statusChanged)
                await _logService.LogStatusChangedAsync(tracked, currentUser);

            if (detailsChanged)
                await _logService.LogDetailsChangedAsync(tracked, currentUser);
        }

        public async Task DeleteWorkOrderAsync(int workOrderId, User currentUser)
        {
            var workOrder = await _dbContext.WorkOrders
                                            .Include(w => w.Tasks)
                                            .Include(w => w.Parts)
                                            .FirstOrDefaultAsync(w => w.Id == workOrderId);
            if (workOrder == null) return;

            _dbContext.WorkOrders.Remove(workOrder);
            await _dbContext.SaveChangesAsync();

            // Optional: implement LogDeletedAsync if you want deletion logs
        }

        // -------------------------------
        // WorkOrderTask methods with logging
        // -------------------------------

        public async Task AddWorkOrderTaskAsync(int workOrderId, WorkOrderTask task, User currentUser)
        {
            var workOrder = await _dbContext.WorkOrders
                                            .Include(w => w.Tasks)
                                            .FirstOrDefaultAsync(w => w.Id == workOrderId);
            if (workOrder == null)
                throw new ArgumentException($"Work order {workOrderId} not found");

            task.WorkOrder = workOrder;
            _dbContext.WorkOrderTasks.Add(task);
            await _dbContext.SaveChangesAsync();

            // Pass the tracked task (with tracked WorkOrder) to the log service
            await _logService.LogTaskAddedAsync(task, currentUser);
        }

        public async Task UpdateWorkOrderTaskAsync(int taskId, WorkOrderTask updatedTask, User currentUser)
        {
            var task = await _dbContext.WorkOrderTasks
                                       .Include(t => t.WorkOrder)
                                       .FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) return;

            bool statusChanged = task.Status != updatedTask.Status;
            bool detailsChanged = task.Details != updatedTask.Details
                               || task.CostCents != updatedTask.CostCents;

            if (!statusChanged && !detailsChanged)
                return;

            task.Details = updatedTask.Details;
            task.StartDate = updatedTask.StartDate;
            task.EndDate = updatedTask.EndDate;
            task.Status = updatedTask.Status;
            task.CostCents = updatedTask.CostCents;

            await _dbContext.SaveChangesAsync();

            if (statusChanged)
                await _logService.LogTaskStatusChangedAsync(task, currentUser);

            if (detailsChanged)
                await _logService.LogTaskDetailsChangedAsync(task, currentUser);
        }

        public async Task DeleteWorkOrderTaskAsync(int taskId, User currentUser)
        {
            var task = await _dbContext.WorkOrderTasks
                                       .Include(t => t.WorkOrder)
                                       .FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) return;

            _dbContext.WorkOrderTasks.Remove(task);
            await _dbContext.SaveChangesAsync();

            await _logService.LogTaskRemovedAsync(task, currentUser);
        }

    }
}
