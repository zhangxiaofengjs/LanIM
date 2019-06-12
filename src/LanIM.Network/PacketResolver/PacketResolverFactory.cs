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

namespace Com.LanIM.Network.PacketsResolver
{
    public class PacketResolverFactory
    {
        public static IPacketResolver CreateResolver(byte[] datagram, int startIndex, int length, byte[] securityKey)
        {
            if (datagram[0] == 49)
            {
                short version = BitConverter.ToInt16(datagram, 0);
                if (version != Packet.VERSION)
                {
                    //兼容IPMsg
                    return new IPMsgUdpPacketResolver(datagram);
                }
            }

            short type = BitConverter.ToInt16(datagram, 2);

            if(type == Packet.PACKTE_TYPE_UDP)
            {
                return new DefaultUdpPacketResolver(datagram, securityKey);
            }
            else if (type == Packet.PACKTE_TYPE_TCP)
            {
                return new DefaultTcpPacketResolver(datagram, securityKey);
            }
            else
            {
                    LoggerFactory.Error("创建解码器失败，未知包类型");
                    return null;
            }
        }
    }
}
