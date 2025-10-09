namespace KeyKiosk.Data
{
    public class UserLogEvent : ILogEvent
    {
        public int ID { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public int ActingUserId { get; private set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public string ActingUserName { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public required User ActingUser { set { ActingUserId = value.Id; ActingUserName = value.Name; } }
        public int? SecondaryUserId { get; private set; }
        public string? SecondaryUserName { get; private set; }
        public User SecondaryUser { set { SecondaryUserId = value.Id; SecondaryUserName = value.Name; } }
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
