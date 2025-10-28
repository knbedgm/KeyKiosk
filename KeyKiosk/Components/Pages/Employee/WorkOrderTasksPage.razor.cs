using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static KeyKiosk.Services.WorkOrderTaskService;

namespace KeyKiosk.Components.Pages.Employee;

public partial class WorkOrderTasksPage : ComponentBase
{
    // services
    [Inject] private WorkOrderTaskService TaskService { get; set; } = default!;
    [Inject] private WorkOrderService WorkOrderService { get; set; } = default!;
    [Inject] private WorkOrderTaskTemplateService TemplateService { get; set; } = default!;
    [Inject] private ReportService ReportService { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private ToastService ToastService { get; set; } = default!;
    [Inject] private WorkOrderPartService PartService { get; set; } = default!;
    [Inject] private PartTemplateService PartTemplateService { get; set; } = default!;

    [Parameter] public int WorkOrderId { get; set; }

    // state
    protected WorkOrder? WorkOrder { get; set; }
    protected List<WorkOrderTask> TaskList { get; set; } = new();
    protected List<WorkOrderTaskTemplate> TemplateList { get; set; } = new();
    protected List<WorkOrderPart> PartList { get; set; } = new();
    private List<PartTemplate> PartTemplateList { get; set; } = new();

    // add/edit task
    protected AddWorkOrderTaskModel TaskToAdd { get; set; } = new();
    protected bool ShowAddForm { get; set; }
    protected int? EditingTaskId { get; set; }
    protected UpdateWorkOrderTaskModel EditingModel { get; set; } = new();

    // filters
    protected string TaskSearch { get; set; } = "";
    protected string FilterTitle { get; set; } = "";
    protected string FilterDetails { get; set; } = "";
    protected string FilterCost { get; set; } = "";
    protected DateTime? FilterStart { get; set; }
    protected DateTime? FilterEnd { get; set; }
    protected string FilterStatus { get; set; } = "";

    // parts
    private int? SelectedPartTemplateId { get; set; }
    protected string PartFilter { get; set; } = "";

    // header details inline edit
    protected bool IsEditingWoDetails { get; set; } = false;
    protected string WoDetailsDraft { get; set; } = string.Empty;

    // lifecycle
    protected override async Task OnInitializedAsync()
    {
        TemplateList = TemplateService.GetAllTaskTemplates().ToList();
        PartTemplateList = PartTemplateService.GetAllPartTemplates();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadWorkOrderAsync();
        RefreshAllLists();
    }

    // ui handlers
    protected void ToggleAddForm() => ShowAddForm = !ShowAddForm;

    protected void ModifyCreateTaskFromTemplate(ChangeEventArgs e)
    {
        if (e.Value is null) return;
        if (!int.TryParse(e.Value.ToString(), out var tplId)) return;

        var t = TemplateService.GetWorkOrderTaskTemplateById(tplId);
        if (t is null) return;

        TaskToAdd.Title = t.TaskTitle;
        TaskToAdd.Details = t.TaskDetails;
        TaskToAdd.CostCents = t.TaskCostCents;
        ResetAddTaskDefaults();
    }

    protected void BeginEditTask(WorkOrderTask t)
    {
        EditingTaskId = t.Id;
        EditingModel = new UpdateWorkOrderTaskModel
        {
            TaskTitle = t.Title,     // locked in UI
            Details = t.Details,
            CostCents = t.CostCents, // locked
            StartDate = t.StartDate, // locked
            EndDate = t.EndDate,     // locked
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

        var current = TaskList.FirstOrDefault(x => x.Id == EditingTaskId.Value);
        if (current is not null)
        {
            ApplyStatusDates(current, EditingModel);
            // keep locked fields
            EditingModel.TaskTitle = current.Title;
            EditingModel.CostCents = current.CostCents;
        }

        TaskService.UpdateWorkOrderTask(EditingTaskId.Value, EditingModel);
        await ReloadAndRefreshAsync("Task updated.", "success");
        CancelEditTask();
    }

    protected void DeleteTask(int id)
    {
        TaskService.DeleteWorkOrderTask(id);
        RefreshTasksList();
        RecalcTotalsAsync(); // fire and forget
        ShowToast("Task deleted.", "info");
    }

    protected void AddNewTask()
    {
        ResetAddTaskDefaults(); // enforce defaults
        TaskService.AddWorkOrderTask(WorkOrderId, TaskToAdd);
        TaskToAdd = new AddWorkOrderTaskModel();
        RefreshTasksList();
        RecalcTotalsAsync(); // fire and forget
        ShowToast("Task added.", "success");
        ShowAddForm = false;
    }

    protected async Task DeleteWorkOrderAsync()
    {
        if (WorkOrder is null) return;
        await WorkOrderService.DeleteWorkOrderAsync(WorkOrder.Id);
        ShowToast($"Work order {WorkOrder.Id} deleted.", "info");
        Nav.NavigateTo("/employee/home");
    }

    protected async Task GenerateWorkOrderDoc()
    {
        if (WorkOrder is null) return;

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
        ShowToast("Generated work order PDF.", "success");
    }

    // tasks filtering
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

    // parts
    private async Task AddPartFromTemplate()
    {
        if (!SelectedPartTemplateId.HasValue || WorkOrder is null) return;

        var tpl = PartTemplateService.GetPartTemplateById(SelectedPartTemplateId.Value);
        if (tpl is null) return;

        var model = new WorkOrderPartService.AddWorkOrderPartModel
        {
            PartName = tpl.PartName,
            Details = tpl.Details,
            CostCents = tpl.CostCents
        };

        PartService.AddWorkOrderPart(WorkOrder.Id, model);
        await ReloadAndRefreshAsync("Part added from template.", "success");
        SelectedPartTemplateId = null;
    }

    private async Task DeletePart(int id)
    {
        PartService.DeleteWorkOrderPart(id);
        await ReloadAndRefreshAsync("Part deleted.", "info");
    }

    private IEnumerable<WorkOrderPart> FilteredParts =>
        PartList
            .Where(p =>
                string.IsNullOrWhiteSpace(PartFilter)
                || p.PartName.Contains(PartFilter, StringComparison.OrdinalIgnoreCase)
                || (p.Details ?? "").Contains(PartFilter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Id);

    // header details edit
    protected void BeginEditWoDetails()
    {
        if (WorkOrder is null) return;
        WoDetailsDraft = WorkOrder.Details ?? string.Empty;
        IsEditingWoDetails = true;
    }

    protected void CancelEditWoDetails()
    {
        IsEditingWoDetails = false;
        WoDetailsDraft = string.Empty;
    }

    protected async Task SaveWoDetailsAsync()
    {
        if (WorkOrder is null) return;
        WorkOrder.Details = WoDetailsDraft ?? string.Empty;
        await WorkOrderService.UpdateWorkOrderAsync(WorkOrder);
        IsEditingWoDetails = false;
        await ReloadAndRefreshAsync("Work order details updated.", "success");
    }

    // helpers (DRY)

    private async Task LoadWorkOrderAsync()
    {
        WorkOrder = await WorkOrderService.GetByIdAsync(WorkOrderId);
    }

    private void RefreshAllLists()
    {
        RefreshTasksList();
        RefreshPartsList();
    }

    protected void RefreshTasksList()
    {
        TaskList = (WorkOrder?.Tasks ?? new List<WorkOrderTask>()).OrderBy(t => t.Id).ToList();
        StateHasChanged();
    }

    protected void RefreshPartsList()
    {
        PartList = (WorkOrder?.Parts ?? new List<WorkOrderPart>()).OrderBy(p => p.Id).ToList();
        StateHasChanged();
    }

    private async Task ReloadAndRefreshAsync(string toastMsg, string toastType)
    {
        await LoadWorkOrderAsync();
        RefreshAllLists();
        ShowToast(toastMsg, toastType);
    }

    private void ShowToast(string message, string type) => ToastService.ShowToast(message, type);

    private void ResetAddTaskDefaults()
    {
        TaskToAdd.Status = WorkOrderTaskStatusType.Created;
        TaskToAdd.StartDate = null;
        TaskToAdd.EndDate = null;
    }

    // central rule for status->dates
    private static void ApplyStatusDates(WorkOrderTask current, UpdateWorkOrderTaskModel edit)
    {
        var now = DateTimeOffset.Now;

        switch (edit.Status)
        {
            case WorkOrderTaskStatusType.Created:
                edit.StartDate = null;
                edit.EndDate = null;
                break;

            case WorkOrderTaskStatusType.WorkStarted:
                edit.StartDate = current.StartDate ?? now;
                edit.EndDate = null;
                break;

            case WorkOrderTaskStatusType.WorkFinished:
                edit.StartDate = current.StartDate ?? now;
                edit.EndDate = now;
                break;
        }
    }

    private async void RecalcTotalsAsync()
    {
        await LoadWorkOrderAsync();
        RefreshAllLists();
    }

    // formatting
    protected static string FormatDateTime(DateTimeOffset? dt) =>
    dt.HasValue ? dt.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm") : "N/A";

    protected static string FormatCost(int cents)
    {
        decimal dollars = cents / 100.0m;
        return dollars.ToString("C");
    }
}
