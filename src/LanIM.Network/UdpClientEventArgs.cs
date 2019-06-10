using Com.LanIM.Network.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network
{
    public class UdpClientEventArgs
    {
        private UdpPacket _packet;
        public UdpPacket Packet
        {
            get { return _packet; }
        }

        public UdpClientEventArgs(UdpPacket packet)
        {
            _packet = packet;
        }
    }

    public class UdpClientSendEventArgs  : UdpClientEventArgs
    {
        private bool _success = false;
        public bool Success
        {
            get
            {
                return this._success;
            }
        }

        public UdpClientSendEventArgs(UdpPacket packet, bool success)
       : base(packet)
        {
            this._success = success;
        }
    }

    public class UdpClientReceiveEventArgs: UdpClientEventArgs
    {
        public UdpClientReceiveEventArgs(UdpPacket packet)
        : base (packet)
        {
        }
    }
}
