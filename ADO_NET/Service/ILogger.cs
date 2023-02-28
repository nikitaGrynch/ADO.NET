using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_NET.Service
{
    internal interface ILogger
    {
        void Log(string message, String level);
        void Log(string message, String level, String className, String methodName);
    }
}
