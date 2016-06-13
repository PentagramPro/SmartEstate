using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using RJCP.IO.Ports;

namespace ModuleTempSensor
{
    public class ArduinoTransport
    {
        // :LLC,param1,param2;
        // LL - Length including command, parameters and terminator ';' (ASCII HEX, ex: FE)
        // C - Command character
        private static Logger log = LogManager.GetCurrentClassLogger();

        private SerialPortStream port;

        public ArduinoTransport(string portName, int baudrate)
        {
            port = new SerialPortStream(portName, baudrate);
        }

        public void Open()
        {
            port.Open();
            port.ReadTimeout = 1000;
        }

        public void Close()
        {
            port.Close();
        }


        public void SendReceive(ArduinoCommand cmd, out ArduinoCommand resp)
        {
            byte[] packet = cmd.Build();
            port.Flush();
            log.Info("Отправляем команду: {0}",Encoding.ASCII.GetString(packet));
            port.Write(packet,0,packet.Length);
            int read = port.Read(packet, 0, 3);
            if (read != 3)
            {
                log.Error("Не удалось прочитать заголовок команды");
                throw new IOException();
            }
            if (packet[0] != ':')
            {
                log.Error("Заголовок команды содержит неверный стартовый символ");
                throw new IOException();
            }
            string strLen = Encoding.ASCII.GetString(packet, 1, 2);
            int len = int.Parse(strLen,NumberStyles.AllowHexSpecifier);
            packet = new byte[len];
            read = port.Read(packet, 0, len);
            if (read != len)
            {
                log.Error("Не удалось прочитать команду: неправильная длина");
                throw new IOException();
            }
            string strResp = Encoding.ASCII.GetString(packet,1,packet.Length-2);

            resp = new ArduinoCommand
            {
                Command = Encoding.ASCII.GetString(packet, 0, 1)[0],
                Parameters = new List<string>(strResp.Split(new  [] { ','},StringSplitOptions.RemoveEmptyEntries))
            };

            log.Info("Получена команда {0} {1}", resp.Command, strResp);
        }
    }
}
