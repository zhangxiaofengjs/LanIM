using Com.LanIM.Common.Logger;
using Com.LanIM.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeamIM;

namespace Com.LanIM.UI
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
            LanIMStore.Initialize();

            FormLogin loginForm = new FormLogin();
            loginForm.ShowDialog();

            FormLanIM form = new FormLanIM();
            Application.Run(form);

            LanIMStore.UnInitialize();
            LoggerFactory.UnInitialize();
        }
    }
}
