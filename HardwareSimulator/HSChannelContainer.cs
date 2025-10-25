using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace HardwareSimulator
{
	public class HSChannelContainer
	{
		public Channel<string> DrawerEvents = Channel.CreateUnbounded<string>();
		public Channel<string> RFIDEvents = Channel.CreateUnbounded<string>();

	}
}
