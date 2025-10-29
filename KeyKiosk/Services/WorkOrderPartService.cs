﻿using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;

namespace KeyKiosk.Services;

public class WorkOrderPartService
{
    // Set up dbContext
    public required ApplicationDbContext dbContext { get; set; }

    public WorkOrderPartService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Get all work order parts
    /// </summary>
    /// <returns></returns>
    public List<WorkOrderPart> GetAllParts()
    {
        return dbContext.WorkOrderParts
                        .OrderBy(t => t.Id)
                        .ToList();
    }

    /// <summary>
    /// Get work order parts by work order id
    /// </summary>
    /// <param name="workOrderId"></param>
    /// <returns></returns>
    public List<WorkOrderPart> GetWorkOrderPartsByWorkOrderId(int workOrderId)
    {
        WorkOrder workOrder = dbContext.WorkOrders
                        .First(w => w.Id == workOrderId);

        List<WorkOrderPart> WorkOrderParts = new List<WorkOrderPart>();

        if (workOrder != null)
        {
            foreach (WorkOrderPart part in workOrder.Parts)
            {
                WorkOrderParts.Add(part);
            }
        }

        return WorkOrderParts;
    }

    /// <summary>
    /// Get a single work order part by id
    /// </summary>
    /// <param name="partId"></param>
    /// <returns></returns>
    public WorkOrderPart GetWorkOrderPartById(int partId)
    {
        return dbContext.WorkOrderParts
                        .First(p => p.Id == partId);
    }


    public class AddWorkOrderPartModel
    {
        public string PartName { get; set; } = "";
        public string Details { get; set; } = "";
        public int CostCents { get; set; }
    }

    /// <summary>
    /// Add a new work order part to the database
    /// </summary>
    /// <param name="newPart"></param>
    /// 
    public void AddWorkOrderPart(int workOrderId, AddWorkOrderPartModel newPart)
    {
        var workOrder = dbContext.WorkOrders.FirstOrDefault(p => p.Id == workOrderId);
        if (workOrder == null)
        {
            throw new ArgumentException($"Work order with id ${workOrderId} doesn't exist", "workOrderId");
        }

        var partToAdd = new WorkOrderPart
        {
            PartName = newPart.PartName,
            Details = newPart.Details,
            CostCents = newPart.CostCents,
            WorkOrder = workOrder,
        };

        dbContext.WorkOrderParts.Add(partToAdd);
        dbContext.SaveChanges();
    }

    public class UpdateWorkOrderPartModel
    {
        public string PartName { get; set; } = "";
        public string Details { get; set; } = "";
        public int CostCents { get; set; }
    }

    /// <summary>
    /// Update existing work order part using id
    /// </summary>
    /// <param name="partId"></param>
    /// <param name="updatedPart"></param>
    public void UpdateWorkOrderPart(int partId, UpdateWorkOrderPartModel updatedPart)
    {
        var partToUpdate = dbContext.WorkOrderParts.FirstOrDefault(p => p.Id == partId);
        if (partToUpdate != null)
        {
            partToUpdate.Details = updatedPart.Details;
            partToUpdate.CostCents = updatedPart.CostCents;
        }
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Deletes work order part using id
    /// </summary>
    /// <param name="idToDelete"></param>
    public void DeleteWorkOrderPart(int idToDelete)
    {
        var partToDelete = dbContext.WorkOrderParts.FirstOrDefault(p => p.Id == idToDelete);
        if (partToDelete != null)
        {
            dbContext.WorkOrderParts.Remove(partToDelete);
        }
        dbContext.SaveChanges();
    }

    // Get parts between two dates
    public async Task<List<WorkOrderPart>> GetPartsByDatePeriod(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        List<WorkOrder> workOrders = await dbContext.WorkOrders
                               .Where(w => w.StartDate.HasValue && w.StartDate.Value >= startDate && w.StartDate.Value <= endDate)
                               .Include(p => p.Parts)
                               .ToListAsync();

        List<WorkOrderPart> partList = new List<WorkOrderPart>();
        foreach (WorkOrder workOrder in workOrders)
        {
            foreach (WorkOrderPart part in workOrder.Parts)
            {
                partList.Add(part);
            }
        }
        return partList;
    }
}
