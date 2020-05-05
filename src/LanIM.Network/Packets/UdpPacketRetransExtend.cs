using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packets
{
    public class UdpPacketRetransExtend
    {
        public byte[] PacketBuf { get; set; }
        public int Length
        {
            get
            {
                if (PacketBuf == null)
                {
                    return 0;
                }
                else
                {
                    return PacketBuf.Length;
                }
            }
        }

        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public long PacketID { get; set; }

        public UdpPacketRetransExtend(byte[] buf)
        {
            this.PacketBuf = buf;
        }

        public override string ToString()
        {
            string str = string.Format("{{toAddr={0}, toPort={1}, pktId={3}, buf={2}bytes}}",
                    Address, Port, Length, PacketID);
            return str;
        }
    }
}
