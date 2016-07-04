using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelmsDeepCommon.Exceptions
{
    public class ModuleExecutionException : Exception
    {
        public ModuleExecutionException(string message) : base(message)
        {
        }
    }
}
