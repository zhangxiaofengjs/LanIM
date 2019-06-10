using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packet
{
    //包格式：版本（2字节)，包编号(8字节），命令编号（8字节），发送主机网卡MAC（12字节），附加信息区域（N字节）
    public class UdpPacket
    {
        //最大消息发送缓存
        public const int MAX_MESSAGE_BYTE_LEN = 32768;
        //版本
        public const short VERSION = 0x10;
        public const int HEAD_LEN = 30;
        //编码
        public static Encoding ENCODING = Encoding.UTF8;
        //命令
        public const ulong CMD_MASK = 0x000000FF;  //命令MASK
        public const ulong CMD_NONE = 0x00000000; //无操作命令
        public const ulong CMD_ENTRY = 0x00000001; //上线
        public const ulong CMD_EXIT = 0x00000002; //下线
        public const ulong CMD_STATE = 0x00000003; //用户各种信息状态
        public const ulong CMD_SEND_TEXT = 0x00000004; //发送文本消息
        public const ulong CMD_SEND_IMAGE = 0x00000005; //发送图像消息
        public const ulong CMD_SEND_FILE_REQUEST = 0x00000006; //发送文件传送要求消息
        public const ulong CMD_RESPONSE = 0x00000006; //收到消息回应

        public const ulong CMD_OPTION_NEED_RESPONSE = 0x00000100; //是否需要回应消息

        public IPAddress Remote { get; set; }
        public int Port { get; set; }

        public short Version { get; set; }
        public long ID { get; set; }
        public ulong Command { get; set; }
        public ulong CMD { get { return CMD_MASK & Command; } }
        public string MAC { get; set; }
        public object Extend { get; set; }

        public UdpPacket()
        {
            this.Version = VERSION;
        }

        //是否检测发包失败
        //需要接受方发送一个回包,并且发送方收到回包后告知发送成功TUdpClient.NotifySendPackageSuccess，
        //否则TUdpClient.SendResponseCheckDelay后检测作为发送失败
        public bool CheckSendResponse
        {
            get
            {
                return (Command & CMD_OPTION_NEED_RESPONSE) != 0;
            }
        }

        internal void GenerateID()
        {
            this.ID = DateTime.Now.Ticks;
        }

        public override string ToString()
        {
            string str = string.Format("{{remote={0}, port={1}, ver={2}, id={3}, command={4}, mac={5}, extend={6}}}",
                    Remote,
                    Port,
                    Version,
                    ID,
                    Command,
                    MAC,
                    Extend);
            return str;
        }
    }
}
