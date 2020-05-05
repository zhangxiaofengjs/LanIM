using Com.LanIM.Common;
using Com.LanIM.Common.Network;
using Com.LanIM.Common.Security;
using Com.LanIM.Network;
using Com.LanIM.Network.Packets;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Threading;
using Com.LanIM.Store.Models;
using Com.LanIM.Network.PacketEncoder;
using Com.LanIM.Common.Logger;
using Com.LanIM.Network.PacketResolver;
using System.IO;
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します

namespace Com.LanIM
{
    class LanUser : User
    {
        private UdpClientEx _client;
        private FileTransportTcpListener _fileTransTcpListener;

        private long _hbId = 0;
        private bool _serverAvailable = false;
        //联系人一览
        public List<LanUser> Contacters { get; set; }
        public LanUser this[string mac]
        {
            get
            {
                LanUser user = Contacters.Find((u) =>
                {
                    if (u.MAC == mac)
                    {
                        return true;
                    }
                    return false;
                });

                if(user == null)
                {
                    user = new LanUser();
                    user.MAC = mac;
                    this.Contacters.Add(user);
                }
                return user;
            }
        }

        public override string MAC { get { return Contacter.MAC; } set { Contacter.MAC = value; } }
        public override IPAddress Address { get { return NCIInfo.Parse(Contacter.IP); } set { Contacter.IP = value.ToString(); } }
        public override int Port { get { return Contacter.Port; } set { Contacter.Port = value; } }
        public List<IPAddress> BroadcastAddress { get; set; }
        public override string NickName { get { return Contacter.NickName; } set { Contacter.NickName = value; } }
        public string Memo { get { return Contacter.Memo; } set { Contacter.Memo = value; } }
        public override UserStatus Status { get; set; }
        public Contacter Contacter { get; internal set; }
        private bool _trans = true;//同一Lan的用户，无需转发

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
            this.Contacter = new Contacter();
            this.Port = UdpClientEx.DEFAULT_PORT;
            this.Contacters = new List<LanUser>();
            this.Status = UserStatus.Offline;
            this.Address = IPAddress.None;
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
                IP = this.Address,
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
            _fileTransTcpListener = new FileTransportTcpListener(_context);
            _fileTransTcpListener.SecurityKeys = this.SecurityKeys;
            _fileTransTcpListener.ProgressChanged += FileSendProgressChanged;
            _fileTransTcpListener.Completed += FileSendCompleted;
            _fileTransTcpListener.Error += FileSendError;
            _fileTransTcpListener.Listen(Address, this.Port);

            return true;
        }

        //发送上线
        public void Login()
        {
            foreach (IPAddress broadcastAddr in this.BroadcastAddress)
            {
                //先发送一个空操作，以防失败
                SendEmptyPacket(broadcastAddr);
                Thread.Sleep(100);

                UdpPacket packet = new UdpPacket();
                packet.Address = broadcastAddr;
                packet.Port = this.Port;
                packet.ToMAC = string.Empty;
                packet.Command = UdpPacket.CMD_ENTRY;
                packet.FromMAC = this.MAC;

                UdpPacketUserStateExtend extend = new UdpPacketUserStateExtend();
                extend.User = this;
                extend.UpdateState = UpdateState.NickName | UpdateState.PublicKey | 
                    UpdateState.Status | UpdateState.IP | UpdateState.Port;

                packet.Extend = extend;

                //广播上线以及上线信息
                _client.Send(packet);
                Thread.Sleep(50);
            }

            //10分钟以后更新头像
            WaitTimer.Once(1 * 60 * 10 * 1000, 
                new TimerCallback((obj)=>
                {
                    UpdateMyStateByBroadcast(UpdateState.Photo);
                }
            ), null);

            //1分钟一次HB给服务器
            StartHeartBeat();
        }

        private void StartHeartBeat()
        {
            if (LanClientConfig.Instance.ServerAddress != IPAddress.Any)
            {
                this._hbId = WaitTimer.LoopNow(LanClientConfig.Instance.ServerHBInterval,
                    new TimerCallback((obj) =>
                    {
                        UdpPacket packet = new UdpPacket();
                        packet.Address = LanClientConfig.Instance.ServerAddress;
                        packet.Port = LanClientConfig.Instance.ServerPort;
                        packet.FromMAC = this.MAC;
                        packet.Command = UdpPacket.CMD_HEART_BEAT;
                    }
                ), null);
            }
        }

