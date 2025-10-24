using HardwareSimulator;

namespace KeyKiosk.Services
{
	public class HSimService : IRFIDReader, IPhysicalDrawerController
	{
		public event EventHandler<OnCardScannedEventArgs>? OnCardScannedEvent;

		private HSChannelContainer Channels;
		public HSimService() {
			this.Channels = HardwareSimulator.LibEntry.Start();

			Task.Run(async () =>
			{
				await foreach (var e in Channels.RFIDEvents.Reader.ReadAllAsync())
				{
					this.OnCardScannedEvent?.Invoke(this, new OnCardScannedEventArgs(e.ToUpperInvariant()));
				}
			});
		}

		Task IPhysicalDrawerController.Open(int id)
		{
			Channels.DrawerEvents.Writer.WriteAsync($"Drawer #{id} opened.");
			return Task.CompletedTask;
		}

		Task IPhysicalDrawerController.OpenAll()
		{
			Channels.DrawerEvents.Writer.WriteAsync($"All drawers opened.");
			return Task.CompletedTask;
		}
	}
}
