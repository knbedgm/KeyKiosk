using KeyKiosk.Components.Pages;
using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;

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


	public class AddWorkOrderTaskModel
	{
		public string Title{ get; set; } = "";
		public string Details { get; set; } = "";
		public DateTimeOffset? StartDate { get; set; }
		public DateTimeOffset? EndDate { get; set; }
		public WorkOrderTaskStatusType Status { get; set; }
		public int CostCents { get; set; }
	}

	/// <summary>
	/// Add a new work order task to the database
	/// </summary>
	/// <param name="newTask"></param>
	/// 
	public void AddWorkOrderTask(int workOrderId, AddWorkOrderTaskModel newTask)
	{
		var workOrder = dbContext.WorkOrders.FirstOrDefault(t => t.Id == workOrderId);
		if (workOrder == null)
		{
			throw new ArgumentException($"Work order with id ${workOrderId} doesn't exist", "workOrderId");
		}

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

	/// <summary>
	/// Update existing work order task using id
	/// </summary>
	/// <param name="idToUpdate"></param>
	/// <param name="template"></param>
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

	// Get tasks between two dates
    public async Task<List<WorkOrderTask>> GetTasksByDatePeriod(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await dbContext.WorkOrderTasks
                               .Where(w => w.StartDate.HasValue && w.StartDate.Value >= startDate && w.StartDate.Value <= endDate)
                               .ToListAsync();
    }
}