        private void SendPacket(UdpPacket packet, bool trans)
        {
            UdpPacket sendPacket = packet;

            if (trans)
            {
                if (this._serverAvailable)
                {
                    //服务器在线，让他代为转发一下
                    sendPacket = new UdpPacket();
                    sendPacket.Address = LanClientConfig.Instance.ServerAddress;
                    sendPacket.Port = LanClientConfig.Instance.ServerPort;
                    sendPacket.ToMAC = string.Empty;//发送到server的都为空
                    sendPacket.FromMAC = this.MAC;
                    sendPacket.Command = UdpPacket.CMD_RETRANSMIT | UdpPacket.CMD_OPTION_NEED_RESPONSE;

                    IPacketEncoder encoder = PacketEncoderFactory.CreateEncoder(packet);
                    EncodeResult result = encoder.Encode();

                    UdpPacketRetransExtend extend = new UdpPacketRetransExtend(result.Fragments[0]);
                    extend.Address = packet.Address;
                    extend.Port = packet.Port;

                    sendPacket.Extend = extend;

                    LoggerFactory.Debug("prepare retrans to server packet:{0}", sendPacket);
                }
                else
                {
                    //服务器不可用
                }
            }

            _client.Send(sendPacket);
        }

        private void SendEmptyPacket(IPAddress addr)
        {
            UdpPacket packet = new UdpPacket();
            packet.Address = addr;
            packet.Port = this.Port;
            packet.ToMAC = string.Empty;
            packet.Command = UdpPacket.CMD_NOTHING;
            packet.FromMAC = this.MAC;

            _client.Send(packet);
        }

        private void Close()
        {
            _client.Close();
        }

        public void Exit()
        {
            foreach (IPAddress broadcastAddr in this.BroadcastAddress)
            {
                SendEmptyPacket(broadcastAddr);

                this.Status = UserStatus.Offline;

                UdpPacket packet = new UdpPacket();
                packet.Address = broadcastAddr;
                packet.Port = this.Port;
                packet.ToMAC = string.Empty;
                packet.Command = UdpPacket.CMD_EXIT;
                packet.FromMAC = this.MAC;

                _client.Send(packet);
                Thread.Sleep(100);
            }

            Close();
        }

        public long SendTextMessage(LanUser user, string text)
        {
            UdpPacket packet = new UdpPacket();
            packet.Address = user.Address;
            packet.Port = user.Port;
            packet.ToMAC = user.MAC;
            packet.Command = UdpPacket.CMD_SEND_TEXT | UdpPacket.CMD_OPTION_NEED_RESPONSE;
            packet.FromMAC = this.MAC;

            UdpPacketTextExtend extend = new UdpPacketTextExtend();
            extend.EncryptKey = user.SecurityKeys.Public;
            extend.Text = text;
            packet.Extend = extend;

            SendPacket(packet, user._trans);

            return packet.ID;
        }

        public long SendImage(LanUser user, string imagePath)
        {
            return SendImageCore(user, null, imagePath);
        }

        //public long SendImage(LanUser user, Image image)
        //{
        //    return SendImageCore(user, image, null);
        //}

        private long SendImageCore(LanUser user, Image image, string imagePath)
        {
            string path;
            if (image != null)
            {
                path = LanEnv.GetNotExistFileName(LanEnv.TempPath, Path.GetFileName(imagePath));
                image.Save(path, image.RawFormat);
            }
            else
            {
                path = imagePath;
            }

            long len = LanFile.GetFileLength(path);
            if (len > UdpClientEx.UDP_MAX_BUF_SIZE)
            {
                //图像文件过大的话用文件形式发送
                return SendFile(user, path, true);
            }
            else
            {
                UdpPacket packet = new UdpPacket();
                packet.Address = user.Address;
                packet.Port = user.Port;
                packet.ToMAC = user.MAC;
                packet.Command = UdpPacket.CMD_SEND_IMAGE | UdpPacket.CMD_OPTION_NEED_RESPONSE;
                packet.FromMAC = this.MAC;

                UdpPacketImageExtend extend = new UdpPacketImageExtend();
                extend.EncryptKey = user.SecurityKeys.Public;
                extend.Image = image ?? Image.FromFile(path);
                extend.FileName = Path.GetFileName(imagePath);
                packet.Extend = extend;

                SendPacket(packet, user._trans);
                _client.Send(packet);

                if (image != null)
                {
                    //直接发送图像的话，临时图片就删除
                    LanFile.Delete(path);
                }

                return packet.ID;
            }
        }

