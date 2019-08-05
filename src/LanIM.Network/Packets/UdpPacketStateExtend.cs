using Com.LanIM.Common;
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
    //上线附加信息包
    public class UdpPacketStateExtend
    {
        public byte[] PublicKey { get; set; }
        public string NickName { get; set; }
        public Image ProfilePhoto { get; set; }
        public UserStatus Status { get; set; }

        public UdpPacketStateExtend()
        {
        }

        public override string ToString()
        {
            string str = string.Format("{{public key={0}, nickname={1}, status={2}, photo={3}}}",
                    PublicKey ==null? "" : Convert.ToBase64String(PublicKey),
                    NickName,
                    Status,
                    ProfilePhoto == null ? "null" : ProfilePhoto.ToString());
            return str;
        }
    }
}
