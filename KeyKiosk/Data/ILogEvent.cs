namespace KeyKiosk.Data
{
    public interface ILogEvent
    {
        public int ID { get; set; }
        public DateTimeOffset DateTime { get; set; }
    }
}
