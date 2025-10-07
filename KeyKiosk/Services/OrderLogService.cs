using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;

namespace KeyKiosk.Services
{
    public class OrderLogService
    {
        private readonly ApplicationDbContext _context;

        public OrderLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        // -------------------
        // READ all data from database
        // -------------------
        public async Task<List<WorkOrder>> GetAllOrdersAsync()
        {
            return await _context.WorkOrders
                                 .Include(o => o.Tasks)
                                 .ToListAsync();
        }

        public async Task<WorkOrder?> GetOrderByIdAsync(int id)
        {
            return await _context.WorkOrders
                                 .Include(o => o.Tasks)
                                 .FirstOrDefaultAsync(o => o.Id == id);
        }

        // -------------------
        // CREATE new work order or task
        // -------------------
        public async Task<WorkOrder> AddWorkOrderAsync(WorkOrder order)
        {
            _context.WorkOrders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<WorkOrderTask> AddTaskToOrderAsync(int orderId, WorkOrderTask task)
        {
            var order = await GetOrderByIdAsync(orderId);
            if (order == null) throw new InvalidOperationException("Order not found");

            order.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        // -------------------
        // UPDATE work order or task
        // -------------------
        public async Task<bool> UpdateTaskAsync(int orderId, WorkOrderTask updatedTask)
        {
            var order = await GetOrderByIdAsync(orderId);
            if (order == null) return false;

            var existingTask = order.Tasks.FirstOrDefault(t => t.Id == updatedTask.Id);
            if (existingTask == null) return false;

            existingTask.Description = updatedTask.Description;
            existingTask.Status = updatedTask.Status;
            existingTask.CostCents = updatedTask.CostCents;
            existingTask.StartDate = updatedTask.StartDate;
            existingTask.EndDate = updatedTask.EndDate;

            await _context.SaveChangesAsync();
            return true;
        }

        // -------------------
        // DELETE work order or task
        // -------------------
        public async Task<bool> DeleteWorkOrderAsync(int id)
        {
            var order = await GetOrderByIdAsync(id);
            if (order == null) return false;

            _context.WorkOrders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int orderId, int taskId)
        {
            var order = await GetOrderByIdAsync(orderId);
            if (order == null) return false;

            var task = order.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;

            order.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
