namespace KeyKiosk.Data
{
    public class DrawerLogEvent : ILogEvent
    {
        public int ID { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public int DrawerId { get; set; }
        public int UserId { get; private set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public string UserName { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public required User User { set { UserId = value.Id; UserName = value.Name; } }
        public DrawerLogEventType EventType { get; set; }
        public string? WorkorderNumber { get; set; }
    }

    public enum DrawerLogEventType
    {
        InsertKey,
        RemoveKey,
        Open,
        Close
    }
}
