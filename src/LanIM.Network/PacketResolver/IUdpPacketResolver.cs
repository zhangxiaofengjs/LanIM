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
    public interface IUdpPacketResolver
    {
        UdpPacket Resolve();
    }
}
