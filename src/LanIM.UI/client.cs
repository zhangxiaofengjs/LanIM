using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamIM
{
	public partial class client : Form
	{
		public client()
		{
			InitializeComponent();
		}

		NetworkStream stream = null;
		private void button1_Click(object sender, EventArgs e)
		{
			IPAddress myIP = IPAddress.Parse(textBox2.Text
				);
			TcpClient client = new TcpClient();
			client.Connect(myIP, 6688);

			//创建网络流,获取数据流
			stream = client.GetStream();

			byte[] buff = new byte[client.SendBufferSize];
			stream.Read(buff, 0, buff.Length);

			richTextBox1.AppendText(Encoding.ASCII.GetString(buff));
		}

		private void button2_Click(object sender, EventArgs e)
		{
			byte[] buff = Encoding.ASCII.GetBytes(textBox1.Text);
			stream.Write(buff, 0 , buff.Length);
			stream.Flush();
		}
	}
}
