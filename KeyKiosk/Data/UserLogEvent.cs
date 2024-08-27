
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyKiosk.Data
{
    public class UserLogEvent : ILogEvent
    {
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        public User ActingUser { get; set; }
        public string? SecondaryUserName { get; set; }
        [Column(TypeName = "TEXT")]
        public UserLogEventType EventType { get; set; }
    }

    public enum UserLogEventType
    {
        Create,
        Login,
        Delete,
        ChangePin,
    }
}
