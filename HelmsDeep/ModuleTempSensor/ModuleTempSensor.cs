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

        public override void Execute(DataRecorder recorder)
        {
            log.Info("Измеряем температуру");
            ArduinoCommand cmd = new ArduinoCommand();
            cmd.Command = 'R';
            ArduinoCommand resp;
            try
            {
                transport.SendReceive(cmd, out resp);
                float temp=0, hum=0;
                float.TryParse(resp.Parameters[0], out temp);
                float.TryParse(resp.Parameters[0], out hum);
                recorder.Record(Name,new []{temp,hum});
                log.Info("Результат: " + resp.ToString());
            }
            catch (TimeoutException ex)
            {
                log.Error("Время ожидания ответа от сенсора истекло");
            }
            catch (Exception ex)
            {
                
                log.Error("Неизвестная ошибка при обмене данными с сенсором");
                log.Error(ex.ToString());
            }
            
        }
    }
}
