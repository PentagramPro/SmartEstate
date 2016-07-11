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
        protected Dictionary<string, string> Parameters; 
        public string Name { get; set; }

        protected GlobalContext glContext;

        public void Init(Dictionary<string, string> parameters, GlobalContext glContext)
        {
            Parameters = parameters;
            this.glContext = glContext;
            InitInternal();
        }

        protected abstract void InitInternal();


        public abstract void Execute(DataRecorder recorder);

        protected void CheckParam(string name)
        {
            if(!Parameters.ContainsKey(name))
                throw new ArgumentException($"Модуль {Name} не обнаружил параметр {name}");
        }
    }
}
