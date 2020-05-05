using Com.LanIM.Common.Network;
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

namespace LanIMTest
{
    public partial class FormNatUdp : Form
    {
        public FormNatUdp()
        {
            InitializeComponent();

            List<NCIInfo> list = NCIInfo.GetNICInfo(NCIType.Physical | NCIType.Wireless);
            textBox1.Text = list[0].Address.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(textBox1.Text), 2425);
            UdpClient client = new UdpClient(ipe);

            IPEndPoint ipe2 = new IPEndPoint(IPAddress.Parse(textBox2.Text), 2425);
            byte[] buff = Encoding.ASCII.GetBytes("hello");
            client.Send(buff, buff.Length, ipe2);
        }
    }
}
