using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Logger
{
    class SimpleTextLogger : ILogger
    {
        private StreamWriter _logWriter;
        public bool Initialize()
        {
            try
            {
                string path = Path.Combine(Environment.CurrentDirectory, "lanim.log");
                _logWriter = new StreamWriter(path, true);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }
            return true;
        }

        public bool Uninitialize()
        {
            try
            {
                _logWriter.Flush();
                _logWriter.Close();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }
            return true;
        }

        public void Debug(string strLog, params object[] args)
        {
            string log = Format(LoggerType.Debug, strLog, args);
            _logWriter.WriteLine(log);
        }

        private static string Format(LoggerType type, string strLog, object[] args)
        {
             return DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + string.Format(strLog, args);
        }

        public void Error(string strLog, params object[] args)
        {
            string log = Format(LoggerType.Error, strLog, args);
            _logWriter.WriteLine(log);
        }
    }
}
