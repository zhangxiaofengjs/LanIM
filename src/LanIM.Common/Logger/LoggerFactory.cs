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
        private static SimpleTextLogger _simpleTextLoggerInstance;

        public static void Initialize()
        {
            if (_simpleTextLoggerInstance == null)
            {
                _simpleTextLoggerInstance = new SimpleTextLogger();
                _simpleTextLoggerInstance.Initialize();
            }
        }

        public static ILogger Instance()
        {
            return _simpleTextLoggerInstance;
        }
    }
}
