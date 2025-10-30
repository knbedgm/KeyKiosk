using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace KeyKiosk.Components.Pages.Employee.Admin;

public partial class PartTemplatesPage
{
    // Data
    private List<PartTemplate> TemplateList { get; set; } = new();
    private PartTemplate TemplateToAdd { get; set; } = new();
    private PartTemplate TemplateToUpdate { get; set; } = new();

    // UI state
    private bool ShowAdd { get; set; } = false;

    private string _search = string.Empty;
    private string Search
    {
        get => _search;
        set
        {
            if (_search != value)
            {
                _search = value ?? string.Empty;
                ResetToFirstPage();
            }
        }
    }

    // Filtering
    private IEnumerable<PartTemplate> Filtered =>
        (TemplateList ?? Enumerable.Empty<PartTemplate>())
            .Where(t =>
                string.IsNullOrWhiteSpace(Search)
                || (t.PartName?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false)
                || (t.Details?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false))
            .OrderBy(t => t.Id);

    // Pagination
    private int _pageSize = 10;
    private int PageSize
    {
        get => _pageSize;
        set
        {
            if (value <= 0) value = 10;
            if (_pageSize != value)
            {
                _pageSize = value;
                ResetToFirstPage();
            }
        }
    }

    private int CurrentPage { get; set; } = 1;

    private int FilteredCount => Filtered.Count();
    private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredCount / (double)PageSize));

    private IEnumerable<PartTemplate> Paged =>
        Filtered.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

    private void ResetToFirstPage()
    {
        CurrentPage = 1;
        StateHasChanged();
    }

    private void GoFirst() => CurrentPage = 1;
    private void GoPrev() { if (CurrentPage > 1) CurrentPage--; }
    private void GoNext() { if (CurrentPage < TotalPages) CurrentPage++; }
    private void GoLast() => CurrentPage = TotalPages;

    // Lifecycle
    protected override Task OnInitializedAsync()
    {
        RefreshPartTemplatesList();
        return Task.CompletedTask;
    }

    private void RefreshPartTemplatesList()
    {
        var templates = PartTemplateService.GetAllPartTemplates();
        PopulateTemplateList(templates);

        // Clamp page if list shrank
        if (CurrentPage > TotalPages) CurrentPage = TotalPages;
    }

    private void PopulateTemplateList(List<PartTemplate> templates)
    {
        TemplateList.Clear();
        foreach (var t in templates)
            TemplateList.Add(t);
    }

    // Add / Delete / Update
    public void AddNewTemplate()
    {
        PartTemplateService.AddPartTemplate(TemplateToAdd);
        TemplateToAdd = new PartTemplate();
        ShowAdd = false;
        RefreshPartTemplatesList();
        ResetToFirstPage();
    }

    public void DeleteTemplate(int id)
    {
        PartTemplateService.DeletePartTemplate(id);
        RefreshPartTemplatesList();
        if (EditingId == id) CancelEdit();

        // If we deleted the last item on the page, bump back a page
        if (!Paged.Any() && CurrentPage > 1) CurrentPage--;
    }

    private void UpdateExistingTemplate()
    {
        PartTemplateService.UpdatePartTemplate(TemplateToUpdate);
        TemplateToUpdate = new PartTemplate();
        RefreshPartTemplatesList();
    }

    private void ToggleAdd() => ShowAdd = !ShowAdd;

    // Inline editing
    private int? EditingId { get; set; }
    private PartTemplate EditingRow { get; set; } = new();

    private void BeginEdit(PartTemplate template)
    {
        EditingId = template.Id;
        EditingRow = new PartTemplate
        {
            Id = template.Id,
            PartName = template.PartName,
            Details = template.Details,
            CostCents = template.CostCents
        };
    }

    private void CancelEdit()
    {
        EditingId = null;
        EditingRow = new PartTemplate();
    }

    private void SaveRowAsync()
    {
        if (EditingId is null) return;

        var updatedTemplate = new PartTemplate
        {
            Id = EditingRow.Id,
            // PartName stays read-only
            Details = EditingRow.Details ?? string.Empty,
            CostCents = EditingRow.CostCents
        };

        PartTemplateService.UpdatePartTemplate(updatedTemplate);

        EditingId = null;
        EditingRow = new PartTemplate();
        RefreshPartTemplatesList();
    }
}
