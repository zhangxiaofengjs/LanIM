using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Logger
{
    public interface ILogger
    {
        bool Initialize();

        bool Uninitialize();

        void Debug(string strLog, params object[] args);
        void Error(string strLog, params object[] args);
    }
}
