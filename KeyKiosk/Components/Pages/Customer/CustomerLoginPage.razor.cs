using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyKiosk.Components.Pages.Customer;

public partial class CustomerLoginPage
{
    ApplicationDbContext dbContext;
    DatabaseService databaseService;
    private string CustomerName { get; set; }

    List<string> customerList { get; set; } = new List<string>();

    public CustomerLoginPage(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        databaseService = new DatabaseService(dbContext);
    }

    protected override async Task OnInitializedAsync()
    {
        await GetAllCustomers();

        CustomerName = "sdf";

    }

    public Task GetAllCustomers()
    {
        var customers = from workOrder in dbContext.WorkOrders
                         where workOrder.CustomerName == CustomerName
                         select workOrder.CustomerName;

        foreach (string customer in customers)
        {
            Console.WriteLine($"CustomerName: {customer}");

            customerList.Add(customer);
        }

        return Task.CompletedTask;
    }

    public void GoCustomerHomePage()
    {
        if (customerList.Contains(CustomerName))
        {

        }
    }
}
