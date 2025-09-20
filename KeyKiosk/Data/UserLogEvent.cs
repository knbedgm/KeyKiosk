
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyKiosk.Data
{
    public class UserLogEvent : ILogEvent
    {
        public int ID { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public int ActingUserId { get; private set; }
        public string ActingUserName { get; private set; }
        public required User ActingUser { set { ActingUserId = value.Id; ActingUserName = value.Name; } }
        public int? SecondaryUserId { get; private set; }
        public string? SecondaryUserName { get; private set; }
        public User SecondaryUser { set { SecondaryUserId = value.Id; SecondaryUserName = value.Name; } }
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
