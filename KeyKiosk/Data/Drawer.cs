namespace KeyKiosk.Data
{
    public class Drawer
    {
        public int Id { get; set; }
        public WorkOrder? CurrentWorkorder { get; set; }
        public string? RFIDUid { get; set; }
        public bool Occupied { get; set; }
    }
}
