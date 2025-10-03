using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KeyKiosk.Services;

public class WorkOrderService
{
    /// Set up dbContext
    public required ApplicationDbContext dbContext { get; set; }

    public WorkOrderService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Get all work orders for a customer name
    /// </summary>
    /// <param name="customerName"></param>
    /// <returns></returns>
    public List<WorkOrder> GetWorkOrdersByCustomerName(string customerName)
    {
        return dbContext.WorkOrders
                        .Where(w => w.CustomerName == customerName)
                        .Include(w => w.Tasks)
                        .OrderBy(w => w.StartDate)
                        .ToList();
    }
}
