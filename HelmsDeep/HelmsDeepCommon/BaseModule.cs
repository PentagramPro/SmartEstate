using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace HelmsDeepCommon
{
    public abstract class BaseModule : IModule
    {
        public string Name { get; set; }

        public abstract void Init(Dictionary<string, string> parameters);


        public abstract void Execute();
    }
}
