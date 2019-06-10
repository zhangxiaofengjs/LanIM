using Com.LanIM.Network.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.PacketResolver
{
    public class UdpPacketResolverFactory
    {
        public static IUdpPacketResolver CreateResolver(byte[] datagram, UdpClientEx udpClient)
        {
            if (datagram[0] == 49)
            {
                short version = BitConverter.ToInt16(datagram, 0);
                if (version != UdpPacket.VERSION)
                {
                    //兼容IPMsg
                    return new IPMsgUdpPacketResolver(datagram);
                }
            }

            return new DefaulUdpPacketResolver(datagram, udpClient);
        }
    }
}
