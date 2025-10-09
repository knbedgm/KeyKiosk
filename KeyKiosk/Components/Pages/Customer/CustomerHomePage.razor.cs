using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;

namespace KeyKiosk.Components.Pages.Customer;

public partial class CustomerHomePage
{
    //[Inject]
    //private WorkOrderService DatabaseService { get; set; }

    //private string EnteredName { get; set; } = "";
    //private List<WorkOrder> WorkOrderList { get; set; } = new List<WorkOrder>();

    //// Do NOT use a constructor for async calls
    //public CustomerHomePage() { }

    //// Event handler for button click
    //private async Task GetListOfWorkOrdersAsync()
    //{
    //    WorkOrderList.Clear();
    //    if (string.IsNullOrWhiteSpace(EnteredName)) return;

    //    var workOrders = await DatabaseService.GetWorkOrdersByCustomerNameAsync(EnteredName);
    //    PopulateWorkOrdersList(workOrders);
    //}

    //private void PopulateWorkOrdersList(List<WorkOrder> workOrders)
    //{
    //    if (workOrders == null) return;
    //    WorkOrderList.Clear();
    //    WorkOrderList.AddRange(workOrders);
    //}
}
