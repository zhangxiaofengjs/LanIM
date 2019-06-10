using Com.LanIM.Common.Security;
using Com.LanIM.Network;
using Com.LanIM.Network.Packet;
using LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    class LanUser
    {
        private UdpClientEx _client;

        //联系人一览
        public List<LanUser> Contacters { get; set; }

        LanUser this[string mac]
        {
            get
            {
                LanUser user = Contacters.Find((u) =>
                {
                    if (u.IP.MAC == mac)
                    {
                        return true;
                    }
                    return false;
                });

                if(user == null)
                {
                    user = new LanUser();
                    IPv4Address ip = new IPv4Address();
                    user.IP = ip;
                    this.Contacters.Add(user);
                }
                return user;
            }
        }
        public byte[] PublicKey { get; set; }
        public IPv4Address IP { get; set; }
        public string NickName { get; set; }
        public int Port { get; set; }
        public LanUserState State { get; set; }
        
        public event SendEventHandler Send;
        public event UserStateEventHandler UserEntry;
        public event UserStateEventHandler UserExit;
        public event UserStateEventHandler UserStateChange;
        public event TextMessageReceivedHandler TextMessageReceived;
        public event ImageReceivedHandler ImageReceived;

        public LanUser()
        {
            this.Port = UdpClientEx.DEFAULT_PORT;
            this.Contacters = new List<LanUser>();
            this.IP = GetIP();
        }

        private IPv4Address GetIP()
        {
            List<IPv4Address> ips = NetworkCardInterface.GetIPv4Address();
            foreach (IPv4Address ip in ips)
            {
                if (ip.NetworkCardInterfaceType == NetworkCardInterfaceType.Physical ||
                    ip.NetworkCardInterfaceType == NetworkCardInterfaceType.Wireless)
                {
                    return ip;
                }
            }
            return null;
        }

        public bool Login()
        {
            if (_client != null)
            {
                _client.Close();
            }
            _client = new UdpClientEx(SynchronizationContext.Current);
            _client.Port = this.Port;
            _client.SecurityKeys = SecurityFactory.GenerateKeys();
            _client.SendPacket += this.SendPacketEvent;
            _client.ReceivePacket += this.ReceivePacketEvent;

            //监听端口开始收信
            if (!_client.Listen())
            {
                return false;
            }

            //发送上线
            SendEntryPacket();

            return true;
        }

        private void SendEntryPacket()
        {
            UdpPacket packet = new UdpPacket();
            packet.Remote = IP.BroadcastAddress;
            packet.Port = this.Port;
            packet.Command = UdpPacket.CMD_ENTRY;
            packet.MAC = IP.MAC;

            UdpPacketEntryExtend extend = new UdpPacketEntryExtend();
            extend.PublicKey = _client.SecurityKeys.Public;//送公钥出去
            extend.NickName = this.NickName;

            packet.Extend = extend;

            _client.Send(packet);
        }

        internal void Close()
        {
            _client.Close();
        }

        public void Exit()
        {
            this.State = LanUserState.Offline;

            UdpPacket packet = new UdpPacket();
            packet.Remote = IP.BroadcastAddress;
            packet.Port = this.Port;
            packet.Command = UdpPacket.CMD_EXIT;
            packet.MAC = IP.MAC;

            _client.Send(packet);
        }

        public void SendTextMessage(LanUser user, string text)
        {
            UdpPacket packet = new UdpPacket();
            packet.Remote = user.IP.Address;
            packet.Port = user.Port;
            packet.Command = UdpPacket.CMD_SEND_TEXT | UdpPacket.CMD_OPTION_NEED_RESPONSE;
            packet.MAC = this.IP.MAC;

            UdpPacketTextExtend extend = new UdpPacketTextExtend();
            extend.EncryptKey = user.PublicKey;
            extend.Text = text;
            packet.Extend = extend;

            _client.Send(packet);
        }

        public void SendImage(LanUser user, Image image)
        {
            UdpPacket packet = new UdpPacket();
            packet.Remote = user.IP.Address;
            packet.Port = user.Port;
            packet.Command = UdpPacket.CMD_SEND_IMAGE | UdpPacket.CMD_OPTION_NEED_RESPONSE;
            packet.MAC = this.IP.MAC;

            UdpPacketImageExtend extend = new UdpPacketImageExtend();
            extend.EncryptKey = user.PublicKey;
            extend.Image = image;
            packet.Extend = extend;

            _client.Send(packet);
        }

        public void SendFile(LanUser user, string path)
        {
            //发送请求
            UdpPacket packet = new UdpPacket();
            packet.Remote = user.IP.Address;
            packet.Port = user.Port;
            packet.Command = UdpPacket.CMD_SEND_FILE_REQUEST | UdpPacket.CMD_OPTION_NEED_RESPONSE;
            packet.MAC = this.IP.MAC;

            UdpPacketFileExtend extend = new UdpPacketFileExtend();
            extend.EncryptKey = user.PublicKey;
            extend.FileInfo = new LanFile(path);

            packet.Extend = extend;

            _client.Send(packet);
        }

        private void SendPacketEvent(object sender, UdpClientSendEventArgs args)
        {
            UdpPacket packet = args.Packet as UdpPacket;
            OnSend(packet, args.Success);
        }

        protected virtual void OnSend(UdpPacket packet, bool success)
        {
            if(Send != null)
            {
                SendEventArgs args = new SendEventArgs(packet, success);
                Send(this, args);
            }
        }

        private void ReceivePacketEvent(object sender, UdpClientReceiveEventArgs args)
        {
            UdpPacket packet = args.Packet as UdpPacket;
           
            if (packet.CheckSendResponse)
            {
                //需要回应收到包
                if (packet.Version == UdpPacket.VERSION)
                {
                    UdpPacket packetRsp = new UdpPacket();
                    packetRsp.Remote = packet.Remote;
                    packetRsp.Port = packet.Port;
                    packetRsp.MAC = IP.MAC;
                    packetRsp.Command = UdpPacket.CMD_RESPONSE;

                    UdpPacketResponseExtend extend = new UdpPacketResponseExtend();
                    extend.ID = packet.ID;
                    packetRsp.Extend = extend;

                    _client.Send(packetRsp);
                }
            }

            if (packet.CMD == UdpPacket.CMD_ENTRY)
            {
                #region CMD_ENTRY
                UpdateContacter(packet);

                //发送自己的信息，让发送方也更新
                UdpPacket packetRsp = new UdpPacket();
                packetRsp.Remote = packet.Remote;
                packetRsp.Port = packet.Port;
                packetRsp.MAC = IP.MAC;
                packetRsp.Command = UdpPacket.CMD_STATE;

                UdpPacketEntryExtend entryExtend = new UdpPacketEntryExtend();
                entryExtend.PublicKey = _client.SecurityKeys.Public;
                entryExtend.NickName = this.NickName;
                packetRsp.Extend = entryExtend;

                _client.Send(packetRsp);

                if (UserEntry != null)
                {
                    LanUser user = this[packet.MAC];
                    UserStateChangeEventArgs stateArgs = new UserStateChangeEventArgs(user);
                    UserEntry(this, stateArgs);
                }
                #endregion
            }
            else if(packet.CMD == UdpPacket.CMD_EXIT)
            {
                #region CMD_EXIT
                if (UserExit != null)
                {
                    LanUser user = this[packet.MAC];
                    user.State = LanUserState.Offline;
                    UserStateChangeEventArgs stateArgs = new UserStateChangeEventArgs(user);
                    UserExit(this, stateArgs);
                }
                #endregion
            }
            else if (packet.CMD == UdpPacket.CMD_STATE)
            {
                #region CMD_STATE
                UpdateContacter(packet);
                if (UserStateChange != null)
                {
                    LanUser user = this[packet.MAC];
                    UserStateChangeEventArgs stateArgs = new UserStateChangeEventArgs(user);
                    UserStateChange(this, stateArgs);
                }
                #endregion
            }
            else if (packet.CMD == UdpPacket.CMD_SEND_TEXT)
            {
                #region CMD_SEND_TEXT
                UdpPacketTextExtend extend = packet.Extend as UdpPacketTextExtend;
                if (TextMessageReceived != null)
                {
                    LanUser user = this[packet.MAC];
                    TextMessageReceivedEventArgs stateArgs = new TextMessageReceivedEventArgs(user, extend.Text);
                    TextMessageReceived(this, stateArgs);
                }
                #endregion
            }
            else if (packet.CMD == UdpPacket.CMD_SEND_IMAGE)
            {
                #region CMD_SEND_IMAGE
                UdpPacketImageExtend extend = packet.Extend as UdpPacketImageExtend;
                if (ImageReceived != null)
                {
                    LanUser user = this[packet.MAC];
                    ImageReceivedEventArgs stateArgs = new ImageReceivedEventArgs(user, extend.Image);
                    ImageReceived(this, stateArgs);
                }
                #endregion
            }
            else if (packet.CMD == UdpPacket.CMD_RESPONSE)
            {
                UdpPacketResponseExtend extend = packet.Extend as UdpPacketResponseExtend;
                _client.NotifySendPacketSuccess(extend.ID);
            }
            else
            {
            }
        }

        private void UpdateContacter(UdpPacket packet)
        {
            UdpPacketEntryExtend extend = packet.Extend as UdpPacketEntryExtend;
           LanUser user = this[packet.MAC];
            user.PublicKey = extend.PublicKey;
            user.IP.MAC = packet.MAC;
            user.IP.Address = packet.Remote;
            user.NickName = extend.NickName;
            user.State = extend.HideState ? LanUserState.Offline : LanUserState.Online;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}][{2}][{3}]",
                this.NickName, this.IP.MAC, this.IP.Address,
                this.State == LanUserState.Online ? "在线" : this.State == LanUserState.Leave ? "离开" : "离线");
        }
    }
}
