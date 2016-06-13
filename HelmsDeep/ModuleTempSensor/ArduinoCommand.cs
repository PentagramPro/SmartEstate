using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleTempSensor
{
    public class ArduinoCommand
    {
        public char Command;
        public List<string> Parameters;

        public byte[] Build()
        {
            string packet = Command.ToString();

            packet = Parameters.Aggregate(packet, (current, p) => current + ("," + p));
            packet += ";";
            packet = ":" + packet.Length.ToString("X2") + packet;
            return Encoding.ASCII.GetBytes(packet);
        }
    }
}
