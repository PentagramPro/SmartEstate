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
        public override void Init(Dictionary<string,string> parameters)
        {
            log.Info("Модуль датчика температуры");
            
        }

        public override void Execute()
        {
            log.Info("Измеряем температуру");
            ArduinoCommand cmd = new ArduinoCommand();
            cmd.Command = 'R';

        }
    }
}
