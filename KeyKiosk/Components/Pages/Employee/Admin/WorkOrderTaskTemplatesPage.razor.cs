using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;

namespace KeyKiosk.Components.Pages.Employee.Admin;

public partial class WorkOrderTaskTemplatesPage
{
    [Inject] private WorkOrderTaskTemplateService TaskTemplateService { get; set; }

    /// <summary>
    /// Displays list of existing templates
    /// </summary>
    private List<WorkOrderTaskTemplate> TemplateList { get; set; } = new List<WorkOrderTaskTemplate>();

    /// <summary>
    /// Model for adding template form
    /// </summary>
    private WorkOrderTaskTemplate TemplateToAdd { get; set; } = new WorkOrderTaskTemplate();

    /// <summary>
    /// Model for updating template form
    /// </summary>
    private WorkOrderTaskTemplate TemplateToUpdate { get; set; } = new WorkOrderTaskTemplate();

    /// <summary>
    /// Loads existing templates to display on page
    /// </summary>
    /// <returns></returns>
    protected override Task OnInitializedAsync()
    {
        RefreshTaskTemplatesList();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Refreshes displayed templates after changes are made
    /// </summary>
    private void RefreshTaskTemplatesList()
    {
        var templates = TaskTemplateService.GetAllTaskTemplates();
        PopulateTemplateList(templates);
    }

    /// <summary>
    /// Populates TemplateList with data from database
    /// </summary>
    /// <param name="templates"></param>
    private void PopulateTemplateList(List<WorkOrderTaskTemplate> templates)
    {
        TemplateList.Clear();

        foreach (WorkOrderTaskTemplate t in templates)
        {
            TemplateList.Add(t);
        }
    }

    /// <summary>
    /// Method to add new template
    /// </summary>
    public void AddNewTemplate()
    {
        TaskTemplateService.AddWorkOrderTaskTemplate(TemplateToAdd);
        RefreshTaskTemplatesList();
        TemplateToAdd = new WorkOrderTaskTemplate();
    }

    /// <summary>
    /// Method to delete existing template using id
    /// </summary>
    /// <param name="id"></param>
    public void DeleteTemplate(int id)
    {
        TaskTemplateService.DeleteWorkOrderTaskTemplate(id);
        RefreshTaskTemplatesList();
    }

    /// <summary>
    /// Method to update existing template
    /// </summary>
    private void UpdateExistingTemplate()
    {
        TaskTemplateService.UpdateWorkOrderTaskTemplate(TemplateToUpdate);
        RefreshTaskTemplatesList();
        TemplateToUpdate = new WorkOrderTaskTemplate();
    }
}
