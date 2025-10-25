using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static KeyKiosk.Services.WorkOrderTaskService;

namespace KeyKiosk.Components.Pages.Employee;

public partial class WorkOrderTasksPage : ComponentBase
{
    [Inject] private WorkOrderTaskService TaskService { get; set; } = default!;
    [Inject] private WorkOrderService WorkOrderService { get; set; } = default!;
    [Inject] private WorkOrderTaskTemplateService TemplateService { get; set; } = default!;
    [Inject] private ReportService ReportService { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private ToastService ToastService { get; set; } = default!;

    [Parameter] public int WorkOrderId { get; set; }

    protected WorkOrder? WorkOrder { get; set; }
    protected List<WorkOrderTask> TaskList { get; set; } = new();
    protected List<WorkOrderTaskTemplate> TemplateList { get; set; } = new();

    // Add form
    protected AddWorkOrderTaskModel TaskToAdd { get; set; } = new();
    protected bool ShowAddForm { get; set; }
    protected void ToggleAddForm() => ShowAddForm = !ShowAddForm;

    // Inline edit
    protected int? EditingTaskId { get; set; }
    protected UpdateWorkOrderTaskModel EditingModel { get; set; } = new();

    // Filters
    protected string TaskSearch { get; set; } = "";
    protected string FilterTitle { get; set; } = "";
    protected string FilterDetails { get; set; } = "";
    protected string FilterCost { get; set; } = "";
    protected DateTime? FilterStart { get; set; }
    protected DateTime? FilterEnd { get; set; }
    protected string FilterStatus { get; set; } = "";

    protected override async Task OnInitializedAsync()
    {
        TemplateList = TemplateService.GetAllTaskTemplates().ToList();
    }

    protected override async Task OnParametersSetAsync()
    {
        WorkOrder = await WorkOrderService.GetByIdAsync(WorkOrderId);
        RefreshTasksList();
    }

    protected void RefreshTasksList()
    {
        TaskList = (WorkOrder?.Tasks ?? new List<WorkOrderTask>()).OrderBy(t => t.Id).ToList();
        StateHasChanged();
    }

    // --- Add / Update / Delete ---

    protected void ModifyCreateTaskFromTemplate(ChangeEventArgs e)
    {
        if (e.Value is null) return;
        if (!int.TryParse(e.Value.ToString(), out var selectedTemplateId)) return;

        var t = TemplateService.GetWorkOrderTaskTemplateById(selectedTemplateId);
        if (t is null) return;

        TaskToAdd.Title = t.TaskTitle;
        TaskToAdd.Details = t.TaskDetails;
        TaskToAdd.CostCents = t.TaskCostCents;
    }

    protected void BeginEditTask(WorkOrderTask t)
    {
        EditingTaskId = t.Id;
        EditingModel = new UpdateWorkOrderTaskModel
        {
            TaskTitle = t.Title,
            Details = t.Details,
            CostCents = t.CostCents,
            StartDate = t.StartDate,
            EndDate = t.EndDate,
            Status = t.Status
        };
    }

    protected void CancelEditTask()
    {
        EditingTaskId = null;
        EditingModel = new UpdateWorkOrderTaskModel();
    }

    protected async Task SaveTaskAsync()
    {
        if (EditingTaskId is null) return;

        TaskService.UpdateWorkOrderTask(EditingTaskId.Value, EditingModel);
        await ReloadWorkOrderAndTasks();
        ToastService.ShowToast("Task updated.", "success");
        CancelEditTask();
    }

    protected void DeleteTask(int id)
    {
        TaskService.DeleteWorkOrderTask(id);
        RefreshTasksList();
        RecalcWorkOrderTotal();
        ToastService.ShowToast("Task deleted.", "info");
    }

    protected void AddNewTask()
    {
        TaskService.AddWorkOrderTask(WorkOrderId, TaskToAdd);
        TaskToAdd = new AddWorkOrderTaskModel();
        RefreshTasksList();
        RecalcWorkOrderTotal();
        ToastService.ShowToast("Task added.", "success");
        ShowAddForm = false;
    }

    // --- WO actions ---

    protected async Task DeleteWorkOrderAsync()
    {
        if (WorkOrder is null) return;
        await WorkOrderService.DeleteWorkOrderAsync(WorkOrder.Id);
        ToastService.ShowToast($"Work order {WorkOrder.Id} deleted.", "info");
        Nav.NavigateTo("/employee/home");
    }

    protected async Task GenerateWorkOrderDoc()
    {
        if (WorkOrder is null) return;

        // Uses your existing ReportService to generate a PDF; downloads it to the browser.
        var pdfBytes = ReportService.GenerateReport(WorkOrder);
        var base64 = Convert.ToBase64String(pdfBytes);

        var js = @"
            window.downloadFileFromBytes = (filename, base64) => {
                const link = document.createElement('a');
                link.href = 'data:application/pdf;base64,' + base64;
                link.download = filename;
                link.click();
            };";
        await JS.InvokeVoidAsync("eval", js);
        await JS.InvokeVoidAsync("downloadFileFromBytes", $"WorkOrder_{WorkOrder.Id}.pdf", base64);

        ToastService.ShowToast("Generated work order PDF.", "success");
    }

    // --- Helpers ---

    protected IEnumerable<WorkOrderTask> FilteredTasks =>
        TaskList.Where(t =>
            (string.IsNullOrWhiteSpace(TaskSearch) ||
                t.Title.Contains(TaskSearch, StringComparison.OrdinalIgnoreCase) ||
                (t.Details ?? "").Contains(TaskSearch, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(FilterTitle) || t.Title.Contains(FilterTitle, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(FilterDetails) || (t.Details ?? "").Contains(FilterDetails, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(FilterCost) || t.CostCents.ToString().Contains(FilterCost)) &&
            (!FilterStart.HasValue || (t.StartDate.HasValue && t.StartDate.Value.Date == FilterStart.Value.Date)) &&
            (!FilterEnd.HasValue || (t.EndDate.HasValue && t.EndDate.Value.Date == FilterEnd.Value.Date)) &&
            (string.IsNullOrWhiteSpace(FilterStatus) || t.Status.ToString() == FilterStatus)
        );

    protected static string FormatDate(DateTimeOffset? dt) =>
        dt?.ToLocalTime().ToString("yyyy-MM-dd") ?? "-";

    protected static string FormatCost(int cents)
    {
        decimal dollars = cents / 100.0m;
        return dollars.ToString("C");
    }

    private async Task ReloadWorkOrderAndTasks()
    {
        WorkOrder = await WorkOrderService.GetByIdAsync(WorkOrderId);
        RefreshTasksList();
    }

    private void RecalcWorkOrderTotal()
    {
        // If your WorkOrderService recalculates and persists totals, reload it.
        // Otherwise you can compute client-side as needed.
        _ = ReloadWorkOrderAndTasks();
    }
}
