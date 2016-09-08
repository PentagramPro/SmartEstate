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
		  Dictionary<string, BaseRemoteCommand> commands = new Dictionary<string, BaseRemoteCommand>();

		  private static Logger log = LogManager.GetCurrentClassLogger();
		  public RemoteCommandProcessor(GlobalContext glContext)
		  {
				this.glContext = glContext;
				commands["логи"] = new RCGetLogs();
				commands["отчет"] = new RCGetReport();
		  }
		  public void ExecuteRemoteCommand(string command, ControllerResponse response)
		  {
				command = command.Trim().ToLowerInvariant();
				if (!commands.ContainsKey(command))
				{
					 log.Error($"команда {command} не найдена");

					 StringBuilder sb = new StringBuilder($"команда {command} не найдена, список:");
					 foreach (var k in commands.Keys)
					 {
						  sb.Append("  ");
						  sb.Append(k);
						  sb.Append(" - ");
						  sb.AppendLine(commands[k].Description);
					 }
					 response.ResposneBody=(sb.ToString());
					 return;
				}


				commands[command].Execute(glContext, response);
		  }
	 }
}
