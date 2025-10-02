using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace KeyKiosk.Components.Pages.Customer;

public partial class CustomerHomePage
{
    [Inject]
    private WorkOrderService DatabaseService { get; set; }

    private string EnteredName { get; set; } = "";

    List<WorkOrder> WorkOrderList { get; set; } = new List<WorkOrder>();

    public CustomerHomePage(WorkOrderService databaseService)
    {
        DatabaseService = databaseService;
    }

    public void GetListOfWorkOrders()
    {
        WorkOrderList.Clear();
        var workOrders = DatabaseService.GetWorkOrdersByCustomerName(EnteredName);
        PopulateWorkOrdersList(workOrders);
    }

    private void PopulateWorkOrdersList(List<WorkOrder> workOrders)
    {
        foreach (WorkOrder w in workOrders)
        {
            WorkOrderList.Add(w);
        }
    }

    public void PrintName()
    {
        Console.WriteLine(EnteredName);
    }
}
