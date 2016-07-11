using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelmsDeep.Remote
{
    public abstract class BaseRemoteCommand
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public abstract void Execute();
    }
}
