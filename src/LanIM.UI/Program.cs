using Com.LanIM.Common.Logger;
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
            Application.Run(new FormIPMsgTest());
		}
	}
}
