using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packets
{
    public class TcpPacket : Packet
    {
        //命令
        public const ulong CMD_MASK = 0x000000FF;  //命令MASK
        public const ulong CMD_NONE = 0x00000000; //无操作命令
        public const ulong CMD_REQUEST_FILE_TRANSPORT = 0x00000001; //要求传输文件

        public override short Type
        {
            get
            {
                return Packet.PACKTE_TYPE_TCP;
            }
        }
        public ulong Command { get; set; }
        public ulong CMD { get { return CMD_MASK & Command; } }
    }
}
