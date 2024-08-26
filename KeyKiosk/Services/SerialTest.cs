using KeyKiosk.Components;
using System;
using System.IO.Ports;

namespace KeyKiosk.Services
{
    public class SerialTest
    {
            SerialPort _serialPort;
        public SerialTest()
        {

            _serialPort = new SerialPort();

            _serialPort.BaudRate = 115200;
            _serialPort.PortName = "COM4";
        }

        public string[] GetPorts()
        {
            return SerialPort.GetPortNames();
        }

        public void Write(string val)
        {
            _serialPort.Open();
            _serialPort.Write("\x1b@");
            _serialPort.WriteLine(val);
            _serialPort.Close();
        }
    }
}
