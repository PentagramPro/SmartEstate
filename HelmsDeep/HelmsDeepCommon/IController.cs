using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelmsDeepCommon
{
    public interface IController
    {
        void ExecuteRemoteCommand(string command);
    }
}
