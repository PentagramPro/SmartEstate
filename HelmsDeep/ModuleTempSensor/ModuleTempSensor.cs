using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelmsDeepCommon;
using NLog;

namespace ModuleTempSensor
{
    public class ModuleTempSensor : BaseModule
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private ArduinoTransport transport;


        protected override void InitInternal()
        {
            log.Info("Модуль датчика температуры");
            CheckParam("port");
            CheckParam("baudrate");
            int baudrate = int.Parse(Parameters["baudrate"]);
            transport = new ArduinoTransport(Parameters["port"],baudrate);
            transport.Open();
        }

        public override void Execute()
        {
            log.Info("Измеряем температуру");
            ArduinoCommand cmd = new ArduinoCommand();
            cmd.Command = 'R';
            ArduinoCommand resp;
            try
            {
                transport.SendReceive(cmd, out resp);
                log.Info("Результат: " + resp.ToString());
            }
            catch (TimeoutException ex)
            {
                log.Error("Время ожидания ответа от сенсора истекло");
            }
            
        }
    }
}
