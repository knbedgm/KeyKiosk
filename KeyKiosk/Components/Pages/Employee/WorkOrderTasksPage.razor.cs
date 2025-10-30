using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using static KeyKiosk.Services.WorkOrderTaskService;

namespace KeyKiosk.Components.Pages.Employee;

public partial class WorkOrderTasksPage : ComponentBase
{
    // services
    [Inject] private WorkOrderTaskService TaskService { get; set; } = default!;
    [Inject] private WorkOrderService WorkOrderService { get; set; } = default!;
    [Inject] private WorkOrderTaskTemplateService TemplateService { get; set; } = default!;
    [Inject] private PreviewDownloadService PreviewDownloadService { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
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

    // details inline edit
    protected bool IsEditingWoDetails { get; set; } = false;
    protected string WoDetailsDraft { get; set; } = string.Empty;

    // header inline edit
    protected bool IsEditingHeader { get; set; } = false;
    protected string HeaderCustomerDraft { get; set; } = string.Empty;
    protected string HeaderVehicleDraft { get; set; } = string.Empty;
    protected WorkOrderStatusType HeaderStatusDraft { get; set; }

    protected override async Task OnInitializedAsync()
    {
        TemplateList = TemplateService.GetAllTaskTemplates().ToList();
        PartTemplateList = PartTemplateService.GetAllPartTemplates();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadWorkOrderAsync();
        RefreshAllLists();
        await AutoUpdateWorkOrderStatusFromTasksAsync(); // keep WO in sync on load
    }

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
            TaskTitle = t.Title,
            Details = t.Details,
            CostCents = t.CostCents,
            StartDate = t.StartDate,
            EndDate = t.EndDate,
            Status = t.Status,
            HoursForCompletion = t.HoursForCompletion
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
            ApplyStatusDatesAndHours(current, EditingModel);
            EditingModel.TaskTitle = current.Title;
            EditingModel.CostCents = current.CostCents;
        }

