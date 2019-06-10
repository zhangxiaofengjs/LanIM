﻿using Com.LanIM.Common.Security;
using Com.LanIM.Network;
using Com.LanIM.Network.Packet;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamIM
{
    public partial class FormIPMsgTest : Form
    {
        LanUser _user = null;

        public FormIPMsgTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ulong cmd = IPMsgUdpPacket.IPMSG_CMD_BR_ENTRY | IPMsgUdpPacket.IPMSG_CMD_OPT_FILEATTACH | IPMsgUdpPacket.IPMSG_CLIPBOARDOPT |
            //    IPMsgUdpPacket.IPMSG_ENCRYPTOPT | IPMsgUdpPacket.IPMSG_CAPUTF8OPT | IPMsgUdpPacket.IPMSG_ENCEXTMSGOPT;

            //IPMsgUdpPacket packet = new IPMsgUdpPacket("zhangxiaofeng", "ZHANGXIAOF-PC", cmd, "zhangxiaofeng123");
            //packet.Remote = IPAddress.Parse("192.168.7.255");
            //packet.Port = 2425;

            //_client.Send(packet);
        }

        Color _color = Color.Black;
        private void OutputLog(string str)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;
            _color = (_color == Color.Black ? Color.Blue : Color.Black);
            richTextBox1.SelectionColor = _color;
            richTextBox1.AppendText(DateTime.Now.ToString("[yyyyMMdd HH:mm:ss]") + str + "\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<IPv4Address> list = NetworkCardInterface.GetIPv4Address();
            foreach (IPv4Address ip in list)
            {
                richTextBox1.AppendText(ip.ToString() + "\r\n");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //List<IPv4Address> ips = NetworkCardInterface.GetIPv4Address();
            //foreach (IPv4Address ip in ips)
            //{
            //    if (ip.NetworkCardInterfaceType == NetworkCardInterfaceType.Physical ||
            //        ip.NetworkCardInterfaceType == NetworkCardInterfaceType.Wireless)
            //    {
            //        ulong cmd = IPMsgUdpPacket.IPMSG_CMD_BR_EXIT | IPMsgUdpPacket.IPMSG_CMD_OPT_FILEATTACH | IPMsgUdpPacket.IPMSG_CLIPBOARDOPT |
            //        IPMsgUdpPacket.IPMSG_ENCRYPTOPT | IPMsgUdpPacket.IPMSG_CAPUTF8OPT | IPMsgUdpPacket.IPMSG_ENCEXTMSGOPT;

            //        IPMsgUdpPacket packet = new IPMsgUdpPacket("zhangxiaofeng", "ZHANGXIAOF-PC", cmd, "zhangxiaofeng123");
            //        packet.Remote = ip.BroadcastAddress;
            //        packet.Port = 2425;

            //        _client.Send(packet);
            //    }
            //}
        }

        private void button6_Click(object sender, EventArgs e)
        {
        }


        private void buttonStart_Click(object sender, EventArgs e)
        {
            _user = new LanUser();
            _user.NickName = textBoxNN.Text;
            _user.UserEntry += _user_UserEntry;
            _user.UserExit += _user_UserExit;
            _user.UserStateChange += _user_UserStateChange;
            _user.Send += _user_Send;
            _user.TextMessageReceived += _user_TextMessageReceived;
            _user.ImageReceived += _user_ImageReceived;

            buttonStart.Enabled = false;
            buttonClose.Enabled = true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            _user.Close();
            buttonStart.Enabled = true;
            buttonClose.Enabled = false;
        }

        private void buttonEntry_Click(object sender, EventArgs e)
        {
            _user.NickName = textBoxNN.Text;
            if (_user.Login())
            {
                OutputLog("登录成功:" + _user.ToString());
            }
            else
            {
                OutputLog("登录失败:" + _user.ToString());
            }
        }

        private void _user_TextMessageReceived(object sender, TextMessageReceivedEventArgs args)
        {
                OutputLog("接受文本消息: user=" + args.User + ", msg=" + args.Message);
        }

        private void _user_Send(object sender, SendEventArgs args)
        {
                OutputLog("发送: success=" + args.Success + ", packet=" + args.Packet);
        }

        private void _user_UserStateChange(object sender, UserStateChangeEventArgs args)
        {
            comboBoxUsers.Items.Clear();
            comboBoxUsers.Items.AddRange(_user.Contacters.ToArray());
            if (comboBoxUsers.Items.Count > 0)
                comboBoxUsers.SelectedIndex = 0;
            OutputLog("状态更新:" + args.User.ToString());
        }

        private void _user_UserExit(object sender, UserStateChangeEventArgs args)
        {
            comboBoxUsers.Items.Clear();
            comboBoxUsers.Items.AddRange(_user.Contacters.ToArray());
            if(comboBoxUsers.Items.Count>0)
                comboBoxUsers.SelectedIndex = 0;
            OutputLog("下线:" + args.User.ToString());
        }

        private void _user_UserEntry(object sender, UserStateChangeEventArgs args)
        {
            comboBoxUsers.Items.Clear();
            comboBoxUsers.Items.AddRange(_user.Contacters.ToArray());
            if (comboBoxUsers.Items.Count > 0)
                comboBoxUsers.SelectedIndex = 0;
            OutputLog("上线:" + args.User.ToString());
        }

        private void _user_ImageReceived(object sender, ImageReceivedEventArgs args)
        {
            OutputLog("接受图片: user=" + args.User + ", image=");
            Clipboard.SetImage(args.Image);
            richTextBox1.Paste();
            richTextBox1.AppendText("\r\n");
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            _user.Exit();
            OutputLog("退出:" + _user.ToString());
        }

        private void buttonSendMsg_Click(object sender, EventArgs e)
        {
            _user.SendTextMessage(comboBoxUsers.SelectedItem as LanUser, textBox2.Text);
        }

        private void buttonSendPic_Click(object sender, EventArgs e)
        {
            _user.SendImage(comboBoxUsers.SelectedItem as LanUser, radioButton1.Checked? pictureBox1.Image:pictureBox2.Image);
        }

        private void buttonSendFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog(this) == DialogResult.OK)
            {
                _user.SendFile(comboBoxUsers.SelectedItem as LanUser, ofd.FileName);
            }
        }
    }
}