using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packets
{
    public class TcpPacketRequestFileTransportExtend : TcpPacket
    {
        public long FileID { get; set; }
        public byte[] EncryptKey { get; set; }

        public TcpPacketRequestFileTransportExtend()
        {
        }

        public override string ToString()
        {
            string str = string.Format("{{encrypt={0}, FileID={1}}}",
                    (EncryptKey != null && EncryptKey.Length != 0 ? "Yes" : "No"),
                    FileID);
            return str;
        }
    }
}
