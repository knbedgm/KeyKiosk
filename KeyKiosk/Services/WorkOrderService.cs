using KeyKiosk.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KeyKiosk.Services
{
    public class WorkOrderService
    {
        private readonly ApplicationDbContext _db;

        public WorkOrderService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<WorkOrder>> GetAllAsync()
        {
            return await _db.WorkOrders.Include(w => w.Tasks).ToListAsync();
        }

        public async Task<WorkOrder?> GetByIdAsync(int id)
        {
            return await _db.WorkOrders.Include(w => w.Tasks)
                                       .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task AddAsync(WorkOrder order)
        {
            _db.WorkOrders.Add(order);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkOrder order)
        {
            _db.WorkOrders.Update(order);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _db.WorkOrders.FindAsync(id);
            if (order != null)
            {
                _db.WorkOrders.Remove(order);
                await _db.SaveChangesAsync();
            }
        }
    }
}
