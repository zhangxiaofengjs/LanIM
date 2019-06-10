using Com.LanIM.Common;
using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packet;
using Com.LanIM.Network.PacketEncoder;
using Com.LanIM.Network.PacketResolver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Com.LanIM.Network
{
    public class UdpClientEx
    {
        public const int DEFAULT_PORT = 2425;
        private const int SEND_RESPONSE_CHECK_DELAY = 5000;
        private const int UDP_MAX_BUF_SIZE = 65507;
        
        private UdpClient _client;
        public int SendResponseCheckDelay { get; set; }
        public int Port { get; set; }

        public SecurityKeys SecurityKeys { get; set; }

        private TaskFactory _taskFactory = new TaskFactory();
        private BlockingCollection<UdpPacket> _receivedPackets = new BlockingCollection<UdpPacket>();
        private ConcurrentDictionary<long, UdpPacket> _sendingPacketDic = new ConcurrentDictionary<long, UdpPacket>();

        public event UdpClientReceiveEventHandler ReceivePacket;
        public event UdpClientSendEventHandler SendPacket;

        public int ReceiveBufferSize = 8192;
        private SynchronizationContext _context;

        public UdpClientEx(SynchronizationContext context)
        {
            this.Port = DEFAULT_PORT;
            this.SendResponseCheckDelay = SEND_RESPONSE_CHECK_DELAY;
            this._context = context;
        }

        private void CreateUdpClient()
        {
            if (_client == null)
            {
                IPEndPoint localIpEp = new IPEndPoint(IPAddress.Any, Port);
                _client = new UdpClient(localIpEp);
                _client.EnableBroadcast = true;
                _client.AllowNatTraversal(true);
            }
        }

        public void Close()
        {
            if (_client != null)
            {
                _client.Close();
            }
        }

        #region Receive
        /// <summary>
        /// 监听某个端口进行消息收取
        /// </summary>
        public bool Listen()
        {
            try
            {
                CreateUdpClient();
                _client.Client.ReceiveBufferSize = this.ReceiveBufferSize;
                _client.BeginReceive(AsyncReceiveHandler, null);

                //由于UI处理时间比较长，开启收包处理任务
                StartReceiveTask();

                return true;
            }
            catch(Exception e)
            {
                LoggerFactory.Instance().Error("[listen error]", e);
            }
            return false;
        }

        private void AsyncReceiveHandler(IAsyncResult ar)
        {
            try
            {
                if (ar.IsCompleted)
                {
                    IPEndPoint remoteEp = null;
                    byte[] buff = _client.EndReceive(ar, ref remoteEp);
                    LoggerFactory.Instance().Debug("[recv]{0}", UdpPacket.ENCODING.GetString(buff));

                    IUdpPacketResolver resolver = UdpPacketResolverFactory.CreateResolver(buff, this);
                    UdpPacket packet = resolver.Resolve();

                    if(packet == null)
                    {
                        throw new Exception("收到未想定数据包");
                    }

                    packet.Remote = remoteEp.Address;
                    packet.Port = remoteEp.Port;

                    OnPackageReceived(packet);

                    //继续下一次收包
                    _client.BeginReceive(AsyncReceiveHandler, null);
                }
            }
            catch (Exception e)
            {
                LoggerFactory.Instance().Error("[recv error]{0}", e);
            }
        }
        protected void OnPackageReceived(UdpPacket packet)
        {
            _receivedPackets.Add(packet);
        }

        private void StartReceiveTask()
        {
            Task _task = _taskFactory.StartNew(() =>
            {
                while (true)
                {
                    UdpPacket packet = _receivedPackets.Take();

                    UdpClientReceiveEventArgs args = new UdpClientReceiveEventArgs(packet);
                    if (_context == null)
                    {
                        ReceivePacketdSendOrPostCallBack(args);
                    }
                    else
                    {
                        _context.Post(ReceivePacketdSendOrPostCallBack, args);
                    }
                }
            });
        }

        private void ReceivePacketdSendOrPostCallBack(object state)
        {
            if (ReceivePacket != null)
            {
                ReceivePacket(this, state as UdpClientReceiveEventArgs);
            }
        }
        #endregion

        #region Send
        public void Send(UdpPacket packet)
        {
            CreateUdpClient();

            //异步发送
            IPEndPoint remoteIpEp = new IPEndPoint(packet.Remote, packet.Port);

            packet.GenerateID();

            IUdpPacketEncoder encoder = UdpPacketEncoderFactory.CreateEncoder(packet);

            byte[] buf = encoder.Encode();
            if (buf != null)
            {
                if (buf.Length < UDP_MAX_BUF_SIZE)
                {
                    _client.BeginSend(buf, buf.Length, remoteIpEp, new AsyncCallback(AsyncSendHandler), packet);
                }
                else
                {
                    //超过大小的，分包？转TCP？
                }
            }
        }

        private void AsyncSendHandler(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                try
                {
                    UdpPacket packet = ar.AsyncState as UdpPacket;
                    _client.EndSend(ar);
                    LoggerFactory.Instance().Debug("[send]{0}", packet);

                    if (packet.CheckSendResponse)
                    {
                        // 添加到正在发送包一览，随后启动定时器检测有无返回结果
                        if (!_sendingPacketDic.TryAdd(packet.ID, packet))
                        {
                            //应该不会到这里来
                            throw new Exception("未想定异常：AsyncSendHandler");
                        }
                        WaitTimer.Start(this.SendResponseCheckDelay, SendResultCheckHandler, packet);
                    }
                    else
                    {
                        //对于不需要确认的直接认为发送完毕
                        OnSendPackage(packet, true);
                    }
                }
                catch (Exception e)
                {
                    LoggerFactory.Instance().Error("[send error]{0}", e);
                }
            }
        }

        private void SendResultCheckHandler(object state)
        {
            UdpPacket packet = state as UdpPacket;
            if (_sendingPacketDic.ContainsKey(packet.ID))
            {

                // 超时仍在队列中
                UdpPacket pkt = null;
                _sendingPacketDic.TryRemove(packet.ID, out pkt);

                OnSendPackage(packet, false);
            }
        }

        protected void OnSendPackage(UdpPacket packet, bool success)
        {
            UdpClientSendEventArgs args = new UdpClientSendEventArgs(packet, success);
            if (_context == null)
            {
                SendPacketSendOrPostCallBack(args);
            }
            else
            {
                //在指定线程上调用事件委托
                _context.Post(SendPacketSendOrPostCallBack, args);
            }
        }

        private void SendPacketSendOrPostCallBack(object state)
        {
            if (SendPacket != null)
            {
                SendPacket(this, state as UdpClientSendEventArgs);
            }
        }

        public void NotifySendPacketSuccess(long id)
        {
            UdpPacket pkt = null;
            if (_sendingPacketDic.TryRemove(id, out pkt))
            {
                OnSendPackage(pkt, true);
            }
        }
        #endregion
    }
}
