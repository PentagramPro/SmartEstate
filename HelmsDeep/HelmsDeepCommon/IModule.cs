﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelmsDeepCommon
{
    public interface IModule
    {
        string Name { get; set; }

        void Init(Dictionary<string, string> parameters, GlobalContext glContext);


        void Execute(DataRecorder recorder);
    }
}
