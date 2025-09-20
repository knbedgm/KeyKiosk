
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyKiosk.Data
{
    public class DrawerLogEvent : ILogEvent
    {
        public int ID { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public int DrawerId { get; set; }
        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public required User User { set { UserId = value.Id; UserName = value.Name; } }
        public DrawerLogEventType EventType { get; set; }
        public string? RONumber { get; set; }
    }

    public enum DrawerLogEventType
    {
        InsertKey,
        RemoveKey,
        Open,
        Close
    }
}
