namespace KeyKiosk.Data
{
    public class WorkOrderTaskTemplate
    {
        public int Id { get; set; }
        public required string TaskDescription { get; set; }
        public int TaskCostCents { get; set; }
    }
}
