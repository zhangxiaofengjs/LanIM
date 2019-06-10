using LanIM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packet
{
    //文件消息附加信息包
    public class UdpPacketFileExtend
    {
        public byte[] EncryptKey { get; set; }
        public LanFile FileInfo { get; set; }

        public UdpPacketFileExtend()
        { 
        }

        public override string ToString()
        {
            string str = string.Format("{{encrypt={0}, file={1}}}",
                    (EncryptKey != null && EncryptKey.Length != 0 ? "Yes" : "No"),
                    LanFile);
            return str;
        }
    }
}
