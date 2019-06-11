using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packets
{
    //图像消息附加信息包
    public class UdpPacketImageExtend
    {
        public byte[] EncryptKey { get; set; }
        public Image Image { get; set; }

        public UdpPacketImageExtend()
        {
        }

        public override string ToString()
        {
            string str = string.Format("{{encrypt={0}, image={1}}}",
                    (EncryptKey != null && EncryptKey.Length != 0 ? "Yes" : "No"),
                    Image);
            return str;
        }
    }
}
