using Com.LanIM.Network.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.PacketEncoder
{
    public class UdpPacketEncoderFactory
    {
        public static IUdpPacketEncoder CreateEncoder(UdpPacket packet)
        {
            if (packet is IPMsgUdpPacket)
            {
                return new IPMsgUdpPacketEncoder(packet as IPMsgUdpPacket);
            }

            return new DefaulUdpPacketEncoder(packet);
        }
    }
}
