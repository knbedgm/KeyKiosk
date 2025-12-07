using System.Threading.Channels;

namespace HardwareSimulator
{
	public partial class Form1 : Form
	{
		HSChannelContainer channels;
		public Form1(HSChannelContainer channels)
		{
			InitializeComponent();
			this.channels = channels;

			Task.Run(async () =>
			{
				await foreach (var e in channels.DrawerEvents.Reader.ReadAllAsync())
				{
					this.Invoke(() =>
					{
						this.listBox1.Items.Insert(0, e);
						this.listBox1.Invalidate();
					});
				}
			});
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var card = comboBox1.Text.ToUpperInvariant();

			comboBox1.Items.Remove(card);
			comboBox1.Items.Insert(0, card);
			comboBox1.Text = "";

			channels.RFIDEvents.Writer.WriteAsync(card);
		}

		private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13) // Enter
			{
				button1_Click(sender, e);
			}
		}
	}
}
