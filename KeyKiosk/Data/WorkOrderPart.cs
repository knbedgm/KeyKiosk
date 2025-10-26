namespace KeyKiosk.Data;

public class WorkOrderPart
{
    public int Id { get; set; }
    public required string PartName { get; set; }
    public string Details { get; set; } = "";
    public int CostCents { get; set; }
    public int WorkOrderId { get; set; }
    public required virtual WorkOrder WorkOrder { get; set; }
}