        TaskService.UpdateWorkOrderTask(EditingTaskId.Value, EditingModel);
        await ReloadAndRefreshAsync("Task updated.", Severity.Success);
        CancelEditTask();
    }

    protected void DeleteTask(int id)
    {
        TaskService.DeleteWorkOrderTask(id);
        RefreshTasksList();
        _ = AutoUpdateWorkOrderStatusFromTasksAsync(); // fire & forget
        ShowToast("Task deleted.", Severity.Info);
    }

    protected void AddNewTask()
    {
        ResetAddTaskDefaults();
        TaskService.AddWorkOrderTask(WorkOrderId, TaskToAdd);
        TaskToAdd = new AddWorkOrderTaskModel();
        RefreshTasksList();
        _ = AutoUpdateWorkOrderStatusFromTasksAsync(); // fire & forget
        ShowToast("Task added.", Severity.Success);
        ShowAddForm = false;
    }

    protected async Task DeleteWorkOrderAsync()
    {
        if (WorkOrder is null) return;
        await WorkOrderService.DeleteWorkOrderAsync(WorkOrder.Id);
        ShowToast($"Work order {WorkOrder.Id} deleted.", Severity.Info);
        Nav.NavigateTo("/employee/home");
    }

    protected async Task GenerateWorkOrderDoc()
    {
        if (WorkOrder is null) return;
        await PreviewDownloadService.DownloadMechanicTodoAsync(WorkOrder);
        ShowToast("Generated work order PDF.", Severity.Success);
    }

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
        await ReloadAndRefreshAsync("Part added from template.", Severity.Success);
        SelectedPartTemplateId = null;
    }

    private async Task DeletePart(int id)
    {
        PartService.DeleteWorkOrderPart(id);
        await ReloadAndRefreshAsync("Part deleted.", Severity.Info);
    }

    private IEnumerable<WorkOrderPart> FilteredParts =>
        PartList
            .Where(p =>
                string.IsNullOrWhiteSpace(PartFilter)
                || p.PartName.Contains(PartFilter, StringComparison.OrdinalIgnoreCase)
                || (p.Details ?? "").Contains(PartFilter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Id);

    // details edit
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
        await ReloadAndRefreshAsync("Work order details updated.", Severity.Success);
    }

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

    private async Task ReloadAndRefreshAsync(string toastMsg, Severity toastType)
    {
        await LoadWorkOrderAsync();
        RefreshAllLists();
        await AutoUpdateWorkOrderStatusFromTasksAsync(); // keep WO in sync after any change
        ShowToast(toastMsg, toastType);
    }

    private void ShowToast(string message, Severity type) => Snackbar.Add(message, type);

    private void ResetAddTaskDefaults()
    {
        TaskToAdd.Status = WorkOrderTaskStatusType.Created;
        TaskToAdd.StartDate = null;
        TaskToAdd.EndDate = null;
        TaskToAdd.HoursForCompletion = 0;
    }

    // status->dates + hours for task edit
    private static void ApplyStatusDatesAndHours(WorkOrderTask current, UpdateWorkOrderTaskModel edit)
    {
        var now = DateTimeOffset.Now;

        switch (edit.Status)
        {
            case WorkOrderTaskStatusType.Created:
                edit.StartDate = null;
                edit.EndDate = null;
                if (edit.HoursForCompletion <= 0) edit.HoursForCompletion = 0;
                break;

            case WorkOrderTaskStatusType.WorkStarted:
                edit.StartDate = current.StartDate ?? now;
                edit.EndDate = null;
                if (edit.HoursForCompletion < 0) edit.HoursForCompletion = 0;
                break;

            case WorkOrderTaskStatusType.WorkFinished:
                edit.StartDate = current.StartDate ?? now;
                edit.EndDate = now;

                if (edit.HoursForCompletion <= 0 && edit.StartDate.HasValue && edit.EndDate.HasValue)
                {
                    var hours = (edit.EndDate.Value - edit.StartDate.Value).TotalHours;
                    edit.HoursForCompletion = (int)Math.Round(hours, MidpointRounding.AwayFromZero);
                    if (edit.HoursForCompletion < 0) edit.HoursForCompletion = 0;
                }
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

    // header edit
    protected void BeginEditHeader()
    {
        if (WorkOrder is null) return;
        HeaderCustomerDraft = WorkOrder.CustomerName ?? string.Empty;
        HeaderVehicleDraft = WorkOrder.VehiclePlate ?? string.Empty;
        HeaderStatusDraft = WorkOrder.Status;
        IsEditingHeader = true;
    }

    protected void CancelEditHeader()
    {
        IsEditingHeader = false;
        HeaderCustomerDraft = string.Empty;
        HeaderVehicleDraft = string.Empty;
    }

    protected async Task SaveHeaderAsync()
    {
        if (WorkOrder is null) return;

        WorkOrder.CustomerName = (HeaderCustomerDraft ?? string.Empty).Trim();
        WorkOrder.VehiclePlate = (HeaderVehicleDraft ?? string.Empty).Trim();

        ApplyWorkOrderStatusDates(WorkOrder, HeaderStatusDraft);

        await WorkOrderService.UpdateWorkOrderAsync(WorkOrder);

        IsEditingHeader = false;
        await ReloadAndRefreshAsync("Work order header updated.", Severity.Success);
    }

    // WO status by header pick (now also handles WorkFinished)
    private static void ApplyWorkOrderStatusDates(WorkOrder wo, WorkOrderStatusType newStatus)
    {
        var now = DateTimeOffset.Now;

        switch (newStatus)
        {
            case WorkOrderStatusType.Created:
                wo.Status = newStatus;
                wo.StartDate = null;
                wo.EndDate = null;
                break;

            case WorkOrderStatusType.WorkStarted:
                wo.Status = newStatus;
                wo.StartDate ??= now;
                wo.EndDate = null;
                break;

            case WorkOrderStatusType.WorkFinished:
                wo.Status = newStatus;
                wo.StartDate ??= now;
                wo.EndDate = now;
                break;

            case WorkOrderStatusType.Closed:
                // Keeping legacy path if you still use Closed somewhere else
                wo.StartDate ??= now;
                wo.EndDate = now;
                wo.Status = newStatus;
                break;

            default:
                wo.Status = newStatus;
                break;
        }
    }

    // === Auto-set WO status from tasks (Created / WorkStarted / WorkFinished) ===
    private async Task AutoUpdateWorkOrderStatusFromTasksAsync()
    {
        if (WorkOrder is null) return;

        var tasks = TaskList;
        if (tasks.Count == 0) return;

        bool allCreated = tasks.All(t => t.Status == WorkOrderTaskStatusType.Created);
        bool allFinished = tasks.All(t => t.Status == WorkOrderTaskStatusType.WorkFinished);

        var now = DateTimeOffset.Now;

        if (allCreated)
        {
            // All tasks are Created -> WO Created
            WorkOrder.Status = WorkOrderStatusType.Created;
            WorkOrder.StartDate = null;
            WorkOrder.EndDate = null;

            await WorkOrderService.UpdateWorkOrderAsync(WorkOrder);
            StateHasChanged();
            return;
        }

        if (allFinished)
        {
            // All tasks Finished -> WO WorkFinished
            var minStart = tasks.Where(t => t.StartDate.HasValue).Select(t => t.StartDate!.Value).DefaultIfEmpty(now).Min();
            var maxEnd = tasks.Where(t => t.EndDate.HasValue).Select(t => t.EndDate!.Value).DefaultIfEmpty(now).Max();

            WorkOrder.Status = WorkOrderStatusType.WorkFinished;
            WorkOrder.StartDate ??= minStart;
            WorkOrder.EndDate = maxEnd;

            await WorkOrderService.UpdateWorkOrderAsync(WorkOrder);
            StateHasChanged();
            return;
        }

        // Any other combination -> WO WorkStarted
        WorkOrder.Status = WorkOrderStatusType.WorkStarted;

        // Prefer earliest real start if any task has one; otherwise set now
        var anyTaskStart = tasks.Where(t => t.StartDate.HasValue).Select(t => t.StartDate!.Value).OrderBy(d => d).FirstOrDefault();
        if (anyTaskStart != default)
            WorkOrder.StartDate ??= anyTaskStart;
        else
            WorkOrder.StartDate ??= now;

        WorkOrder.EndDate = null;

        await WorkOrderService.UpdateWorkOrderAsync(WorkOrder);
        StateHasChanged();
    }
}
