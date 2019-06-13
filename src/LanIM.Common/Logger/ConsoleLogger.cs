using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Logger
{
    class ConsoleLogger : ILogger
    {
        public bool Initialize()
        {
            return true;
        }

        public bool Uninitialize()
        {
            return true;
        }

        public void Debug(string strLog, params object[] args)
        {
            string log = LoggerFormater.Format(LoggerType.Debug, strLog, args);
            Console.WriteLine(log);
        }

        public void Error(string strLog, params object[] args)
        {
            string log = LoggerFormater.Format(LoggerType.Error, strLog, args);
            Console.WriteLine(log);
        }

        public void Flush()
        {
        }
    }
}
