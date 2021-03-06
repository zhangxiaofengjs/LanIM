﻿using Com.LanIM.Common;
using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packets;
using Com.LanIM.Network.PacketEncoder;
using Com.LanIM.Network.PacketResolver;
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
        enum ClientState
        {
            Waiting,
            Working,
            Closing,
            Closed,
        }

        public const int DEFAULT_PORT = 2425;
        public const int DEFAULT_RECEIVE_BUF_SIZE = 1024 * 64;//16k
        private const int SEND_RESPONSE_CHECK_DELAY = 5000;
        //public const int UDP_MAX_BUF_SIZE = 59392;//本来是65507，考虑到有些余量定为58k
        public const int UDP_MAX_BUF_SIZE = 1400;//跨网的时候根据测试设定大于1472字节不能正常传输TODO分片发送

        private UdpClient _client;
        private ClientState _state = ClientState.Waiting;
        public int SendResponseCheckDelay { get; set; } = SEND_RESPONSE_CHECK_DELAY;
        public int Port { get; set; } = DEFAULT_PORT;
        public IPAddress IP { get; set; }

        public SecurityKeys SecurityKeys { get; set; } = new SecurityKeys();

        private TaskFactory _taskFactory = new TaskFactory();
        private BlockingCollection<UdpPacket> _receivedPackets = new BlockingCollection<UdpPacket>();
        private ConcurrentDictionary<long, UdpPacket> _sendingPacketDic = new ConcurrentDictionary<long, UdpPacket>();
        private ConcurrentDictionary<long, MultiUdpPacket> _recvingPacketDic = new ConcurrentDictionary<long, MultiUdpPacket>();

        public event UdpClientReceiveEventHandler ReceivePacket;
        public event UdpClientSendEventHandler SendPacket;
        public event UdpClientErrorEventHandler Error;

        public int ReceiveBufferSize = DEFAULT_RECEIVE_BUF_SIZE;
        private SynchronizationContext _context;

        public UdpClientEx(SynchronizationContext context)
        {
            this._context = context;
        }

        public void Close()
        {
            try
            {
                LoggerFactory.Debug("closing");

                this._state = ClientState.Closing;
                if (_client != null)
                {
                    _client.Close();
                    _client.Dispose();
                }

                this._state = ClientState.Closed;
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

                IPEndPoint localIpEp = new IPEndPoint(IP, Port);
                _client = new UdpClient(localIpEp)
                {
                    EnableBroadcast = true
                };
                _client.AllowNatTraversal(true);
                _client.Client.ReceiveBufferSize = this.ReceiveBufferSize;

                //开始接受消息
                _client.BeginReceive(AsyncReceiveHandler, null);

                this._state = ClientState.Working;

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
            bool isExiting = false;

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
                        packet.Address = remoteEp.Address;
                        packet.Port = remoteEp.Port;
                        LoggerFactory.Debug("resolved packet:{0}", packet);

                        OnPackageReceived(packet);

                        if(packet.Type == Packet.PACKTE_TYPE_UDP &&
                            packet.CMD == UdpPacket.CMD_EXIT &&
                            packet.Address.Equals(this.IP))
                        {
                            //如果是当前用户发出退出请求，下一步则不监听了
                            isExiting = true;
                        }
                    }
                    catch (Exception e)
                    {
                        OnError(Errors.ResolveError, "解包错误。", e);
                    }
                }
            }

            if (isExiting || 
                this._state == ClientState.Closing ||
                this._state == ClientState.Closed)
            {
                //正在准备退出
            }
            else
            { 
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
        }

        protected void OnPackageReceived(UdpPacket packet)
        {
            if (packet.Type == Packet.PACKTE_TYPE_MULTI_UDP)
            {
                MultiUdpPacket mp = packet as MultiUdpPacket;
                if(!_recvingPacketDic.TryGetValue(mp.ParentID, out MultiUdpPacket parentMp))
                {
                    parentMp = new MultiUdpPacket(mp.TotalLength);
                    parentMp.ID = mp.ParentID;
                    parentMp.Address = packet.Address;
                    parentMp.Port = packet.Port;

                    _recvingPacketDic.TryAdd(mp.ParentID, parentMp);
                }
                parentMp.CopyFragmentBuff(mp);

                if(parentMp.TotalLength == parentMp.Length)
                {
                    //收取中缓存移除，添加到已经接受的包中
                    _recvingPacketDic.TryRemove(mp.ParentID, out MultiUdpPacket pMp);

                    IPacketResolver resolver = PacketResolverFactory.CreateResolver(pMp.FragmentBuff, 0, pMp.TotalLength, this.SecurityKeys.Private);
                    LoggerFactory.Debug("get resolver:{0}", resolver.GetType().Name);

                    UdpPacket udpPacket = resolver.Resolve() as UdpPacket;
                    udpPacket.Address = packet.Address;
                    udpPacket.Port = packet.Port;

                    _receivedPackets.Add(udpPacket);
                }
            }
            else
            {
                _receivedPackets.Add(packet);
            }
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
            ReceivePacket?.Invoke(this, state as UdpClientReceiveEventArgs);
        }
        #endregion

        #region Send
        struct SendState
        {
            public bool LastPacket;
            public UdpPacket Packet;
        }
        public void Send(UdpPacket packet)
        {
            if (this._state != ClientState.Working)
            {
                return;
            }

            //异步发送
            IPEndPoint remoteIpEp = new IPEndPoint(packet.Address, packet.Port);

            packet.GenerateID();

            LoggerFactory.Debug("parepare send packet:{0}", packet);

            EncodeResult result = null;
            try
            {
                IPacketEncoder encoder = PacketEncoderFactory.CreateEncoder(packet);
                LoggerFactory.Debug("get encoder:{0}", encoder.GetType().Name);

                result = encoder.Encode();
                LoggerFactory.Debug("encode packet:{0}", result);

                if (result.Length > UDP_MAX_BUF_SIZE)
                {
                    //超过大小的分包处理
                    MultiUdpPacket mpacket = new MultiUdpPacket(result.Fragments[0]);
                    mpacket.ID = packet.ID;
                    mpacket.ParentID = packet.ID;
                    mpacket.MaxFragmentLength = UDP_MAX_BUF_SIZE - MultiUdpPacket.HEAD_SIZE;

                    encoder = PacketEncoderFactory.CreateEncoder(mpacket);
                    LoggerFactory.Debug("get encoder:{0}", encoder.GetType().Name);

                    result = encoder.Encode();
                    LoggerFactory.Debug("encode packet:{0}", result);
                }
            }
            catch (Exception e)
            {
                OnSendPackage(packet, false);
                OnError(Errors.EncodeError, "加密包错误。", e);
            }

            try
            {
                for (int i = 0; i < result.Fragments.Count; i++)
                {
                    byte[] buf = result.Fragments[i];
                    SendState state = new SendState();
                    state.LastPacket = i == result.Fragments.Count - 1;
                    state.Packet = packet;
                    _client.BeginSend(buf, buf.Length, remoteIpEp, new AsyncCallback(AsyncSendHandler), state);
                    Thread.Sleep(20);//稍微等待一下，避免丢包
                }
            }
            catch (Exception e)
            {
                OnSendPackage(packet, false);
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

                SendState state = (SendState)ar.AsyncState;
                if (state.LastPacket)
                {
                    //最后一个包发完开始检测结果
                    UdpPacket packet = state.Packet;
                    if (packet.CheckSendResponse)
                    {
                        // 添加到正在发送包一览，随后启动定时器检测有无返回结果
                        LoggerFactory.Debug("add to check response:id={0}", packet.ID);

                        if (!_sendingPacketDic.TryAdd(packet.ID, packet))
                        {
                            //应该不会到这里来
                            OnError(Errors.Unknow, "未想定异常：AsyncSendHandler", null);
                        }

                        WaitTimer.Once(this.SendResponseCheckDelay, SendResultCheckHandler, packet);
                    }
                    else
                    {
                        //对于不需要确认的直接认为发送完毕
                        OnSendPackage(packet, true);
                    }
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
                _sendingPacketDic.TryRemove(packet.ID, out UdpPacket pkt);

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
            SendPacket?.Invoke(this, state as UdpClientSendEventArgs);
        }

        public void NotifySendPacketSuccess(long id)
        {
            LoggerFactory.Debug("send packet success:id={0}", id);
            if (_sendingPacketDic.TryRemove(id, out UdpPacket pkt))
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
