using HelmsDeep.Model;
using HelmsDeep.Remote;
using HelmsDeepCommon;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelmsDeep
{
    public class RemoteCommandProcessor : IController
    {
        GlobalContext glContext;
        Dictionary<string, BaseRemoteCommand> commands;

        private static Logger log = LogManager.GetCurrentClassLogger();
        public RemoteCommandProcessor(GlobalContext glContext)
        {
            this.glContext = glContext;
            commands["логи"] = new RCGetLogs();
        }
        public void ExecuteRemoteCommand(string command, object param)
        {
            command = command.Trim().ToLowerInvariant();
            if(!commands.ContainsKey(command))
            {
                log.Error($"команда {command} не найдена");
                return;
            }

            commands[command].Execute(glContext,param);
        }
    }
}
