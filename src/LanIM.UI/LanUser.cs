using Com.LanIM.Common;
using Com.LanIM.Common.Security;
using Com.LanIM.Network;
using Com.LanIM.Network.Packets;
using Com.LanIM.Network.PacketsEncoder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します

namespace Com.LanIM.UI
{
    class LanUser
    {
        private UdpClientEx _client;
        private FileTransportTcpListener _fileTransTcpListener;
        
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
        private SecurityKeys _securityKeys = new SecurityKeys();
        public SecurityKeys SecurityKeys
        {
            get
            {
                return _securityKeys;
            }
            set
            {
                _securityKeys = value;
            }
        }
        public IPv4Address IP { get; set; }
        public string NickName { get; set; }
        public Image ProfilePhoto { get; set; }
        public int Port { get; set; }
        public LanUserState State { get; set; }
        public string ID { get { return IP.MAC; } }

        private SynchronizationContext _context = null;

        //用户上下线等状态变化
        public event UserStateEventHandler UserEntry;
        public event UserStateEventHandler UserExit;
        public event UserStateEventHandler UserStateChange;

        //文字消息接收
        public event TextMessageReceivedHandler TextMessageReceived;

        //文件接收
        public event FileTransportRequestedHandler FileTransportRequested;
        public event FileTransportEventHandler FileReceiveProgressChanged;
        public event FileTransportEventHandler FileReceiveCompleted;
        public event FileTransportErrorEventHandler FileReceiveError;

        //图像接收
        public event FileTransportEventHandler ImageReceiveProgressChanged;
        public event FileTransportErrorEventHandler ImageReceiveError;
        public event ImageReceivedHandler ImageReceived;

        //发送
        public event SendEventHandler Send;

        //文件发送
        public event FileTransportEventHandler FileSendProgressChanged;
        public event FileTransportEventHandler FileSendCompleted;
        public event FileTransportErrorEventHandler FileSendError;

        public LanUser(SynchronizationContext context = null)
        {
            this._context = context;
            this.Port = UdpClientEx.DEFAULT_PORT;
            this.Contacters = new List<LanUser>();
            this.State = LanUserState.Offline;
            this.IP = new IPv4Address();
        }

        public bool Listen()
        {
            this.SecurityKeys = SecurityFactory.GenerateKeys();

            if (_client != null)
            {
                _client.Close();
            }
            _client = new UdpClientEx(_context)
            {
                Port = this.Port,
                SecurityKeys = SecurityKeys
            };
            _client.SendPacket += this.SendPacketEvent;
            _client.ReceivePacket += this.ReceivePacketEvent;

            //监听端口开始收信
            if (!_client.Listen())
            {
                return false;
            }

            //文件或者大图片发送监听启动
            if(_fileTransTcpListener != null)
            {
                _fileTransTcpListener.Close();
            }
            _fileTransTcpListener = new FileTransportTcpListener(SynchronizationContext.Current);
            _fileTransTcpListener.SecurityKeys = this.SecurityKeys;
            _fileTransTcpListener.ProgressChanged += FileSendProgressChanged;
            _fileTransTcpListener.Completed += FileSendCompleted;
            _fileTransTcpListener.Error += FileSendError;
            _fileTransTcpListener.Listen(IP.Address, this.Port);
            return true;
        }

