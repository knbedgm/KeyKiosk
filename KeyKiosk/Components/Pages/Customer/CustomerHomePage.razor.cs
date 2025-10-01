using KeyKiosk.Data;
using KeyKiosk.Services;

namespace KeyKiosk.Components.Pages.Customer;

public partial class CustomerHomePage
{
    ApplicationDbContext dbContext;
    DatabaseService databaseService;
    private string EnteredName { get; set; }

    List<WorkOrder> WorkOrderList { get; set; } = new List<WorkOrder>();

    public CustomerHomePage(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        databaseService = new DatabaseService(dbContext);
        EnteredName = "";
    }

    public void GetListOfWorkOrders()
    {
        WorkOrderList.Clear();
        GetWorkOrdersByCustomerName(EnteredName);
    }

    public bool GetWorkOrdersByCustomerName(string customerName)
    {


        var workOrders = from workOrder in dbContext.WorkOrders
                         where workOrder.CustomerName == customerName
                         orderby workOrder.StartDate
                         select workOrder;

        foreach (WorkOrder workOrder in workOrders)
        {
            Console.WriteLine($"Id: {workOrder.Id} ----- CustomerName: {workOrder.CustomerName} ----- Status: {workOrder.Status}");

            WorkOrderList.Add(workOrder);
        }

        return true;
    }

    public void PrintName()
    {
        Console.WriteLine(EnteredName);
    }
}
