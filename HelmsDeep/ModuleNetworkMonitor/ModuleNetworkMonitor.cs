using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using HelmsDeepCommon;
using NLog;

namespace ModuleNetworkMonitor
{
    public class ModuleNetworkMonitor : BaseModule
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        protected override void InitInternal()
        {
            
        }

        public override void Execute(DataRecorder recorder)
        {
            int tries = 10;
            
            log.Info("Проверяем интернет");

            for (int i = 0; i < tries; i++)
            {
                if (CheckInternet()) 
                {
                    log.Info("  интернет обнаружен, прерываемся");
                    return;
                }
            }

            log.Info("  интернтета нет, перезагружаем контроллер");
            try
            {
                System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
            }
            catch (Exception e)
            {
                log.Error("Не удалось перезагрузить контроллер");
                log.Error(e);
            }
        }

        bool CheckInternet()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "8.8.8.8";
                byte[] buffer = new byte[32];
                int timeout = 500;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
