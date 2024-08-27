
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyKiosk.Data
{
    public class DrawerLogEvent : ILogEvent
    {
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        public Drawer Drawer { get; set; }
        public User User { get; set; }
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
