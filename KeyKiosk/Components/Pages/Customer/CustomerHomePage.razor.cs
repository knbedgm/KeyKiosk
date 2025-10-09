using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;

namespace KeyKiosk.Components.Pages.Customer;

public partial class CustomerHomePage
{
    //[Inject]
    //private WorkOrderService DatabaseService { get; set; }

    /// <summary>
    /// Stores name to query with
    /// </summary>
    private string EnteredName { get; set; } = "";

    /// <summary>
    /// List for displaying work orders
    /// </summary>
    List<WorkOrder> WorkOrderList { get; set; } = new List<WorkOrder>();

    //// Event handler for button click
    //private async Task GetListOfWorkOrdersAsync()
    //{
    //    WorkOrderList.Clear();
    //    if (string.IsNullOrWhiteSpace(EnteredName)) return;

    /// <summary>
    /// Gets list of work orders based on entered name
    /// </summary>
    public void GetListOfWorkOrders()
    {
        WorkOrderList.Clear();
        var workOrders = DatabaseService.GetWorkOrdersByCustomerName(EnteredName);
        PopulateWorkOrdersList(workOrders);
    }

    /// <summary>
    /// Populates WorkOrderList with data
    /// </summary>
    /// <param name="workOrders"></param>
    private void PopulateWorkOrdersList(List<WorkOrder> workOrders)
    {
        foreach (WorkOrder w in workOrders)
        {
            WorkOrderList.Add(w);
        }
    }
}
