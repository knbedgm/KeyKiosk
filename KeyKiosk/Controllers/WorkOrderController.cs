using KeyKiosk.Data;
using Microsoft.AspNetCore.Mvc;

namespace KeyKiosk.Controllers
{
    /// <summary>
    /// Controller for managing work orders in the system.
    /// Principal Author: Hillary
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WorkOrderController : ControllerBase
    {
        private static readonly List<WorkOrder> _workOrders = new();

        // GET: api/workorder
        [HttpGet]
        public ActionResult<IEnumerable<WorkOrder>> GetAll()
        {
            return Ok(_workOrders);
        }

        // GET: api/workorder/{id}
        [HttpGet("{id}")]
        public ActionResult<WorkOrder> GetById(int id)
        {
            var workOrder = _workOrders.FirstOrDefault(w => w.Id == id);
            if (workOrder == null)
                return NotFound();

            return Ok(workOrder);
        }

        // POST: api/workorder
        [HttpPost]
        public ActionResult<WorkOrder> Create([FromBody] WorkOrder workOrder)
        {
            workOrder.Id = _workOrders.Count > 0 ? _workOrders.Max(w => w.Id) + 1 : 1;
            _workOrders.Add(workOrder);

            return CreatedAtAction(nameof(GetById), new { id = workOrder.Id }, workOrder);
        }

        // PUT: api/workorder/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] WorkOrder updatedWorkOrder)
        {
            var existing = _workOrders.FirstOrDefault(w => w.Id == id);
            if (existing == null)
                return NotFound();

            existing.CustomerName = updatedWorkOrder.CustomerName;
            existing.StartDate = updatedWorkOrder.StartDate;
            existing.EndDate = updatedWorkOrder.EndDate;
            existing.Status = updatedWorkOrder.Status;
            existing.Details = updatedWorkOrder.Details;
            existing.Tasks = updatedWorkOrder.Tasks;

            return NoContent();
        }

        // DELETE: api/workorder/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var workOrder = _workOrders.FirstOrDefault(w => w.Id == id);
            if (workOrder == null)
                return NotFound();

            _workOrders.Remove(workOrder);
            return NoContent();
        }
    }
}
