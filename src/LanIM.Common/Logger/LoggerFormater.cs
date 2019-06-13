using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Logger
{
    public class LoggerFormater
    {
        public static string Format(LoggerType type, string strFmt, object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[{0:yyyy/MM/dd HH:mm:ss}]", DateTime.Now);
            if ((type & LoggerType.Debug) != 0)
            {
                sb.Append("[debug]");
            }
            if ((type & LoggerType.Error) != 0)
            {
                sb.Append("[error]");
            }

            if (args != null && args.Length > 0)
            {
                object[] tmpArgs = new object[args.Length];

                for (int i = 0; i < args.Length; i++)
                {
                    object arg = args[i];
                    if (arg is byte[])
                    {
                        tmpArgs[i] = Convert.ToBase64String(arg as byte[]);
                    }
                    else
                    {
                        tmpArgs[i] = arg;
                    }
                }

                sb.AppendFormat(strFmt, tmpArgs);
            }
            else
            {
                sb.Append(strFmt);
            }

            return sb.ToString();
        }
    }
}
