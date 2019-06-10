using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packet
{
    //回应消息附加信息包
    public class UdpPacketResponseExtend
    {
        public long ID { get; set; } //回应原包ID

        public UdpPacketResponseExtend()
        {
        }

        public override string ToString()
        {
            string str = string.Format("{{ID={0}}}",
                    ID);
            return str;
        }
    }
}
