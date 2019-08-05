using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.PacketResolver
{
    public class PacketResolverFactory
    {
        public static IPacketResolver CreateResolver(byte[] datagram, int startIndex, int length, byte[] securityKey)
        {
            if(datagram == null || datagram.Length < 2)
            {
                throw new Exception("创建包解码器失败，未知包类型。");
            }

            if (datagram[0] == 49)
            {
                short version = BitConverter.ToInt16(datagram, 0);
                if (version != Packet.VERSION)
                {
                    //兼容IPMsg
                    return new IPMsgUdpPacketResolver(datagram);
                }
            }

            byte type = datagram[2];

            if (type == Packet.PACKTE_TYPE_UDP ||
                type == Packet.PACKTE_TYPE_MULTI_UDP)
            {
                return new DefaultUdpPacketResolver(datagram, securityKey);
            }
            else if (type == Packet.PACKTE_TYPE_TCP)
            {
                return new DefaultTcpPacketResolver(datagram, securityKey);
            }
            else
            {
                throw new Exception("创建包解码器失败，未知包类型。");
            }
        }
    }
}
