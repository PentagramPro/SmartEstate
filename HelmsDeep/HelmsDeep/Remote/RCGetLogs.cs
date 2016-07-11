using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelmsDeepCommon;

namespace HelmsDeep.Remote
{
    public class RCGetLogs : BaseRemoteCommand
    {
        public override void Execute(GlobalContext glContext, object param)
        {
            log.Info("Запрошена отправка логов");
            if(! (param is string) )
            {
                log.Error("Передан неверный параметр в функцию Execute");
                return;
            }
        }
    }
}
