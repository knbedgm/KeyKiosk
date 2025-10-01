using KeyKiosk.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KeyKiosk.Services;

public class DatabaseService
{
    ApplicationDbContext dbContext;

    public DatabaseService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void GetWorkOrderByCustomerName(string customerName)
    {
        var workOrders = dbContext.WorkOrders
            .Where(w => w.CustomerName == customerName)
            .OrderBy(w => w.StartDate);

        foreach (WorkOrder workOrder in workOrders)
        {
            Console.WriteLine($"Id: {workOrder.Id}");
            Console.WriteLine($"CustomerName: {workOrder.CustomerName}");
            Console.WriteLine($"Status: {workOrder.Status}");
        }
    }
}
