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

    // Get all work orders
    public async Task<List<WorkOrder>> GetAllWorkOrdersAsync()
    {
        return await _dbContext.WorkOrders
                               .Include(w => w.Tasks)
                               .OrderBy(w => w.StartDate)
                               .ToListAsync();
    }

    // Get work orders by customer name
    public async Task<List<WorkOrder>> GetWorkOrdersByCustomerNameAsync(string customerName)
    {
        return await _dbContext.WorkOrders
                               .Where(w => w.CustomerName.ToLower() == customerName.ToLower())
                               .Include(w => w.Tasks)
                               .OrderBy(w => w.StartDate)
                               .ToListAsync();
    }

    // Add a new work order
    public async Task<WorkOrder> AddWorkOrderAsync(WorkOrder workOrder)
    {
        if (workOrder.Tasks == null)
            workOrder.Tasks = new List<WorkOrderTask>();

        _dbContext.WorkOrders.Add(workOrder);
        await _dbContext.SaveChangesAsync();
        return workOrder;
    }

    //fetch all work orders
    public async Task<List<WorkOrder>> GetAllAsync()
    {
        return await _dbContext.WorkOrders.Include(w => w.Tasks).ToListAsync();
    }


    //fetch work order by id
    public async Task<WorkOrder?> GetByIdAsync(int id)
    {
        return await _dbContext.WorkOrders.Include(w => w.Tasks)
                                   .FirstOrDefaultAsync(w => w.Id == id);
    }

    // Update an existing work order
    public async Task UpdateWorkOrderAsync(WorkOrder workOrder)
    {
        // Get the tracked entity if it exists
        var trackedEntity = await _dbContext.WorkOrders
                                            .Include(w => w.Tasks)
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


    // Delete a work order
    public async Task DeleteWorkOrderAsync(int workOrderId)
    {
        var workOrder = await _dbContext.WorkOrders
                                        .Include(w => w.Tasks)
                                        .FirstOrDefaultAsync(w => w.Id == workOrderId);
        if (workOrder != null)
        {
            _dbContext.WorkOrders.Remove(workOrder);
            await _dbContext.SaveChangesAsync();
        }
    }
    // Get work orders by vehicle plate
    public async Task<List<WorkOrder>> GetWorkOrdersByVehiclePlateAsync(string plate)
    {
        return await _dbContext.WorkOrders
                               .Where(w => w.VehiclePlate.ToLower() == plate.ToLower())
                               .Include(w => w.Tasks)
                               .OrderBy(w => w.StartDate)
                               .ToListAsync();
    }
}