using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimulator
{
	public static class LibEntry
	{
		public static HSChannelContainer Start()
		{
			var chanels = new HSChannelContainer();

			Thread thread = new Thread(new ThreadStart(() =>
			{
				// To customize application configuration such as set high DPI settings or default font,
				// see https://aka.ms/applicationconfiguration.
				ApplicationConfiguration.Initialize();
				Application.Run(new Form1(chanels));
			}));

			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();

			return chanels;
		}
	}
}
