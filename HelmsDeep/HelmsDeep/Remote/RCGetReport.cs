using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelmsDeepCommon;

namespace HelmsDeep.Remote
{
	public class RCGetReport : BaseRemoteCommand
	{
		public override string Description
		{
			get
			{
				return "присылает внеочередной отчет (без сброса)";
			}
		}


		public override void Execute(GlobalContext glContext, ControllerResponse response)
		{

		}
	}
}