        //发送上线
        public void Login()
        {
            //先发送一个空操作，以防失败
            SendEmptyPacket();

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

        private void SendEmptyPacket()
        {
            UdpPacket packet = new UdpPacket();
            packet.Remote = IP.BroadcastAddress;
            packet.Port = this.Port;
            packet.Command = UdpPacket.CMD_NOTHING;
            packet.MAC = IP.MAC;

            _client.Send(packet);

            Thread.Sleep(200);
        }

        public void Close()
        {
            _client.Close();
        }

        public void Exit()
        {
            SendEmptyPacket();

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
            extend.EncryptKey = user.SecurityKeys.Public;
            extend.Text = text;
            packet.Extend = extend;

            _client.Send(packet);
        }

        public void SendImage(LanUser user, string imagePath)
        {
            SendImageCore(user, null, imagePath);
        }

        public void SendImage(LanUser user, Image image)
        {
            SendImageCore(user, image, null);
        }

        private void SendImageCore(LanUser user, Image image, string imagePath)
        {
            string path;
            if (image != null)
            {
                path = LanConfig.Instance.GetTempFileName(".png");
                image.Save(path, ImageFormat.Png);
            }
            else
            {
                path = imagePath;
            }

            long len = LanFile.GetFileLength(path);
            if (len > UdpClientEx.UDP_MAX_BUF_SIZE)
            {
                //图像文件过大的话用文件形式发送
                SendFile(user, path, true);
            }
            else
            {
                UdpPacket packet = new UdpPacket();
                packet.Remote = user.IP.Address;
                packet.Port = user.Port;
                packet.Command = UdpPacket.CMD_SEND_IMAGE | UdpPacket.CMD_OPTION_NEED_RESPONSE;
                packet.MAC = this.IP.MAC;

                UdpPacketImageExtend extend = new UdpPacketImageExtend();
                extend.EncryptKey = user.SecurityKeys.Public;
                extend.Image = image ?? Image.FromFile(path);
                packet.Extend = extend;

                _client.Send(packet);

                if (image != null)
                {
                    //直接发送图像的话，临时图片就删除
                    LanFile.Delete(path);
                }
            }
        }

        public void SendFile(LanUser user, string path, bool bImage = false)
        {
            //发送请求
            UdpPacket packet = new UdpPacket();
            packet.Remote = user.IP.Address;
            packet.Port = user.Port;
            packet.Command = UdpPacket.CMD_SEND_FILE_REQUEST | UdpPacket.CMD_OPTION_NEED_RESPONSE;
            if(bImage)
            {
                packet.Command |= UdpPacket.CMD_OPTION_SEND_FILE_IMAGE;
            }
            packet.MAC = this.IP.MAC;

            UdpPacketSendFileRequestExtend extend = new UdpPacketSendFileRequestExtend();
            extend.EncryptKey = user.SecurityKeys.Public;
            extend.File = new LanFile(path);

            packet.Extend = extend;

            _client.Send(packet);

            //保存要发送文件一览
            TransportFile file = new TransportFile(packet.ID, user.IP.Address, user.Port, user.SecurityKeys.Public, extend.File);
            _fileTransTcpListener.AddTransportFile(file);
        }

        public void ReceiveFile(TransportFile file)
        {
            FileTransportTcpClient client = new FileTransportTcpClient(SynchronizationContext.Current);
            client.ProgressChanged += FileReceiveProgressChanged;
            client.Completed += FileReceiveCompleted;
            client.Error += FileReceiveError;
            client.Receive(file);
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
                #region 需要回应收到包
                if (packet.Version == Packet.VERSION)
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
                #endregion
            }
            
            if (packet.CMD == UdpPacket.CMD_NOTHING)
            {
                //do nothing
            }
            else if (packet.CMD == UdpPacket.CMD_ENTRY)
            {
                #region CMD_ENTRY
                UpdateContacter(packet);

                //发送自己的信息，让发送方也更新
                UdpPacket packetRsp = new UdpPacket();
                packetRsp.Remote = packet.Remote;
                packetRsp.Port = packet.Port;
                packetRsp.MAC = IP.MAC;
                packetRsp.Command = UdpPacket.CMD_STATE;

                UdpPacketEntryExtend entryExtend = new UdpPacketEntryExtend
                {
                    PublicKey = _client.SecurityKeys.Public,
                    NickName = this.NickName
                };
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
            else if (packet.CMD == UdpPacket.CMD_SEND_FILE_REQUEST)
            {
                #region CMD_SEND_FILE_REQUEST
                LanUser user = this[packet.MAC];
                UdpPacketSendFileRequestExtend extend = packet.Extend as UdpPacketSendFileRequestExtend;

                TransportFile file = new TransportFile(packet.ID, user.IP.Address, user.Port, user.SecurityKeys.Public, extend.File);

                if ((packet.Command & UdpPacket.CMD_OPTION_SEND_FILE_IMAGE) != 0)
                {
                    //接收图像
                    if (ImageReceived != null)
                    {
                        this.ReceiveImage(user, file);
                    }
                }
                else
                {
                    if (FileTransportRequested != null)
                    {
                        FileTransportRequestedEventArgs stateArgs = new FileTransportRequestedEventArgs(user, file);
                        FileTransportRequested(this, stateArgs);
                    }
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
                throw new Exception("未想定命令");
            }
        }

        private void ReceiveImage(LanUser user, TransportFile file)
        {
            file.SavePath = LanConfig.Instance.GetTempFileName(".png");

            FileTransportTcpClient client = new FileTransportTcpClient(SynchronizationContext.Current);
            client.ProgressChanged += ImageReceiveProgressChanged;
            client.Completed += new FileTransportEventHandler((sender, args)=>
            {
                Image image = Image.FromFile(file.SavePath);
                ImageReceivedEventArgs stateArgs = new ImageReceivedEventArgs(user, image);
                ImageReceived(this, stateArgs);
            });
            client.Error += ImageReceiveError;
            client.Receive(file);
        }

        private void UpdateContacter(UdpPacket packet)
        {
            UdpPacketEntryExtend extend = packet.Extend as UdpPacketEntryExtend;
           LanUser user = this[packet.MAC];
            user.SecurityKeys.Public = extend.PublicKey;
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
