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
		// Context for communicating with PC/SC subsystem
		CardContext context = new CardContext();

		// Monitor for card insertion/removal events
		StatusChangeMonitor changeMonitor;

		// Configured reader name from configuration file
		public string readerName { get; private set; }

		// Public event fired when a card is successfully scanned
		public event EventHandler<OnCardScannedEventArgs>? OnCardScannedEvent;

		public PCSCReaderService(IConfiguration conf)
		{
			// Load reader name from config
			this.readerName = conf.GetValue<string>("RFIDReaderName")!;
			SetupContext();
		}

		// Destructor ensures PC/SC context is cleaned up
		~PCSCReaderService()
		{
			this.context.Release();
		}

		/// <summary>
		/// Initializes PC/SC context, configures the reader, disables beep, 
		/// and sets up card insertion monitoring.
		/// </summary>
		private void SetupContext()
		{
			// Initialize PC/SC context
			context.Establish().ThrowIfNotSuccess();
			context.ListReaders(null).ThrowIfNotSuccess();

			// Open direct mode channel to configure the reader
			var channel = new CardChannel(context, readerName);

			// Mode 3 = DIRECT, allows sending control commands to the reader
			channel.Connect((ShareMode)3, Protocol.Unset).ThrowIfNotSuccess();

			// Disable read beep using vendor-specific control command
			byte[] resp = [];
			channel.Control(0x003136b0, [0xff, 0x00, 0x52, 0x00, 0x00], out resp).ThrowIfNotSuccess();

			// Leave card in its current state
			channel.Disconnect(Disposition.LeaveCard);

			// Begin monitoring for card insertions
			changeMonitor = new StatusChangeMonitor(context, [readerName]);
			changeMonitor.OnCardInsertionEvent += (object? sender, OnCardInsertionEventArgs e) =>
			{
				// Handle the card insertion event
				HandleNewCardEvent(e.ReaderState.ReaderName);
			};

			// Start listening for events
			changeMonitor.Start();
		}

		/// <summary>
		/// Sends a beep command to the reader (can be called after a successful scan).
		/// </summary>
		private void ReaderBeep(CardChannel channel)
		{
			byte[] resp = [];
			// Control code triggers audible beep
			channel.Control(0x003136b0, [0xff, 0x00, 0x40, 0b10100000, 0x04, 1, 0, 1, 1], out resp);
		}

		/// <summary>
		/// Reads the UID of the card currently connected to the channel.
		/// </summary>
		private string ReadUID(CardChannel channel)
		{
			// APDU command for reading card UID for ISO14443 readers
			var tx = new CommandAPDU([0xff, 0xca, 0, 0, 0]);
			var rx = new ResponseAPDU();

			// Send APDU and receive response
			channel.Transmit(tx, rx);

			// Convert UID bytes to uppercase hex
			return Convert.ToHexString(rx.Udr).ToUpperInvariant();
		}

		/// <summary>
		/// Handles the workflow when a card is detected in the reader.
		/// </summary>
		private void HandleNewCardEvent(string readerName)
		{
			// Open a shared channel for normal card communication
			var chan = new CardChannel(context, readerName);
			chan.Connect(ShareMode.Shared, Protocol.Any).ThrowIfNotSuccess();

			// Read card UID
			var uid = ReadUID(chan);
			Console.WriteLine("uid scanned: " + uid);

			// Ignore empty or all-zero UIDs
			if (!Regex.IsMatch(uid, "^0+$"))
			{
				// Fire event in background thread to avoid blocking
				Task.Run(() =>
				{
					this.OnCardScannedEvent?.Invoke(this, new OnCardScannedEventArgs(uid));
				});

				// Provide audible confirmation
				ReaderBeep(chan);
			}

			// Leave card in reader after disconnecting
			chan.Disconnect(Disposition.LeaveCard);
		}

		/// <summary>
		/// Static helper for listing all available PC/SC reader names.
		/// </summary>
		public static List<string> GetReaderNames()
		{
			var readers = new List<string>();

			var context = new CardContext();
			context.Establish();
			context.ListReaders(null);

			// Add each reader name to list
			readers.AddRange(context.Readers);

			context.Release();
			return readers;
		}
	}

	/// <summary>
	/// Event argument wrapper containing scanned card ID.
	/// </summary>
	public class OnCardScannedEventArgs(string CardId)
	{
		public string CardId { get; } = CardId;
	}
}
