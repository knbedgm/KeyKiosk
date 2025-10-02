using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KeyKiosk.Services;

public class WorkOrderService
{
    public required ApplicationDbContext dbContext { get; set; }

    public WorkOrderService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public List<WorkOrder> GetWorkOrdersByCustomerName(string customerName)
    {
        List<WorkOrder> workOrders = dbContext.WorkOrders
                                  .Where(w => w.CustomerName == customerName)
                                  .Include(w => w.Tasks)
                                  .OrderBy(w => w.StartDate)
                                  .ToList();

        foreach (WorkOrder workOrder in workOrders)
        {
            Console.WriteLine($"Id: {workOrder.Id}");
            Console.WriteLine($"CustomerName: {workOrder.CustomerName}");
            Console.WriteLine($"Status: {workOrder.Status}");
        }

        return workOrders;
    }
}
