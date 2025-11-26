using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace KeyKiosk.Components.Pages.Employee.Admin;

public partial class WorkOrderTaskTemplatesPage : ComponentBase
{
    [Inject] private WorkOrderTaskTemplateService TemplateService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    // ===== Data =====
    protected List<WorkOrderTaskTemplate>? Templates;

    // ===== Toolbar =====
    protected bool ShowAdd { get; set; } = false;

    private string _search = string.Empty;
    protected string Search
    {
        get => _search;
        set
        {
            if (_search != value)
            {
                _search = value ?? string.Empty;
                CurrentPage = 1; // reset when filtering
            }
        }
    }

    // ===== Add form model =====
    protected AddTemplateModel AddModel { get; set; } = new();

    // ===== Inline edit model =====
    protected int? EditingId { get; set; }
    protected WorkOrderTaskTemplate EditingRow { get; set; } = new();

    // ===== Filter (base) =====
    protected IEnumerable<WorkOrderTaskTemplate> Filtered =>
        (Templates ?? Enumerable.Empty<WorkOrderTaskTemplate>())
            .Where(t =>
                string.IsNullOrWhiteSpace(Search)
                || (t.TaskTitle?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false)
                || (t.TaskDetails?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false))
            .OrderBy(t => t.Id);

    // ===== Pagination =====
    private int _pageSize = 10;
    protected int PageSize
    {
        get => _pageSize;
        set
        {
            var v = value <= 0 ? 10 : value;
            if (_pageSize != v)
            {
                _pageSize = v;
                CurrentPage = 1;
            }
        }
    }

    protected int CurrentPage { get; set; } = 1;

    protected int FilteredCount => Filtered.Count();
    protected int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredCount / (double)PageSize));

    protected IEnumerable<WorkOrderTaskTemplate> PagedFiltered =>
        Filtered.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

    protected void GoFirst() => CurrentPage = 1;
    protected void GoPrev() { if (CurrentPage > 1) CurrentPage--; }
    protected void GoNext() { if (CurrentPage < TotalPages) CurrentPage++; }
    protected void GoLast() => CurrentPage = TotalPages;

    private void ClampPage() => CurrentPage = Math.Min(CurrentPage, TotalPages);

    /// <summary>
    /// Runs on page load
    /// </summary>
    protected override void OnInitialized()
    {
        LoadTemplates();
    }

    /// <summary>
    /// Loads existing templates for display
    /// </summary>
    private void LoadTemplates()
    {
        Templates = TemplateService.GetAllTaskTemplates();
        ClampPage();
        StateHasChanged();
    }

    // ===== Toolbar actions =====
    protected void ToggleAdd() => ShowAdd = !ShowAdd;

    /// <summary>
    /// Adds new task template
    /// </summary>
    protected void OnAdd()
    {
        try
        {
            var template = new WorkOrderTaskTemplate
            {
                TaskTitle = AddModel.TaskTitle?.Trim() ?? string.Empty,
                TaskDetails = AddModel.TaskDetails?.Trim() ?? string.Empty,
                TaskCostCents = AddModel.TaskCostCents,
                ExpectedHoursForCompletion = AddModel.ExpectedHours
            };

            TemplateService.AddWorkOrderTaskTemplate(template);
            LoadTemplates();

            AddModel = new();
            ShowAdd = false;
            Snackbar.Add("Template created successfully.", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Create failed: {ex.Message}", Severity.Error);
        }
    }

    // ===== Inline edit =====
    protected void BeginEdit(WorkOrderTaskTemplate t)
    {
        EditingId = t.Id;
        EditingRow = new WorkOrderTaskTemplate
        {
            Id = t.Id,
            TaskTitle = t.TaskTitle,
            // TaskDetails = t.TaskDetails,
            TaskCostCents = t.TaskCostCents,
            ExpectedHoursForCompletion = t.ExpectedHoursForCompletion
        };
    }

    protected void CancelEdit()
    {
        EditingId = null;
        EditingRow = new WorkOrderTaskTemplate();
    }

    protected void SaveRowAsync()
    {
        if (EditingId is null) return;

        try
        {
            TemplateService.UpdateWorkOrderTaskTemplate(EditingRow);
            LoadTemplates();
            EditingId = null;
            Snackbar.Add("Template updated successfully.", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Update failed: {ex.Message}", Severity.Error);
        }
    }

    protected void OnDelete(int id)
    {
        try
        {
            TemplateService.DeleteWorkOrderTaskTemplate(id);
            LoadTemplates();
            ClampPage();
            Snackbar.Add($"Template #{id} deleted.", Severity.Info);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Delete failed: {ex.Message}", Severity.Error);
        }
    }

    // ===== Helpers =====
    protected static string FormatCost(int cents) => (cents / 100m).ToString("C");

    // ===== Add form DTO =====
    protected sealed class AddTemplateModel
    {
        public string? TaskTitle { get; set; }
        public string? TaskDetails { get; set; }
        public int TaskCostCents { get; set; }
        public int ExpectedHours { get; set; }
    }
}
