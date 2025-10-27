using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;

namespace KeyKiosk.Components.Pages.Employee.Admin;

public partial class PartTemplatesPage
{
    [Inject] private PartTemplateService PartTemplateService { get; set; }

    /// <summary>
    /// Displays list of existing templates
    /// </summary>
    private List<PartTemplate> TemplateList { get; set; } = new List<PartTemplate>();

    /// <summary>
    /// Model for adding template form
    /// </summary>
    private PartTemplate TemplateToAdd { get; set; } = new PartTemplate();

    /// <summary>
    /// Model for updating template form
    /// </summary>
    private PartTemplate TemplateToUpdate { get; set; } = new PartTemplate();

    /// <summary>
    /// Loads existing templates to display on page
    /// </summary>
    /// <returns></returns>
    protected override Task OnInitializedAsync()
    {
        RefreshPartTemplatesList();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Refreshes displayed templates after changes are made
    /// </summary>
    private void RefreshPartTemplatesList()
    {
        var templates = PartTemplateService.GetAllPartTemplates();
        PopulateTemplateList(templates);
    }

    /// <summary>
    /// Populates TemplateList with data from database
    /// </summary>
    /// <param name="templates"></param>
    private void PopulateTemplateList(List<PartTemplate> templates)
    {
        TemplateList.Clear();

        foreach (PartTemplate t in templates)
        {
            TemplateList.Add(t);
        }
    }

    /// <summary>
    /// Method to add new template
    /// </summary>
    public void AddNewTemplate()
    {
        PartTemplateService.AddPartTemplate(TemplateToAdd);
        RefreshPartTemplatesList();
        TemplateToAdd = new PartTemplate();
    }

    /// <summary>
    /// Method to delete existing template using id
    /// </summary>
    /// <param name="id"></param>
    public void DeleteTemplate(int id)
    {
        PartTemplateService.DeletePartTemplate(id);
        RefreshPartTemplatesList();
    }

    /// <summary>
    /// Method to update existing template
    /// </summary>
    private void UpdateExistingTemplate()
    {
        PartTemplateService.UpdatePartTemplate(TemplateToUpdate);
        RefreshPartTemplatesList();
        TemplateToUpdate = new PartTemplate();
    }
}
