using KeyKiosk.Data;

namespace KeyKiosk.Services;

public class WorkOrderTaskService
{
    // Set up dbContext
    public required ApplicationDbContext dbContext { get; set; }

    public WorkOrderTaskService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Get all work order tasks
    /// </summary>
    /// <returns></returns>
    public List<WorkOrderTask> GetAllTasks()
    {
        return dbContext.WorkOrderTasks
                        .OrderBy(t => t.Id)
                        .ToList();
    }

    /// <summary>
    /// Get work order tasks by work order id
    /// </summary>
    /// <param name="workOrderId"></param>
    /// <returns></returns>
    public List<WorkOrderTask> GetWorkOrderTasksByWorkOrderId(int workOrderId)
    {
        WorkOrder workOrder = dbContext.WorkOrders
                        .First(w => w.Id == workOrderId);

        List<WorkOrderTask> workOrderTasks = new List<WorkOrderTask>();

        if (workOrder != null)
        {
            foreach (WorkOrderTask task in workOrder.Tasks)
            {
                workOrderTasks.Add(task);
            }
        }

        return workOrderTasks;
    }

    /// <summary>
    /// Get a single work order task by id
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public WorkOrderTask GetWorkOrderTaskById(int taskId)
    {
        return dbContext.WorkOrderTasks
                        .First(t => t.Id == taskId);
    }

    /// <summary>
    /// Add a new work order task to the database
    /// </summary>
    /// <param name="newTask"></param>
    public void AddWorkOrderTask(WorkOrderTask newTask)
    {
        var taskToAdd = new WorkOrderTask
        {
            Description = newTask.Description,
            CostCents = newTask.CostCents,
            StartDate = newTask.StartDate,
            EndDate = newTask.EndDate,
            Status = newTask.Status
        };

        dbContext.WorkOrderTasks.Add(taskToAdd);
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Update existing work order task using id
    /// </summary>
    /// <param name="idToUpdate"></param>
    /// <param name="template"></param>
    public void UpdateWorkOrderTask(WorkOrderTask updatedTask)
    {
        var taskToUpdate = dbContext.WorkOrderTasks.FirstOrDefault(t => t.Id == updatedTask.Id);
        if (taskToUpdate != null)
        {
            taskToUpdate.Description = updatedTask.Description;
            taskToUpdate.CostCents = updatedTask.CostCents;
            taskToUpdate.StartDate = updatedTask.StartDate;
            taskToUpdate.EndDate = updatedTask.EndDate;
            taskToUpdate.Status = updatedTask.Status;
        }
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Deletes work order task using id
    /// </summary>
    /// <param name="idToDelete"></param>
    public void DeleteWorkOrderTask(int idToDelete)
    {
        var taskToDelete = dbContext.WorkOrderTasks.FirstOrDefault(t => t.Id == idToDelete);
        if (taskToDelete != null)
        {
            dbContext.WorkOrderTasks.Remove(taskToDelete);
        }
        dbContext.SaveChanges();
    }
}