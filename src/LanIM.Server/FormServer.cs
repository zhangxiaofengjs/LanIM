using Com.LanIM.Common;
using Com.LanIM.Common.Network;
using Com.LanIM.Network;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.Server
{
    public partial class FormServer : Form
    {
        private RetransServer _server;

        public FormServer()
        {
            InitializeComponent();

            List<NCIInfo> list = NCIInfo.GetNICInfo(NCIType.Physical | NCIType.Wireless);
            textBox1.Text = list[0].Address.ToString();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            _server = new RetransServer(SynchronizationContext.Current);
            _server.IP = IPAddress.Parse(textBox1.Text);
            _server.Port = int.Parse(textBox2.Text);
            _server.MAC = LanServerConfig.Instance.MAC;
            _server.Start();
        }
    }
}
