using Com.LanIM.Common;
using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packets;
using Com.LanIM.Network.PacketsEncoder;
using Com.LanIM.Network.PacketsResolver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static Com.LanIM.Network.UdpClientErrorEventArgs;

namespace Com.LanIM.Network
{
    public class UdpClientEx
    {
        public const int DEFAULT_PORT = 2425;
        public const int DEFAULT_RECEIVE_BUF_SIZE = 1024 * 64;//16k
        private const int SEND_RESPONSE_CHECK_DELAY = 5000;
        public const int UDP_MAX_BUF_SIZE = 59392;//本来是65507，考虑到有些余量定为58k

        private UdpClient _client;
        public int SendResponseCheckDelay { get; set; }
        public int Port { get; set; }

        public SecurityKeys SecurityKeys { get; set; }

        private TaskFactory _taskFactory = new TaskFactory();
        private BlockingCollection<UdpPacket> _receivedPackets = new BlockingCollection<UdpPacket>();
        private ConcurrentDictionary<long, UdpPacket> _sendingPacketDic = new ConcurrentDictionary<long, UdpPacket>();

        public event UdpClientReceiveEventHandler ReceivePacket;
        public event UdpClientSendEventHandler SendPacket;
        public event UdpClientErrorEventHandler Error;

        public int ReceiveBufferSize = DEFAULT_RECEIVE_BUF_SIZE;
        private SynchronizationContext _context;

        public UdpClientEx(SynchronizationContext context)
        {
            this.Port = DEFAULT_PORT;
            this.SendResponseCheckDelay = SEND_RESPONSE_CHECK_DELAY;
            this._context = context;
        }

        public void Close()
        {
            try
            {
                LoggerFactory.Debug("closing");

                if (_client != null)
                {
                    _client.Close();
                    _client.Dispose();
                }

                LoggerFactory.Debug("closed");
            }
            catch (Exception e)
            {
                OnError(Errors.OutofSizePacket, "监听停止错误。", e);
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
                LoggerFactory.Debug("listen prepare");

                IPEndPoint localIpEp = new IPEndPoint(IPAddress.Any, Port);
                _client = new UdpClient(localIpEp);
                _client.EnableBroadcast = true;
                _client.AllowNatTraversal(true);
                _client.Client.ReceiveBufferSize = this.ReceiveBufferSize;

                //开始接受消息
                _client.BeginReceive(AsyncReceiveHandler, null);
                LoggerFactory.Debug("begin receive");

                //由于UI处理时间比较长，开启收包处理任务
                StartReceiveTask();

                LoggerFactory.Debug("receive task started");

                return true;
            }
            catch (Exception e)
            {
                OnError(Errors.OutofSizePacket, "监听错误。可能端口已经被其他程序占用。查看端口占用进程请执行：netstat -ano | findstr " + this.Port, e);
            }
            return false;
        }

        private void AsyncReceiveHandler(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                byte[] buff = null;
                IPEndPoint remoteEp = null;
                try
                {
                    buff = _client.EndReceive(ar, ref remoteEp);
                    LoggerFactory.Debug("received:{0}", buff);
                }
                catch (Exception e)
                {
                    OnError(Errors.OutofSizePacket, "收包错误。", e);
                }

                if (buff != null && buff.Length > 0)
                {
                    try
                    {
                        IPacketResolver resolver = PacketResolverFactory.CreateResolver(buff, 0, buff.Length, this.SecurityKeys.Private);
                        LoggerFactory.Debug("get resolver:{0}", resolver.GetType().Name);

                        UdpPacket packet = resolver.Resolve() as UdpPacket;
                        packet.Remote = remoteEp.Address;
                        packet.Port = remoteEp.Port;
                        LoggerFactory.Debug("resolved packet:{0}", packet);

                        OnPackageReceived(packet);
                    }
                    catch (Exception e)
                    {
                        OnError(Errors.ResolveError, "解包错误。", e);
                    }
                }
            }

            try
            {
                //继续下一次收包
                _client.BeginReceive(AsyncReceiveHandler, null);
                LoggerFactory.Debug("begin receive");
            }
            catch (Exception e)
            {
                OnError(Errors.OutofSizePacket, "接受消息停止,需要重新监听....", e);
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
            //异步发送
            IPEndPoint remoteIpEp = new IPEndPoint(packet.Remote, packet.Port);

            packet.GenerateID();

            LoggerFactory.Debug("parepare send packet:{0}", packet);

            byte[] buf = null;
            try
            {
                IPacketEncoder encoder = PacketEncoderFactory.CreateEncoder(packet);
                LoggerFactory.Debug("get encoder:{0}", encoder.GetType().Name);

                buf = encoder.Encode();
                LoggerFactory.Debug("encode packet:{0}", buf);
            }
            catch (Exception e)
            {
                OnError(Errors.EncodeError, "加密包错误。", e);
            }

            try
            {
                if (buf.Length < UDP_MAX_BUF_SIZE)
                {
                    _client.BeginSend(buf, buf.Length, remoteIpEp, new AsyncCallback(AsyncSendHandler), packet);
                }
                else
                {
                    //超过大小的，分包？转TCP？
                    OnError(Errors.OutofSizePacket, "包超过大小。", null);
                }
            }
            catch (Exception e)
            {
                OnError(Errors.NetworkError, "发送包错误。", e);
            }
        }

        private void AsyncSendHandler(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                try
                {
                    int len = _client.EndSend(ar);
                    LoggerFactory.Debug("send:{0} bytes", len);

                }
                catch (Exception e)
                {
                    OnError(Errors.NetworkError, "发送包失败。", e);
                }

                UdpPacket packet = ar.AsyncState as UdpPacket;
                if (packet.CheckSendResponse)
                {
                    // 添加到正在发送包一览，随后启动定时器检测有无返回结果
                    LoggerFactory.Debug("add to check response:id={0}", packet.ID);

                    if (!_sendingPacketDic.TryAdd(packet.ID, packet))
                    {
                        //应该不会到这里来
                        OnError(Errors.Unknow, "未想定异常：AsyncSendHandler", null);
                    }

                    WaitTimer.Start(this.SendResponseCheckDelay, SendResultCheckHandler, packet);
                }
                else
                {
                    //对于不需要确认的直接认为发送完毕
                    OnSendPackage(packet, true);
                }
            }
        }

        private void SendResultCheckHandler(object state)
        {
            UdpPacket packet = state as UdpPacket;
            LoggerFactory.Debug("check send packet success:id={0}", packet.ID);

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
            LoggerFactory.Debug("send packet event");
            if (SendPacket != null)
            {
                SendPacket(this, state as UdpClientSendEventArgs);
            }
        }

        public void NotifySendPacketSuccess(long id)
        {
            LoggerFactory.Debug("send packet success:id={0}", id);
            UdpPacket pkt = null;
            if (_sendingPacketDic.TryRemove(id, out pkt))
            {
                OnSendPackage(pkt, true);
            }
        }

        protected virtual void OnError(Errors error, string message, Exception e)
        {
            LoggerFactory.Error("{0}, exception={1}", message, e);

            if (this.Error != null)
            {
                UdpClientErrorEventArgs args = new UdpClientErrorEventArgs(error, message, e);
                this.Error(this, args);
            }
        }
        #endregion
    }
}
