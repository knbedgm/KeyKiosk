using KeyKiosk.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KeyKiosk.Services;

public class WorkOrderService
{
	/// Set up dbContext
	public required ApplicationDbContext dbContext { get; set; }

	//encapsulate db operations for work orders
	//injected into the service so it can query the db
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

	//fetch all work orders
	public async Task<List<WorkOrder>> GetAllAsync()
	{
		return await dbContext.WorkOrders.Include(w => w.Tasks).ToListAsync();
	}

	//fetch work order by id
	public async Task<WorkOrder?> GetByIdAsync(int id)
	{
		return await dbContext.WorkOrders.Include(w => w.Tasks)
								   .FirstOrDefaultAsync(w => w.Id == id);
	}

	/*
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
    */
}
