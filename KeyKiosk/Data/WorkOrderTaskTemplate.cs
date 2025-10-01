using System.ComponentModel.DataAnnotations.Schema;

namespace KeyKiosk.Data
{
    public class WorkOrderTaskTemplate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TaskDescription { get; set; } = "";
        public int TaskCostCents { get; set; }
    }
}
