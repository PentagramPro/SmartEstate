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

        public override void Init()
        {
            log.Info("Модуль датчика температуры");
        }

        public override void Execute()
        {
            log.Info("Вызван модуль датчика температуры");
        }
    }
}
