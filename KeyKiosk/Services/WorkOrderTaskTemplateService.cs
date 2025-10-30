using KeyKiosk.Data;

namespace KeyKiosk.Services;

public class WorkOrderTaskTemplateService
{
    // Set up dbContext
    public required ApplicationDbContext dbContext { get; set; }

    public WorkOrderTaskTemplateService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Get all work order task templates
    /// </summary>
    /// <returns></returns>
    public List<WorkOrderTaskTemplate> GetAllTaskTemplates()
    {
        return dbContext.WorkOrderTaskTemplates
                        .OrderBy(t => t.Id)
                        .ToList();
    }

    /// <summary>
    /// Get a single work order task template by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public WorkOrderTaskTemplate GetWorkOrderTaskTemplateById(int id)
    {
        return dbContext.WorkOrderTaskTemplates
                        .First(t => t.Id == id);
    }

    /// <summary>
    /// Add a new work order task template to the database
    /// </summary>
    /// <param name="template"></param>
    public void AddWorkOrderTaskTemplate(WorkOrderTaskTemplate template)
    {
        var newTemplate = new WorkOrderTaskTemplate { TaskTitle = template.TaskTitle, TaskDetails = template.TaskDetails, TaskCostCents = template.TaskCostCents, ExpectedHoursForCompletion = template.ExpectedHoursForCompletion };
        dbContext.WorkOrderTaskTemplates.Add(newTemplate);
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Update existing work order template using id
    /// </summary>
    /// <param name="idToUpdate"></param>
    /// <param name="template"></param>
    public void UpdateWorkOrderTaskTemplate(WorkOrderTaskTemplate updatedTemplate)
    {
        var templateToUpdate = dbContext.WorkOrderTaskTemplates.FirstOrDefault(t => t.Id == updatedTemplate.Id);
        if (templateToUpdate != null)
        {
            templateToUpdate.TaskDetails = updatedTemplate.TaskDetails;
            templateToUpdate.TaskCostCents = updatedTemplate.TaskCostCents;
            templateToUpdate.ExpectedHoursForCompletion = updatedTemplate.ExpectedHoursForCompletion;
        }
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Delete existing template using id
    /// </summary>
    /// <param name="idToDelete"></param>
    public void DeleteWorkOrderTaskTemplate(int idToDelete)
    {
        var templateToDelete = dbContext.WorkOrderTaskTemplates.FirstOrDefault(t => t.Id == idToDelete);
        if (templateToDelete != null)
        {
            dbContext.WorkOrderTaskTemplates.Remove(templateToDelete);
        }
        dbContext.SaveChanges();
    }
}
