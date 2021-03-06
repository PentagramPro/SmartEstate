﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public bool ReadPort(byte[] buf, int len, long timeout)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int resLen = 0;
            while (resLen < len)
            {
                resLen += port.Read(buf, resLen, len-resLen);
                if (stopWatch.ElapsedMilliseconds > timeout)
                    break;
            }

            return resLen == len;
        }
        public void SendReceive(ArduinoCommand cmd, out ArduinoCommand resp)
        {
            const long timeout = 2000;
            byte[] packet = cmd.Build();
            port.ReadExisting();
            log.Info("Отправляем команду: {0}",Encoding.ASCII.GetString(packet));
            port.Write(packet,0,packet.Length);
            
            if (!ReadPort(packet, 3, timeout))
            {
                log.Error("Не удалось прочитать заголовок команды");
                port.Flush();
                throw new IOException();
            }
            if (packet[0] != ':')
            {
                log.Error("Заголовок команды содержит неверный стартовый символ");
                port.Flush();
                throw new IOException();
            }
            string strLen = Encoding.ASCII.GetString(packet, 1, 2);
            int len = int.Parse(strLen,NumberStyles.AllowHexSpecifier);
            string strResp = "";
            if (len > 0)
            {
                packet = new byte[len];
                if (!ReadPort(packet, len, timeout))
                {
                    log.Error("Не удалось прочитать команду: неправильная длина");
                    port.Flush();
                    throw new IOException();
                }
                strResp = Encoding.ASCII.GetString(packet, 1, packet.Length - 2);
            }
            resp = new ArduinoCommand
            {
                Command = Encoding.ASCII.GetString(packet, 0, 1)[0],
                Parameters = new List<string>(strResp.Split(new  [] { ','},StringSplitOptions.RemoveEmptyEntries))
            };

            log.Info("Получена команда {0} {1}", resp.Command, strResp);
        }
    }
}
