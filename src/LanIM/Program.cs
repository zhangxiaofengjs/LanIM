using Com.LanIM.Common;
using Com.LanIM.Common.Logger;
using Com.LanIM.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM
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
#if !DEBUG
            try
            {
#endif
                LanConfig.Instance.Load();
                LanIMStore.Initialize();

                FormLogin loginForm = new FormLogin();
                DialogResult dr = loginForm.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    FormLanIM form = new FormLanIM();
                    Application.Run(form);

                    LanConfig.Instance.Save();
                }

                LanIMStore.UnInitialize();
#if !DEBUG
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + e.Source);
            }
#endif
            LoggerFactory.UnInitialize();
        }
    }
}
