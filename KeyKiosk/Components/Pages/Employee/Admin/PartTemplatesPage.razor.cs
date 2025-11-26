using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace KeyKiosk.Components.Pages.Employee.Admin;

public partial class PartTemplatesPage
{
    /// <summary>
    /// Stores list of existing templates in database
    /// </summary>
    private List<PartTemplate> TemplateList { get; set; } = new();
    
    /// <summary>
    /// Stores data for template to add
    /// </summary>
    private PartTemplate TemplateToAdd { get; set; } = new();
    
    /// <summary>
    /// Stores data for template to update
    /// </summary>
    private PartTemplate TemplateToUpdate { get; set; } = new();

    /// <summary>
    /// Controls whether to show the add template panel
    /// </summary>
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

    /// <summary>
    /// Filters displayed templates
    /// </summary>
    private IEnumerable<PartTemplate> Filtered =>
        (TemplateList ?? Enumerable.Empty<PartTemplate>())
            .Where(t =>
                string.IsNullOrWhiteSpace(Search)
                || (t.PartName?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false)
                || (t.Details?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false))
            .OrderBy(t => t.Id);

    /// <summary>
    /// Pagination
    /// </summary>
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

    /// <summary>
    /// Current page value
    /// </summary>
    private int CurrentPage { get; set; } = 1;

    private int FilteredCount => Filtered.Count();

    /// <summary>
    /// Total number of pages
    /// </summary>
    private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredCount / (double)PageSize));

    private IEnumerable<PartTemplate> Paged =>
        Filtered.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

    /// <summary>
    /// Resets the current page to the first page
    /// </summary>
    private void ResetToFirstPage()
    {
        CurrentPage = 1;
        StateHasChanged();
    }

    // Change the displayed page
    private void GoFirst() => CurrentPage = 1;
    private void GoPrev() { if (CurrentPage > 1) CurrentPage--; }
    private void GoNext() { if (CurrentPage < TotalPages) CurrentPage++; }
    private void GoLast() => CurrentPage = TotalPages;

    /// <summary>
    /// Runs on page load
    /// </summary>
    /// <returns></returns>
    protected override Task OnInitializedAsync()
    {
        RefreshPartTemplatesList();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets templates from database and populates template list
    /// </summary>
    private void RefreshPartTemplatesList()
    {
        var templates = PartTemplateService.GetAllPartTemplates();
        PopulateTemplateList(templates);

        // Clamp page if list shrank
        if (CurrentPage > TotalPages) CurrentPage = TotalPages;
    }

    /// <summary>
    /// Populates template list with provided data
    /// </summary>
    /// <param name="templates">List of templates</param>
    private void PopulateTemplateList(List<PartTemplate> templates)
    {
        TemplateList.Clear();
        foreach (var t in templates)
            TemplateList.Add(t);
    }

    /// <summary>
    /// Adds new using PartTemplateService
    /// </summary>
    public void AddNewTemplate()
    {
        PartTemplateService.AddPartTemplate(TemplateToAdd);
        TemplateToAdd = new PartTemplate();
        ShowAdd = false;
        RefreshPartTemplatesList();
        ResetToFirstPage();
    }

    /// <summary>
    /// Deletes template using PartTemplateService
    /// </summary>
    public void DeleteTemplate(int id)
    {
        PartTemplateService.DeletePartTemplate(id);
        RefreshPartTemplatesList();
        if (EditingId == id) CancelEdit();

        // If we deleted the last item on the page, bump back a page
        if (!Paged.Any() && CurrentPage > 1) CurrentPage--;
    }

    /// <summary>
    /// Updates existing template using PartTemplateService
    /// </summary>
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

    /// <summary>
    /// Cancels changes made while editing template
    /// </summary>
    private void CancelEdit()
    {
        EditingId = null;
        EditingRow = new PartTemplate();
    }

    /// <summary>
    /// Updates edited template using PartTemplateService
    /// </summary>
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
