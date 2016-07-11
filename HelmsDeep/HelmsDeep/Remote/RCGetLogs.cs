using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelmsDeepCommon;
using System.IO;

namespace HelmsDeep.Remote
{
    public class RCGetLogs : BaseRemoteCommand
    {
        public override void Execute(GlobalContext glContext, ControllerResponse response)
        {
            log.Info("Запрошена отправка логов");
            if(response==null)
            {
                log.Error("Передан неверный параметр в функцию Execute");
                return;
            }

            var files = Directory.EnumerateFiles(glContext.LogsDirFull);
            response.Attachments.AddRange(files);

            response.ResposneBody = $"К письму приложено {response.Attachments.Count} файла(ов) с логами\n";

            log.Info("  логи подготовлены к отправке");
        }
    }
}
