using Com.LanIM.Common;
using Com.LanIM.Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.Server
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoggerFactory.Initialize();

            LanServerConfig.Instance.Load();

            Application.Run(new FormServer());

            LanServerConfig.Instance.Save();

            LoggerFactory.UnInitialize();
        }
    }
}
