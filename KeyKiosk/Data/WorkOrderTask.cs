using System.ComponentModel.DataAnnotations.Schema;

namespace KeyKiosk.Data
{
	public class WorkOrderTask
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
		public string Description { get; set; } = "";
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
		public WorkOrderTaskStatusType Status { get; set; }
		public int CostCents { get; set; }
	}
	public enum WorkOrderTaskStatusType
	{
		WorkStarted,
		WorkFinished,
	}
}
