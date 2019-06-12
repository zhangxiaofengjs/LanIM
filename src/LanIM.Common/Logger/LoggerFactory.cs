using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Logger
{
    public class LoggerFactory
    {
        private static List<ILogger> _loggers;
        public static void Initialize()
        {
            _loggers = new List<ILogger>();
            _loggers.Add(new SimpleTextLogger());
            _loggers.Add(new TraceLogger());

            foreach (ILogger l in _loggers)
            {
                l.Initialize();
            }
        }

        public static void UnInitialize()
        {
            foreach (ILogger l in _loggers)
            {
                l.Uninitialize();
            }
        }

        public static void Debug(string strLog, params object[] args)
        {
            foreach (ILogger l in _loggers)
            {
                l.Debug(strLog, args);
            }
        }

        public static void Error(string strLog, params object[] args)
        {
            foreach (ILogger l in _loggers)
            {
                l.Error(strLog, args);
            }
        }

        public static void Flush()
        {
            foreach (ILogger l in _loggers)
            {
                l.Flush();
            }
        }
    }
}
