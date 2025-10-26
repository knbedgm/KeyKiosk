using KeyKiosk.Data;

namespace KeyKiosk.Services;

public class PartTemplateService
{
    public required ApplicationDbContext dbContext { get; set; }

    public PartTemplateService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Get all part templates
    /// </summary>
    /// <returns></returns>
    public List<PartTemplate> GetAllPartTemplates()
    {
        return dbContext.PartTemplates
                        .OrderBy(t => t.Id)
                        .ToList();
    }

    /// <summary>
    /// Get a single part template by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public PartTemplate GetPartTemplateById(int id)
    {
        return dbContext.PartTemplates
                        .First(t => t.Id == id);
    }

    /// <summary>
    /// Add a new part template to the database
    /// </summary>
    /// <param name="template"></param>
    public void AddPartTemplate(PartTemplate template)
    {
        var newTemplate = new PartTemplate { PartName = template.PartName, Details = template.Details, CostCents = template.CostCents };
        dbContext.PartTemplates.Add(newTemplate);
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Update existing part template using id
    /// </summary>
    /// <param name="idToUpdate"></param>
    /// <param name="template"></param>
    public void UpdatePartTemplate(PartTemplate updatedTemplate)
    {
        var templateToUpdate = dbContext.PartTemplates.FirstOrDefault(t => t.Id == updatedTemplate.Id);
        if (templateToUpdate != null)
        {
            templateToUpdate.Details = updatedTemplate.Details;
            templateToUpdate.CostCents = updatedTemplate.CostCents;
        }
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Delete existing template using id
    /// </summary>
    /// <param name="idToDelete"></param>
    public void DeletePartTemplate(int idToDelete)
    {
        var templateToDelete = dbContext.PartTemplates.FirstOrDefault(t => t.Id == idToDelete);
        if (templateToDelete != null)
        {
            dbContext.PartTemplates.Remove(templateToDelete);
        }
        dbContext.SaveChanges();
    }
}
