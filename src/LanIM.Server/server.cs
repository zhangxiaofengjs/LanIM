using com.tim.nw;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamIMServer
{
	public partial class server : Form
	{
        TIMUdpClient _client = new TIMUdpClient();
        TIMUdpClient _server;

        public server()
		{
			InitializeComponent();

            _client.Context = SynchronizationContext.Current;
            _client.PackageSend += _client_PackageSend;
        }

        private void _client_PackageSend(object sender, TIMUdpClientSendEventArgs args)
        {
            if(args.IsSuccess)
            {
                richTextBox1.AppendText("send:" + args.Package.ID + "\r\n");
            }
            else
            {
                richTextBox1.AppendText("send error:" + args.Package.ID + "\r\n");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _client.PackageReceived += _server_Received;
            _client.Listen(6689);
            richTextBox1.AppendText("begin rev\r\n");
        }

        private void _server_Received(object sender, TIMUdpClientReceiveEventArgs arg)
        {
            String str = Encoding.ASCII.GetString(arg.Package.Data);
            richTextBox1.AppendText(arg.Package.ID + ":" + str + "\r\n");
        }

        TcpListener myServer;
		NetworkStream stream;
		IPEndPoint ipend;
		private void button1_Click(object sender, EventArgs e)
		{
			IPAddress myIP = IPAddress.Parse("10.16.0.76");
			myServer = new TcpListener(IPAddress.Any, 6688);
			//myServer.AllowNatTraversal(true);
			myServer.Start();
			richTextBox1.AppendText("start..." + myServer.LocalEndpoint.AddressFamily.ToString());

			myServer.BeginAcceptTcpClient(
				new AsyncCallback(HandleTcpClientAccepted), myServer);
		}

		private void HandleTcpClientAccepted(IAsyncResult ar)
		{
			TcpClient client = myServer.EndAcceptTcpClient(ar);

			ipend = (IPEndPoint)client.Client.RemoteEndPoint;
			richTextBox1.AppendText("connect..." +
				(ipend).ToString());
			stream = client.GetStream();

			byte[] buff = new byte[client.ReceiveBufferSize];
			stream.BeginRead(buff, 0, buff.Length, HandleDataReceived, buff);
		}

		private void HandleDataSend(IAsyncResult ar)
		{
			stream.EndWrite(ar);
		}

		private void HandleDataReceived(IAsyncResult ar)
		{
			stream.EndRead(ar);
			byte[] buff = (byte[])ar.AsyncState;

			String str = Encoding.ASCII.GetString(buff);
			richTextBox1.Invoke(new Action(delegate ()
			{
				richTextBox1.AppendText(str);
			}));

			byte[] b = new byte[8192];
			stream.BeginRead(b, 0, b.Length, HandleDataReceived, b);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			byte[] buff = Encoding.ASCII.GetBytes("hello, " + ipend.ToString());
			stream.BeginWrite(buff, 0, buff.Length, HandleDataSend, buff);
		}


        private void button3_Click(object sender, EventArgs e)
		{
			IPEndPoint remote = new IPEndPoint(IPAddress.Parse(textBoxUdp.Text), 6689);

            for (int i = 0; i < 1; i++)
            {
                //byte[] buff = Encoding.ASCII.GetBytes(textBoxUdpMsg.Text + i);
                byte[] buff = new byte[65482];
                for (int j = 0; j < buff.Length; j++)
                {
                    buff[j] = 48;
                }
                TIMUdpPacket package = TIMUdpPacket.Create(buff);
                package.Remote = remote;
                _client.Send(package);
                Thread.Sleep(30);
            }
        }
    }
}