        public long SendFile(LanUser user, string path, bool bImage = false)
        {
            //发送请求
            UdpPacket packet = new UdpPacket();
            packet.Address = user.Address;
            packet.Port = user.Port;
            packet.ToMAC = user.MAC;
            packet.Command = UdpPacket.CMD_SEND_FILE_REQUEST | UdpPacket.CMD_OPTION_NEED_RESPONSE;
            if(bImage)
            {
                packet.Command |= UdpPacket.CMD_OPTION_SEND_FILE_IMAGE;
            }
            packet.FromMAC = this.MAC;

            UdpPacketSendFileRequestExtend extend = new UdpPacketSendFileRequestExtend();
            extend.EncryptKey = user.SecurityKeys.Public;
            extend.File = new LanFile(path);

            packet.Extend = extend;

            _client.Send(packet);

            //保存要发送文件一览
            TransportFile file = new TransportFile(packet.ID, user.MAC, user.Address, user.Port, user.SecurityKeys.Public, extend.File);
            _fileTransTcpListener.AddTransportFile(file);

            return file.ID;
        }

        public void ReceiveFile(TransportFile file)
        {
            FileTransportTcpClient client = new FileTransportTcpClient(this._context);
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
            if (Send != null)
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
                    packetRsp.Address = packet.Address;
                    packetRsp.Port = packet.Port;
                    packet.ToMAC = packet.ToMAC;
                    packetRsp.FromMAC = this.MAC;
                    packetRsp.Command = UdpPacket.CMD_RESPONSE;

                    UdpPacketResponseExtend extend = new UdpPacketResponseExtend();
                    extend.ID = packet.ID;
                    packetRsp.Extend = extend;

                    _client.Send(packetRsp);
                }
                #endregion
            }

            ReceivePacketCore(packet, false);
        }

        private void ReceivePacketCore(UdpPacket packet, bool isRetrans)
        { 
            if (packet.CMD == UdpPacket.CMD_NOTHING)
            {
                //do nothing
            }
            else if (packet.CMD == UdpPacket.CMD_HEART_BEAT)
            {
                this._serverAvailable = true;
            }
            else if (packet.CMD == UdpPacket.CMD_ENTRY)
            {
                #region CMD_ENTRY 对方上线
                UdpPacketUserStateExtend extend = packet.Extend as UdpPacketUserStateExtend;
                LanUser user = this[packet.FromMAC];
                user.Update(extend.User, extend.UpdateState);

                if (!isRetrans)
                {
                    //同一个Lan内收到的广播，才需要给对方回复一个自己的状态
                    SendUpdateStatePacket(UpdateState.NickName | UpdateState.PublicKey | UpdateState.Status,
                        packet.Address, packet.Port, packet.FromMAC);

                    user._trans = false;
                }
                else
                {
                    //服务器转送过来的，无需回复，应该由服务器更新用户信息一览给发送者
                }

                if (UserEntry != null)
                {
                    UserStateChangeEventArgs stateArgs = new UserStateChangeEventArgs(user, extend.UpdateState);
                    UserEntry(this, stateArgs);
                }
                #endregion
            }
            else if(packet.CMD == UdpPacket.CMD_EXIT)
            {
                #region CMD_EXIT
                if (UserExit != null)
                {
                    LanUser user = this[packet.FromMAC];
                    user.Status = UserStatus.Offline;
                    UserStateChangeEventArgs stateArgs = new UserStateChangeEventArgs(user, UpdateState.Status);
                    UserExit(this, stateArgs);
                }
                #endregion
            }
            else if (packet.CMD == UdpPacket.CMD_STATE)
            {
                #region CMD_STATE
                UdpPacketUserStateExtend extend = packet.Extend as UdpPacketUserStateExtend;
                LanUser user = this[packet.FromMAC];
                user.Update(extend.User, extend.UpdateState);
                if ((extend.UpdateState & UpdateState.Photo) != 0)
                {
                    //缓存本地头像
                    ProfilePhotoPool.SetPhoto(user.ID, user.ProfilePhoto);
                }

                if (UserStateChange != null)
                {
                    UserStateChangeEventArgs stateArgs = new UserStateChangeEventArgs(user, extend.UpdateState);
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
                    LanUser user = this[packet.FromMAC];
                    TextMessageReceivedEventArgs stateArgs = new TextMessageReceivedEventArgs(user, packet.ID, extend.Text);
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
                    LanUser user = this[packet.FromMAC];
                    ImageReceivedEventArgs stateArgs = new ImageReceivedEventArgs(user, packet.ID, extend.Image, extend.FileName);
                    ImageReceived(this, stateArgs);
                }
                #endregion
            }
            else if (packet.CMD == UdpPacket.CMD_SEND_FILE_REQUEST)
            {
                #region CMD_SEND_FILE_REQUEST
                LanUser user = this[packet.FromMAC];
                UdpPacketSendFileRequestExtend extend = packet.Extend as UdpPacketSendFileRequestExtend;

                TransportFile file = new TransportFile(packet.ID, user.MAC, user.Address, user.Port, user.SecurityKeys.Public, extend.File);

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
            else if (packet.CMD == UdpPacket.CMD_RETRANSMIT)
            {
                UdpPacketRetransExtend extend = packet.Extend as UdpPacketRetransExtend;

                IPacketResolver resolver = PacketResolverFactory.CreateResolver(extend.PacketBuf, 0, extend.Length, this.SecurityKeys.Private);
                UdpPacket rawPacket = resolver.Resolve() as UdpPacket;
                rawPacket.Address = extend.Address;
                rawPacket.Port = extend.Port;
                ReceivePacketCore(rawPacket, true);
            }
            else if (packet.CMD == UdpPacket.CMD_USER_LIST)
            {
                //用户一览更新
                UdpPacketUserListExtend extend = packet.Extend as UdpPacketUserListExtend;
                List<User> users = extend.Users;

            }
            else
            {
                throw new Exception("未想定命令");
            }
        }

        private void SendUpdateStatePacket(UpdateState state, IPAddress remote, int port, string mac)
        {
            if((state & UpdateState.Photo) != 0)
            {
                this.ProfilePhoto = ProfilePhotoPool.GetPhoto(this.ID);
            }

            //发送自己的信息，让发送方也更新
            UdpPacket packetRsp = new UdpPacket();
            packetRsp.Address = remote;
            packetRsp.Port = port;
            packetRsp.ToMAC = mac;
            packetRsp.FromMAC = this.MAC;
            packetRsp.Command = UdpPacket.CMD_STATE;

            UdpPacketUserStateExtend entryExtend = new UdpPacketUserStateExtend();
            entryExtend.User = this;
            entryExtend.UpdateState = state;

            packetRsp.Extend = entryExtend;

            _client.Send(packetRsp);
        }

        public void UpdateMyStateByBroadcast(UpdateState state)
        {
            foreach (IPAddress item in this.BroadcastAddress)
            {
                SendUpdateStatePacket(state, item, this.Port, "");
                Thread.Sleep(50);
            }
        }

        private void ReceiveImage(LanUser user, TransportFile file)
        {
            file.SavePath = LanEnv.GetNotExistFileName(LanEnv.PicturePath, file.File.Name);

            FileTransportTcpClient client = new FileTransportTcpClient(_context);
            client.ProgressChanged += ImageReceiveProgressChanged;
            client.Completed += new FileTransportEventHandler((sender, args)=>
            {
                Image image = Image.FromFile(file.SavePath);
                ImageReceivedEventArgs stateArgs = new ImageReceivedEventArgs(user, file.ID, image, Path.GetFileName(file.SavePath));
                ImageReceived(this, stateArgs);
            });
            client.Error += ImageReceiveError;
            client.Receive(file);
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}][{2}][{3}]",
                this.NickName, this.MAC, this.Address,
                this.Status == UserStatus.Online ? "在线" : 
                    this.Status == UserStatus.Busy ? "忙碌" : "离线");
        }
    }
}
