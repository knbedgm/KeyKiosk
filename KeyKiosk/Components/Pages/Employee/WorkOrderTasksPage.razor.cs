using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;

namespace KeyKiosk.Components.Pages.Employee;

public partial class WorkOrderTasksPage : ComponentBase
{
    [Inject]
    private WorkOrderTaskService TaskService { get; set; }

    [Inject]
    private WorkOrderTaskTemplateService TemplateService { get; set; }

    private List<WorkOrderTaskTemplate> TemplateList { get; set; }
    private WorkOrderTaskTemplate SelectedTemplateAdd { get; set; }

    /// <summary>
    /// Displays list of existing tasks
    /// </summary>
    private List<WorkOrderTask> TaskList { get; set; } = new List<WorkOrderTask>();

    /// <summary>
    /// Model for adding task form
    /// </summary>
    private WorkOrderTask TaskToAdd { get; set; } = new WorkOrderTask();

    /// <summary>
    /// Model for updating task form
    /// </summary>
    private WorkOrderTask TaskToUpdate { get; set; } = new WorkOrderTask();

    /// <summary>
    /// Loads existing tasks to display on page
    /// </summary>
    /// <returns></returns>
    protected override Task OnInitializedAsync()
    {
        TemplateList = TemplateService.GetAllTaskTemplates();
        RefreshTasksList();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Refreshes displayed tasks after changes are made
    /// </summary>
    private void RefreshTasksList()
    {
        var tasks = TaskService.GetAllTasks();
        PopulateTaskList(tasks);
    }

    /// <summary>
    /// Updates the task to add description and automatically fills in the cost
    /// </summary>
    /// <param name="e"></param>
    private void UpdateTaskToAddDescriptionCost(ChangeEventArgs e)
    {
        int selectedTemplateId = Int32.Parse(e.Value?.ToString());
        WorkOrderTaskTemplate tempTemplate = TemplateService.GetWorkOrderTaskTemplateById(selectedTemplateId);
        TaskToAdd.Description = tempTemplate.TaskDescription;
        TaskToAdd.CostCents = tempTemplate.TaskCostCents;
    }

    /// <summary>
    /// Updates the task to add description and automatically fills in the cost
    /// </summary>
    /// <param name="e"></param>
    private void UpdateTaskToUpdateDescriptionCost(ChangeEventArgs e)
    {
        int selectedTemplateId = Int32.Parse(e.Value?.ToString());
        WorkOrderTaskTemplate tempTemplate = TemplateService.GetWorkOrderTaskTemplateById(selectedTemplateId);
        TaskToUpdate.Description = tempTemplate.TaskDescription;
        TaskToUpdate.CostCents = tempTemplate.TaskCostCents;
    }

    /// <summary>
    /// Populates TemplateList with data from database
    /// </summary>
    /// <param name="tasks"></param>
    private void PopulateTaskList(List<WorkOrderTask> tasks)
    {
        TaskList.Clear();

        foreach (WorkOrderTask t in tasks)
        {
            TaskList.Add(t);
        }
    }

    /// <summary>
    /// Method to add new task
    /// </summary>
    public void AddNewTask()
    {
        TaskService.AddWorkOrderTask(TaskToAdd);
        RefreshTasksList();
        TaskToAdd = new WorkOrderTask();
    }

    /// <summary>
    /// Method to delete existing task using id
    /// </summary>
    /// <param name="id"></param>
    public void DeleteTask(int id)
    {
        TaskService.DeleteWorkOrderTask(id);
        RefreshTasksList();
    }

    /// <summary>
    /// Method to update existing task
    /// </summary>
    private void UpdateExistingTask()
    {
        TaskService.UpdateWorkOrderTask(TaskToUpdate);
        RefreshTasksList();
        TaskToUpdate = new WorkOrderTask();
    }
}
