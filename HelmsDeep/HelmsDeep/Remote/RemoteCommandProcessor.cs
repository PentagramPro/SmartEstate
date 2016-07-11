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
        LocalContext context;
        Dictionary<string, BaseRemoteCommand> commands;

        private static Logger log = LogManager.GetCurrentClassLogger();
        public RemoteCommandProcessor(LocalContext context)
        {
            this.context = context;
        }
        public void ExecuteRemoteCommand(string command)
        {
            command = command.Trim().ToLowerInvariant();
            if(!commands.ContainsKey(command))
            {
                log.Error($"команда {command} не найдена");
                return;
            }

            commands[command].Execute();
        }
    }
}
