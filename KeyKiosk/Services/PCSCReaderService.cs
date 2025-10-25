using System.Text.RegularExpressions;
using WSCT.Core.Fluent.Helpers;
using WSCT.ISO7816;
using WSCT.Wrapper;
using WSCT.Wrapper.Desktop.Core;
using WSCT.Wrapper.Desktop.Core.SCardControl;

namespace KeyKiosk.Services
{
	public class PCSCReaderService : IRFIDReader
	{

		CardContext context = new CardContext();
		StatusChangeMonitor changeMonitor;

		public string readerName { get; private set; }

		public event EventHandler<OnCardScannedEventArgs>? OnCardScannedEvent;

		public PCSCReaderService(IConfiguration conf)
		{

			this.readerName = conf.GetValue<string>("RFIDReaderName")!;
			SetupContext();

		}

		~PCSCReaderService()
		{
			this.context.Release();
		}

		private void SetupContext()
		{
			context.Establish().ThrowIfNotSuccess();
			context.ListReaders(null).ThrowIfNotSuccess();

			// disable read beep
			var channel = new CardChannel(context, readerName);

			// mode 3 is Direct
			channel.Connect((ShareMode)3, Protocol.Unset).ThrowIfNotSuccess();
			byte[] resp = [];
			channel.Control(0x003136b0, [0xff, 0x00, 0x52, 0x00, 0x00], out resp).ThrowIfNotSuccess(); // disable beep on read

			channel.Disconnect(Disposition.LeaveCard);

			changeMonitor = new StatusChangeMonitor(context, [readerName]);
			changeMonitor.OnCardInsertionEvent += (object? sender, OnCardInsertionEventArgs e) =>
			{
				//Console.WriteLine("Ins event: " + e.ReaderState.ToString());
				HandleNewCardEvent(e.ReaderState.ReaderName);
			};
			changeMonitor.Start();
		}

		private void ReaderBeep(CardChannel channel)
		{
			byte[] resp = [];
			channel.Control(0x003136b0, [0xff, 0x00, 0x40, 0b10100000, 0x04, 1, 0, 1, 1], out resp);//.ThrowIfNotSuccess(); // beep
		}

		private string ReadUID(CardChannel channel)
		{
			var tx = new CommandAPDU([0xff, 0xca, 0, 0, 0]);
			var rx = new ResponseAPDU();

			channel.Transmit(tx, rx);

			return Convert.ToHexString(rx.Udr).ToUpperInvariant();
		}

		private void HandleNewCardEvent(string readerName)
		{
			var chan = new CardChannel(context, readerName);
			chan.Connect(ShareMode.Shared, Protocol.Any).ThrowIfNotSuccess();

			var uid = ReadUID(chan);
				Console.WriteLine("uid scanned: " + uid);
			if (!Regex.IsMatch(uid, "^0+$"))
			{
				this.OnCardScannedEvent?.Invoke(this, new OnCardScannedEventArgs(uid));

				ReaderBeep(chan);
			}

			chan.Disconnect(Disposition.LeaveCard);
		}

		public static List<string> GetReaderNames()
		{
			var readers = new List<string>();

			var context = new CardContext();
			context.Establish();
			context.ListReaders(null);
			readers.AddRange(context.Readers);
			context.Release();

			return readers;
		}
	}

	public class OnCardScannedEventArgs(string CardId)
	{
		public string CardId { get; } = CardId;
	}
}
