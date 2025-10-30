using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace KeyKiosk.Components.Pages.Employee.Admin;

public partial class PartTemplatesPage
{
    // ===== Data =====
    private List<PartTemplate> TemplateList { get; set; } = new();
    private PartTemplate TemplateToAdd { get; set; } = new();
    private PartTemplate TemplateToUpdate { get; set; } = new();

    // ===== UI State (new) =====
    private bool ShowAdd { get; set; } = false;
    private string Search { get; set; } = string.Empty;

    // Computed filter to match Users page behavior
    private IEnumerable<PartTemplate> Filtered =>
        (TemplateList ?? Enumerable.Empty<PartTemplate>())
            .Where(t =>
                string.IsNullOrWhiteSpace(Search)
                || (t.PartName?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false)
                || (t.Details?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false));

    protected override Task OnInitializedAsync()
    {
        RefreshPartTemplatesList();
        return Task.CompletedTask;
    }

    private void RefreshPartTemplatesList()
    {
        var templates = PartTemplateService.GetAllPartTemplates();
        PopulateTemplateList(templates);
    }

    private void PopulateTemplateList(List<PartTemplate> templates)
    {
        TemplateList.Clear();
        foreach (PartTemplate t in templates)
            TemplateList.Add(t);
    }

    // ===== Add / Delete / Update =====
    public void AddNewTemplate()
    {
        PartTemplateService.AddPartTemplate(TemplateToAdd);
        RefreshPartTemplatesList();
        TemplateToAdd = new PartTemplate();
        ShowAdd = false; // collapse panel like Users page
        StateHasChanged();
    }

    public void DeleteTemplate(int id)
    {
        PartTemplateService.DeletePartTemplate(id);
        RefreshPartTemplatesList();
        if (EditingId == id) CancelEdit();
    }

    private void UpdateExistingTemplate()
    {
        PartTemplateService.UpdatePartTemplate(TemplateToUpdate);
        RefreshPartTemplatesList();
        TemplateToUpdate = new PartTemplate();
    }

    // ===== Toolbar toggle (new) =====
    private void ToggleAdd() => ShowAdd = !ShowAdd;

    // ===== Inline Editing Logic =====
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
            // PartName remains read-only per your UI
            Details = EditingRow.Details ?? string.Empty,
            CostCents = EditingRow.CostCents
        };

        PartTemplateService.UpdatePartTemplate(updatedTemplate);

        EditingId = null;
        EditingRow = new PartTemplate();
        RefreshPartTemplatesList();
    }
}
