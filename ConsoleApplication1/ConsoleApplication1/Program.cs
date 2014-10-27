using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static SerialPort Port;
        static void Main(string[] args)
        {
            Port = new SerialPort("COM9", 115200, Parity.None, 8, StopBits.One);
            Port.Open();

            byte[] data = new byte[] { 0xF0, 0xF0, 0xF2, 0xD };
            Port.Write(data, 0, data.Length);

            Port.Write("HM" + (char)0xd);

            while (true)
            {
                if (Port.BytesToRead > 0)
                {
                    int d = Port.ReadByte();
                    Console.WriteLine(((char)d).ToString() + " - " + d.ToString("X2"));
                }
            }

        }
    }
}
