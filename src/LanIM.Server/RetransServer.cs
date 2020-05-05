using Com.LanIM.Common;
using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network;
using Com.LanIM.Network.PacketEncoder;
using Com.LanIM.Network.PacketResolver;
using Com.LanIM.Network.Packets;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.LanIM.Server
{
    class RetransServer
    {
        private UdpClientEx _client;
        private SynchronizationContext _context = null;
        public int Port { get; set; } = 2425;
        public IPAddress IP { get; set; } = IPAddress.Any;
        public string MAC { get; set; } = string.Empty;

        private List<ClientUser> _users = new List<ClientUser>();
        private ClientUser this[string id]
        {
            get
            {
                ClientUser user = _users.Find((u) => {
                    if(u.ID == id)
                    {
                        return true;
                    }
                    return false;
                });

                if(user == null)
                {
                    user = new ClientUser();
                    user.ID = id;
                }

                return user;
            }
        }

        public RetransServer(SynchronizationContext context = null)
        {
            _context = context;
        }

        internal bool Start()
        {
            if (_client != null)
            {
                _client.Close();
            }
            _client = new UdpClientEx(_context)
            {
                Port = this.Port,
                IP = this.IP,
            };
            _client.SendPacket += this.SendPacketEvent;
            _client.ReceivePacket += this.ReceivePacketEvent;

            //监听端口开始收信
            if (!_client.Listen())
            {
                return false;
            }
            
            return true;
        }

        private void SendPacketEvent(object sender, UdpClientSendEventArgs args)
        {
            //UdpPacket packet = args.Packet as UdpPacket;
            //if(packet.CMD == UdpPacket.CMD_RETRANSMIT)
            //{
            //    if(args.Success)
            //    {
            //        UdpPacketResponseExtend extend = packet.Extend as UdpPacketResponseExtend;
            //        extend.ID = 
            //        _client.NotifySendPacketSuccess(extend.ID);
            //    }
            //}
        }

        private void ReceivePacketEvent(object sender, UdpClientReceiveEventArgs args)
        {
            UdpPacket packet = args.Packet;

            switch (packet.CMD)
            {
                case UdpPacket.CMD_NOTHING: break;//do nothing
                case UdpPacket.CMD_RETRANSMIT: RetransPacket(packet); break;//转送包
                case UdpPacket.CMD_HEART_BEAT: HeartBeat(packet); break;
                case UdpPacket.CMD_ENTRY: 
                case UdpPacket.CMD_STATE: 
                case UdpPacket.CMD_EXIT: UserUpdateState(packet); break;
                case UdpPacket.CMD_RESPONSE: NotifyResponseFromUser(packet); break;
                default: break;
            }
        }

        private void RetransPacket(UdpPacket packet)
        {
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

                    UdpPacketResponseExtend extendRsp = new UdpPacketResponseExtend();
                    extendRsp.ID = packet.ID;
                    packetRsp.Extend = extendRsp;

                    _client.Send(packetRsp);
                }
                #endregion
            }

            UdpPacketRetransExtend extend = packet.Extend as UdpPacketRetransExtend;

            //修改目的地后转送
            UdpPacket retransPacket = new UdpPacket();
            retransPacket.Address = extend.Address;//传送目的地
            retransPacket.Port = extend.Port;
            retransPacket.FromMAC = this.MAC;
            retransPacket.Command = UdpPacket.CMD_RETRANSMIT | UdpPacket.CMD_OPTION_NEED_RESPONSE;

            UdpPacketRetransExtend retransExtend = new UdpPacketRetransExtend(extend.PacketBuf);
            //更改来源地址
            retransExtend.PacketID = packet.ID;
            retransExtend.Address = packet.Address;
            retransExtend.Port = packet.Port;
            retransPacket.Extend = extend;

            _client.Send(retransPacket);

            LoggerFactory.Debug("retrans:{0}", retransPacket);
        }

        private void HeartBeat(UdpPacket packet)
        {
            ClientUser user = this[packet.FromMAC];
            user.LastHeartBeat = DateTime.Now;

            UdpPacket ansPacket = new UdpPacket();
            ansPacket.Address = packet.Address;
            ansPacket.Port = packet.Port;
            ansPacket.FromMAC = this.MAC;
            ansPacket.Command = UdpPacket.CMD_HEART_BEAT;

            _client.Send(ansPacket);
        }

        private void UserUpdateState(UdpPacket packet)
        {
            UdpPacketUserStateExtend extend = packet.Extend as UdpPacketUserStateExtend;

            //更新用户状态
            ClientUser user = this[packet.FromMAC];
            user.Update(extend.User, extend.UpdateState);

            //用户一览信息回给上线者
            UdpPacket packetUsers = new UdpPacket();
            packetUsers.Address = packet.Address;
            packetUsers.Port = packet.Port;
            packetUsers.ToMAC = packet.FromMAC;
            packetUsers.FromMAC = this.MAC;
            packetUsers.Command = UdpPacket.CMD_USER_LIST;
            UdpPacketUserListExtend extendUserList = new UdpPacketUserListExtend();
            packetUsers.Extend = extendUserList;
            foreach (User u in _users)
            {
                extendUserList.AddUser(u);
            }
            _client.Send(packetUsers);

            //上线信息广播给各位
            foreach (IPAddress brdIp in LanServerConfig.Instance.BroadcastAddress)
            {
                SendUpdateStatePacket(extend.UpdateState, user, brdIp);
                Thread.Sleep(50);
            }
        }

        private void SendUpdateStatePacket(UpdateState state, User user, IPAddress brdIp)
        {
            UdpPacket packet = new UdpPacket();
            packet.Address = user.Address;
            packet.Port = user.Port;
            packet.ToMAC = "";
            packet.FromMAC = user.ID;
            packet.Command = UdpPacket.CMD_STATE;

            UdpPacketUserStateExtend entryExtend = new UdpPacketUserStateExtend();
            entryExtend.User = user;
            entryExtend.UpdateState = state;

            packet.Extend = entryExtend;

            UdpPacket packetWrap = new UdpPacket();
            packetWrap.Address = brdIp;
            packetWrap.Port = this.Port;
            packetWrap.ToMAC = "";
            packetWrap.FromMAC = this.MAC;
            packetWrap.Command = UdpPacket.CMD_RETRANSMIT;

            IPacketEncoder encoder = PacketEncoderFactory.CreateEncoder(packet);
            EncodeResult result = encoder.Encode();

            UdpPacketRetransExtend extend = new UdpPacketRetransExtend(result.Fragments[0]);
            packetWrap.Extend = extend;

            _client.Send(packetWrap);
        }

        private void NotifyResponseFromUser(UdpPacket packet)
        {
            UdpPacketResponseExtend extend = packet.Extend as UdpPacketResponseExtend;
            _client.NotifySendPacketSuccess(extend.ID);
        }
    }
}
