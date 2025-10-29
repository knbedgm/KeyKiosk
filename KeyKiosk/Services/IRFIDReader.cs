namespace KeyKiosk.Services
{
	public interface IRFIDReader
	{

		public event EventHandler<OnCardScannedEventArgs>? OnCardScannedEvent;
	}
}
