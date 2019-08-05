using Com.LanIM.Common.Network;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LanIM.Server
{
    public partial class FormServer : CommonForm
    {
        UdpClient _client;
        public FormServer()
        {
            InitializeComponent();

            List<NCIInfo> list = NCIInfo.GetNICInfo(NCIType.Physical | NCIType.Wireless);
            textBox1.Text = list[0].Address.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(textBox1.Text), 2425);
            _client = new UdpClient(ipe);

            _client.BeginReceive(AsyncReceiveHandler, null);
        }

        delegate void xxx(string str);
        private void AsyncReceiveHandler(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                byte[] buff = null;
                IPEndPoint remoteEp = null;
                buff = _client.EndReceive(ar, ref remoteEp);

                richTextBox1.Invoke(new xxx(
                    (str) =>
                    {
                        richTextBox1.AppendText(str);
                    }
                    ),
                    "r:" + remoteEp.Address.ToString() + ":" + remoteEp.Port + Encoding.ASCII.GetString(buff) + "\r\n");
                _client.BeginReceive(AsyncReceiveHandler, null);
            }
        }
    }
}
