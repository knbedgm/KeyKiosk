namespace KeyKiosk.Data
{
    public interface ILogEvent
    {
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
    }
}
