namespace KeyKiosk.Services
{
    public class ScopedTest
    {
        public int val { get; set; }
        public ScopedTest()
        {
            var r = new Random();
            val = r.Next(100);
        }
    }
}
