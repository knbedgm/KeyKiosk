using KeyKiosk.Data;
using Microsoft.AspNetCore.Mvc;
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

    public async Task<List<WorkOrder>> GetAllAsync()
    {
        return await dbContext.WorkOrders.Include(w => w.Tasks).ToListAsync();
    }

    public async Task<WorkOrder?> GetByIdAsync(int id)
    {
        return await dbContext.WorkOrders.Include(w => w.Tasks)
                                   .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task AddAsync(WorkOrder order)
    {
        dbContext.WorkOrders.Add(order);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(WorkOrder order)
    {
        dbContext.WorkOrders.Update(order);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var order = await dbContext.WorkOrders.FindAsync(id);
        if (order != null)
        {
            dbContext.WorkOrders.Remove(order);
            await dbContext.SaveChangesAsync();
        }
    }
}
