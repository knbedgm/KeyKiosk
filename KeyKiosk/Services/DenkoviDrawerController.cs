
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace KeyKiosk.Services
{
    public class DenkoviDrawerController : IPhysicalDrawerController
    {

        SerialPort port;
        string portName;

        public DenkoviDrawerController(string portName)
        {
            this.portName = portName;
            buildSerial();


        }

        void buildSerial()
        {
            if (!(port is null))
            {
                port.Close();
                port.Dispose();
                port = null;
            }
            try
            {
                port = new SerialPort(portName, baudRate: 9600, Parity.None, dataBits: 8, StopBits.One);
                port.DiscardNull = false;
                port.Encoding = Encoding.Latin1;
                port.Open();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine($"Available serial ports are: [{String.Join(", ", SerialPort.GetPortNames())}]");
                throw;
            }
        }

        public async Task Open(int id)
        {
            if (id > 16 || id < 1) throw new ArgumentOutOfRangeException("id", "Drawer id must be between 1 and 16 inclusive.");
            byte[] buffer = new byte[5];
            id.TryFormat(buffer, out int written, "00");
            Debug.Assert(written == 2);
            buffer[2] = (byte)('+');
            buffer[3] = (byte)('/');
            buffer[4] = (byte)('/');
            Console.WriteLine(Encoding.Latin1.GetString(buffer));
            await Task.Delay(200);
            buffer[2] = (byte)('-');
            Console.WriteLine(Encoding.Latin1.GetString(buffer));
        }

        public Task OpenAll()
        {
            throw new NotImplementedException();
            //Encoding.
        }
    }
}