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
    //兼容IPMsg 3.42的包格式的解析类
    public class IPMsgUdpPacketEncoder : IUdpPacketEncoder
    {

        private IPMsgUdpPacket _packet;

        public IPMsgUdpPacketEncoder(IPMsgUdpPacket packet)
        {
            this._packet = packet;
        }

        public byte[] Encode()
        {
            string str = string.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                   _packet.Version,
                   _packet.ID,
                   _packet.Sender,
                   _packet.SenderHost,
                   _packet.Command,
                   _packet.Message);

            return Encoding.UTF8.GetBytes(str);
        }

        private static string UTF82ASCII(string str)
        {
            byte[] buff = Encoding.UTF8.GetBytes(str);
            return Encoding.ASCII.GetString(buff);
        }
    }
}
