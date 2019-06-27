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
    public class UdpPacketEntryExtend
    {
        public byte[] PublicKey { get; set; }
        public string NickName { get; set; }
        public Image ProfilePhoto { get; set; }
        public bool HideState { get; set; }//隐藏在线

        public UdpPacketEntryExtend()
        {
        }

        public override string ToString()
        {
            string str = string.Format("{{public key={0}, nickname={1}, hidestate={2}}}",
                    Convert.ToBase64String(PublicKey),
                    NickName,
                    HideState);
            return str;
        }
    }
}
