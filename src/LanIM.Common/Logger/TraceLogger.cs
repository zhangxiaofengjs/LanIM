using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Logger
{
    class TraceLogger : ILogger
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
            string log = Format(LoggerType.Debug, strLog, args);
            Trace.WriteLine(log);
        }

        private static string Format(LoggerType type, string strLog, object[] args)
        {
             return DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + string.Format(strLog, args);
        }

        public void Error(string strLog, params object[] args)
        {
            string log = Format(LoggerType.Error, strLog, args);
            Trace.WriteLine(log);
        }

        public void Flush()
        {
            Trace.Flush();
        }
    }
}
