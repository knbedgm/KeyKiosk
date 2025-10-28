using KeyKiosk.Components.Pages;
using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;

namespace KeyKiosk.Services;

public class WorkOrderTaskService
{
    public required ApplicationDbContext dbContext { get; set; }

    public WorkOrderTaskService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public List<WorkOrderTask> GetAllTasks()
    {
        return dbContext.WorkOrderTasks
                        .OrderBy(t => t.Id)
                        .ToList();
    }

    public List<WorkOrderTask> GetWorkOrderTasksByWorkOrderId(int workOrderId)
    {
        var workOrder = dbContext.WorkOrders
            .Include(w => w.Tasks)
            .FirstOrDefault(w => w.Id == workOrderId);

        if (workOrder == null || workOrder.Tasks == null)
            return new List<WorkOrderTask>();

        return workOrder.Tasks.OrderBy(t => t.Id).ToList();
    }

    public WorkOrderTask GetWorkOrderTaskById(int taskId)
    {
        return dbContext.WorkOrderTasks
                        .First(t => t.Id == taskId);
    }

    public class AddWorkOrderTaskModel
    {
        public string Title { get; set; } = "";
        public string Details { get; set; } = "";
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public WorkOrderTaskStatusType Status { get; set; }
        public int CostCents { get; set; }
    }

    public void AddWorkOrderTask(int workOrderId, AddWorkOrderTaskModel newTask)
    {
        var workOrder = dbContext.WorkOrders.FirstOrDefault(t => t.Id == workOrderId);
        if (workOrder == null)
            throw new ArgumentException($"Work order with id {workOrderId} doesn't exist", nameof(workOrderId));

        var taskToAdd = new WorkOrderTask
        {
            Title = newTask.Title,
            Details = newTask.Details,
            StartDate = newTask.StartDate,
            EndDate = newTask.EndDate,
            Status = newTask.Status,
            CostCents = newTask.CostCents,
            WorkOrder = workOrder,
        };

        dbContext.WorkOrderTasks.Add(taskToAdd);
        dbContext.SaveChanges();
    }

    public class UpdateWorkOrderTaskModel
    {
        public string TaskTitle { get; set; } = "";
        public string Details { get; set; } = "";
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public WorkOrderTaskStatusType Status { get; set; }
        public int CostCents { get; set; }
    }

    public void UpdateWorkOrderTask(int TaskId, UpdateWorkOrderTaskModel updatedTask)
    {
        var taskToUpdate = dbContext.WorkOrderTasks.FirstOrDefault(t => t.Id == TaskId);
        if (taskToUpdate != null)
        {
            taskToUpdate.Details = updatedTask.Details;
            taskToUpdate.StartDate = updatedTask.StartDate;
            taskToUpdate.EndDate = updatedTask.EndDate;
            taskToUpdate.Status = updatedTask.Status;
            taskToUpdate.CostCents = updatedTask.CostCents;
        }
        dbContext.SaveChanges();
    }

    public void DeleteWorkOrderTask(int idToDelete)
    {
        var taskToDelete = dbContext.WorkOrderTasks.FirstOrDefault(t => t.Id == idToDelete);
        if (taskToDelete != null)
        {
            dbContext.WorkOrderTasks.Remove(taskToDelete);
        }
        dbContext.SaveChanges();
    }

    // Get tasks between two dates
    public async Task<List<WorkOrderTask>> GetTasksByDatePeriod(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await dbContext.WorkOrderTasks
                               .Where(w => w.StartDate.HasValue && w.StartDate.Value >= startDate && w.StartDate.Value <= endDate)
                               .ToListAsync();
    }
}
