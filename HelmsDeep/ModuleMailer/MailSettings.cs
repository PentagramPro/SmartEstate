using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModuleMailer
{
	public class MailSettings
	{
		public NetworkCredential Credential;
		public string From;
		public IEnumerable<string> Targets;
	}
}
