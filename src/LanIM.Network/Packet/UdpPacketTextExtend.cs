using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packet
{
    //文本消息附加信息包
    public class UdpPacketTextExtend
    {
        public byte[] EncryptKey { get; set; }
        public string Text { get; set; }

        public UdpPacketTextExtend()
        {
        }

        public override string ToString()
        {
            string str = string.Format("{{encrypt={0}, message={1}}}",
                    (EncryptKey != null && EncryptKey.Length != 0 ? "Yes" : "No"),
                    Text);
            return str;
        }
    }
}
