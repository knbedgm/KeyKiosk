using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;

namespace KeyKiosk.Services;

public class WorkOrderService
{
    private readonly ApplicationDbContext _dbContext;

    public WorkOrderService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Get all work orders
    /// </summary>
    /// <returns></returns>
    public async Task<List<WorkOrder>> GetAllWorkOrdersAsync()
    {
        return await _dbContext.WorkOrders
                               .Include(w => w.Tasks)
                               .Include(w => w.Parts)
                               .OrderBy(w => w.StartDate)
                               .ToListAsync();
    }

    /// <summary>
    /// Get work orders by customer name
    /// </summary>
    /// <param name="customerName">Name of customer to get work orders for</param>
    /// <returns></returns>
    public async Task<List<WorkOrder>> GetWorkOrdersByCustomerNameAsync(string customerName)
    {
        return await _dbContext.WorkOrders
                               .Where(w => w.CustomerName.ToLower() == customerName.ToLower())
                               .Include(w => w.Tasks)
                               .Include(w => w.Parts)
                               .OrderBy(w => w.StartDate)
                               .ToListAsync();
    }

    /// <summary>
    /// Add a new work order to database
    /// </summary>
    /// <param name="workOrder">Work order values</param>
    /// <returns></returns>
    public async Task<WorkOrder> AddWorkOrderAsync(WorkOrder workOrder)
    {
        if (workOrder.Tasks == null)
            workOrder.Tasks = new List<WorkOrderTask>();

        if (workOrder.Parts == null)
            workOrder.Parts = new List<WorkOrderPart>();

        _dbContext.WorkOrders.Add(workOrder);
        await _dbContext.SaveChangesAsync();
        return workOrder;
    }

    /// <summary>
    /// Get all work orders including associated tasks and parts
    /// </summary>
    /// <returns></returns>
    public async Task<List<WorkOrder>> GetAllAsync()
    {
        return await _dbContext.WorkOrders.Include(w => w.Tasks).Include(w => w.Parts).ToListAsync();
    }


    /// <summary>
    /// Get work order by id
    /// </summary>
    /// <param name="id">Id of work order to get</param>
    /// <returns></returns>
    public async Task<WorkOrder?> GetByIdAsync(int id)
    {
        return await _dbContext.WorkOrders.Include(w => w.Tasks).Include(w => w.Parts)
                                   .FirstOrDefaultAsync(w => w.Id == id);
    }

    /// <summary>
    /// Update an existing work order
    /// </summary>
    /// <param name="workOrder">Work order to update including new values</param>
    /// <returns></returns>
    public async Task UpdateWorkOrderAsync(WorkOrder workOrder)
    {
        // Get the tracked entity if it exists
        var trackedEntity = await _dbContext.WorkOrders
                                            .Include(w => w.Tasks)
                                            .Include(w => w.Parts)
                                            .FirstOrDefaultAsync(w => w.Id == workOrder.Id);

        if (trackedEntity != null)
        {
            // Update only the properties you want to change
            trackedEntity.CustomerName = workOrder.CustomerName;
            trackedEntity.VehiclePlate = workOrder.VehiclePlate;
            trackedEntity.StartDate = workOrder.StartDate;
            trackedEntity.EndDate = workOrder.EndDate;
            trackedEntity.Status = workOrder.Status;
            trackedEntity.Details = workOrder.Details;

            // Optional: update tasks manually if needed
            // trackedEntity.Tasks = workOrder.Tasks;

            await _dbContext.SaveChangesAsync();
        }
    }


    /// <summary>
    /// Delete a work order using id
    /// </summary>
    /// <param name="workOrderId">Id of work order to delete</param>
    /// <returns></returns>
    public async Task DeleteWorkOrderAsync(int workOrderId)
    {
        var workOrder = await _dbContext.WorkOrders
                                        .Include(w => w.Tasks)
                                        .Include(w => w.Parts)
                                        .FirstOrDefaultAsync(w => w.Id == workOrderId);
        if (workOrder != null)
        {
            _dbContext.WorkOrders.Remove(workOrder);
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Get work orders by vehicle plate
    /// </summary>
    /// <param name="plate">License plate of work order to get</param>
    /// <returns></returns>
    public async Task<List<WorkOrder>> GetWorkOrdersByVehiclePlateAsync(string plate)
    {
        return await _dbContext.WorkOrders
                               .Where(w => w.VehiclePlate.ToLower() == plate.ToLower())
                               .Include(w => w.Tasks)
                               .Include(w => w.Parts)
                               .OrderBy(w => w.StartDate)
                               .ToListAsync();
    }

    /// <summary>
    /// Get work orders between two dates
    /// </summary>
    /// <param name="startDate">Earliest starting date of work order</param>
    /// <param name="endDate">Latest starting date of work order</param>
    /// <returns></returns>
    public async Task<List<WorkOrder>> GetWorkOrderByDatePeriod(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await _dbContext.WorkOrders
                               .Include(w => w.Tasks)
                               .Include(w => w.Parts)
                               .Where(w => w.StartDate.HasValue && w.StartDate.Value >= startDate && w.StartDate.Value <= endDate)
                               .ToListAsync();
    }
}
