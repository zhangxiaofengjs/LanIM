using Com.LanIM.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.PacketsEncoder
{
    public class PacketEncoderFactory
    {
        public static IPacketEncoder CreateEncoder(Packet packet)
        {
            if (packet is IPMsgUdpPacket)
            {
                return new IPMsgUdpPacketEncoder(packet as IPMsgUdpPacket);
            }
            else if (packet is UdpPacket)
            {
                return new DefaulUdpPacketEncoder(packet as UdpPacket);
            }
            else if (packet is TcpPacket)
            {
                return new DefaultTcpPacketEncoder(packet as TcpPacket);
            }
            else
            {
                throw new Exception("未知的包" + packet.ToString());
            }
        }
    }
}
