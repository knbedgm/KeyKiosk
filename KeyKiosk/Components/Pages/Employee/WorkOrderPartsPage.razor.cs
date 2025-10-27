using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;
using static KeyKiosk.Services.WorkOrderPartService;

namespace KeyKiosk.Components.Pages.Employee;

public partial class WorkOrderPartsPage
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Inject] private WorkOrderPartService PartService { get; set; }
    [Inject] private WorkOrderService WorkOrderService { get; set; }
    [Inject] private PartTemplateService TemplateService { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [Parameter] public int WorkOrderId { get; set; }
    private WorkOrder? WorkOrder;

    private List<PartTemplate> TemplateList { get; set; } = new List<PartTemplate>();

    /// <summary>
    /// Displays list of existing Parts
    /// </summary>
    private List<WorkOrderPart> PartList { get; set; } = new List<WorkOrderPart>();

    /// <summary>
    /// Model for adding Part form
    /// </summary>
    private AddWorkOrderPartModel PartToAdd { get; set; } = new AddWorkOrderPartModel();

    /// <summary>
    /// Model for updating Part form
    /// </summary>
    private UpdateWorkOrderPartModel PartToUpdate { get; set; } = new UpdateWorkOrderPartModel();
    private WorkOrderPart? PartToUpdateOriginal { get; set; }
    private int? PartToUpdateId { get; set; }

    /// <summary>
    /// Loads existing Parts to display on page
    /// </summary>
    /// <returns></returns>
    protected override Task OnInitializedAsync()
    {
        TemplateList.AddRange(TemplateService.GetAllPartTemplates());
        RefreshPartsList();
        return Task.CompletedTask;
    }

    protected override async Task OnParametersSetAsync()
    {
        WorkOrder = await WorkOrderService.GetByIdAsync(WorkOrderId);
        RefreshPartsList();
    }

    /// <summary>
    /// Refreshes displayed Parts after changes are made
    /// </summary>
    private void RefreshPartsList()
    {
        var Parts = WorkOrder?.Parts;
        PartList.Clear();
        if (Parts != null)
            PartList.AddRange(Parts);
        //this.StateHasChanged();
    }

    /// <summary>
    /// Updates the Part to add description and automatically fills in the cost
    /// </summary>
    /// <param name="e"></param>
    private void ModifyCreatePartFromTemplate(ChangeEventArgs e)
    {
        int selectedTemplateId = Int32.Parse(e.Value?.ToString());
        PartTemplate tempTemplate = TemplateService.GetPartTemplateById(selectedTemplateId);
        PartToAdd.PartName = tempTemplate.PartName;
        PartToAdd.Details = tempTemplate.Details;
        PartToAdd.CostCents = tempTemplate.CostCents;
    }

    /// <summary>
    /// Updates the Part to add description and automatically fills in the cost
    /// </summary>
    /// <param name="e"></param>
    private void ModifyUpdatePartFromTemplate(ChangeEventArgs e)
    {
        int selectedTemplateId = Int32.Parse(e.Value?.ToString());
        PartTemplate tempTemplate = TemplateService.GetPartTemplateById(selectedTemplateId);
        PartToUpdate.PartName = tempTemplate.PartName;
        PartToUpdate.Details = tempTemplate.Details;
        PartToUpdate.CostCents = tempTemplate.CostCents;
    }

    private async Task LoadUpdatePart(int? val)
    {
        //var val = e.Value?.ToString();

        if (val == null)
        {
            ClearPartToUpdate();
            return;
        }

        PartToUpdateId = val;
        var t = WorkOrder!.Parts.First(t => t.Id == PartToUpdateId);
        PartToUpdateOriginal = t;

        PartToUpdate = new UpdateWorkOrderPartModel
        {
            Details = t.Details,
            CostCents = t.CostCents,
        };
    }

    private void ClearPartToUpdate()
    {
        PartToUpdateId = null;
        PartToUpdateOriginal = null;
        PartToUpdate = new UpdateWorkOrderPartModel();
    }

    /// <summary>
    /// Method to add new Part
    /// </summary>
    public void AddNewPart()
    {
        PartService.AddWorkOrderPart(WorkOrderId, PartToAdd);
        RefreshPartsList();
        PartToAdd = new AddWorkOrderPartModel();
    }

    /// <summary>
    /// Method to delete existing Part using id
    /// </summary>
    /// <param name="id"></param>
    public void DeletePart(int id)
    {
        PartService.DeleteWorkOrderPart(id);
        RefreshPartsList();
    }

    /// <summary>
    /// Method to update existing Part
    /// </summary>
    private void UpdateExistingPart()
    {
        PartService.UpdateWorkOrderPart(PartToUpdateId!.Value, PartToUpdate);
        RefreshPartsList();
        ClearPartToUpdate();
    }
}
