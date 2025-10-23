using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;
using static KeyKiosk.Services.WorkOrderTaskService;

namespace KeyKiosk.Components.Pages.Employee;

public partial class WorkOrderTasksPage : ComponentBase
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Inject]
    private WorkOrderTaskService TaskService { get; set; }
    [Inject]
    private WorkOrderService WorkOrderService { get; set; }
    [Inject]
    private WorkOrderTaskTemplateService TemplateService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [Parameter] public int WorkOrderId { get; set; }
    private WorkOrder? WorkOrder;

    private List<WorkOrderTaskTemplate> TemplateList { get; set; } = new List<WorkOrderTaskTemplate>();

    /// <summary>
    /// Displays list of existing tasks
    /// </summary>
    private List<WorkOrderTask> TaskList { get; set; } = new List<WorkOrderTask>();

    /// <summary>
    /// Model for adding task form
    /// </summary>
    private AddWorkOrderTaskModel TaskToAdd { get; set; } = new AddWorkOrderTaskModel();

    /// <summary>
    /// Model for updating task form
    /// </summary>
    private UpdateWorkOrderTaskModel TaskToUpdate { get; set; } = new UpdateWorkOrderTaskModel();
    private WorkOrderTask? TaskToUpdateOriginal { get; set; }
    private int? TaskToUpdateId { get; set; }

    /// <summary>
    /// Loads existing tasks to display on page
    /// </summary>
    /// <returns></returns>
    protected override Task OnInitializedAsync()
    {
        TemplateList.AddRange(TemplateService.GetAllTaskTemplates());
        RefreshTasksList();
        return Task.CompletedTask;
    }

    protected override async Task OnParametersSetAsync()
    {
        WorkOrder = await WorkOrderService.GetByIdAsync(WorkOrderId);
        RefreshTasksList();
    }

    /// <summary>
    /// Refreshes displayed tasks after changes are made
    /// </summary>
    private void RefreshTasksList()
    {
        var tasks = WorkOrder?.Tasks;
        TaskList.Clear();
        if (tasks != null)
            TaskList.AddRange(tasks);
        //this.StateHasChanged();
    }

    /// <summary>
    /// Updates the task to add description and automatically fills in the cost
    /// </summary>
    /// <param name="e"></param>
    private void ModifyCreateTaskFromTemplate(ChangeEventArgs e)
    {
        int selectedTemplateId = Int32.Parse(e.Value?.ToString());
        WorkOrderTaskTemplate tempTemplate = TemplateService.GetWorkOrderTaskTemplateById(selectedTemplateId);
        TaskToAdd.Title = tempTemplate.TaskTitle;
        TaskToAdd.Details = tempTemplate.TaskDetails;
        TaskToAdd.CostCents = tempTemplate.TaskCostCents;
    }

    /// <summary>
    /// Updates the task to add description and automatically fills in the cost
    /// </summary>
    /// <param name="e"></param>
    private void ModifyUpdateTaskFromTemplate(ChangeEventArgs e)
    {
        int selectedTemplateId = Int32.Parse(e.Value?.ToString());
        WorkOrderTaskTemplate tempTemplate = TemplateService.GetWorkOrderTaskTemplateById(selectedTemplateId);
        TaskToUpdate.TaskTitle = tempTemplate.TaskTitle;
        TaskToUpdate.Details = tempTemplate.TaskDetails;
        TaskToUpdate.CostCents = tempTemplate.TaskCostCents;
    }

    private async Task LoadUpdateTask(int? val)
    {
        //var val = e.Value?.ToString();

        if (val == null)
        {
            ClearTaskToUpdate();
            return;
        }

        TaskToUpdateId = val;
        var t = WorkOrder!.Tasks.First(t => t.Id == TaskToUpdateId);
        TaskToUpdateOriginal = t;

        TaskToUpdate = new UpdateWorkOrderTaskModel
        {
            Details = t.Details,
            StartDate = t.StartDate,
            EndDate = t.EndDate,
            Status = t.Status,
            CostCents = t.CostCents,
        };
    }

    private void ClearTaskToUpdate()
    {
        TaskToUpdateId = null;
        TaskToUpdateOriginal = null;
        TaskToUpdate = new UpdateWorkOrderTaskModel();
    }

    /// <summary>
    /// Method to add new task
    /// </summary>
    public void AddNewTask()
    {
        TaskService.AddWorkOrderTask(WorkOrderId, TaskToAdd);
        RefreshTasksList();
        TaskToAdd = new AddWorkOrderTaskModel();
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
        TaskService.UpdateWorkOrderTask(TaskToUpdateId!.Value, TaskToUpdate);
        RefreshTasksList();
        ClearTaskToUpdate();
    }
}