using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packet
{
    //兼容IPMsg 3.42的包格式
    //版本：包编号：发送者名字：发送主机：命令编号：附加信息区域
    public class IPMsgUdpPacket : UdpPacket
    {
        /* IPMsg版本号3.42 */
        public const int IPMSG_VERSION = 0x0001;
        public const int IPMSG_DEFAULT_PORT = 0x0979;

        /* 命令模式 */
        public const ulong IPMSG_CMD_MASK = 0x000000ffUL;  //命令MASK

        public const ulong IPMSG_CMD_NOOPERATION = 0x00000000UL; //无操作命令
        public const ulong IPMSG_CMD_BR_ENTRY = 0x00000001UL;  //上线
        public const ulong IPMSG_CMD_BR_EXIT = 0x00000002UL;  //下线
        public const ulong IPMSG_CMD_ANSENTRY = 0x00000003UL;
        public const ulong IPMSG_CMD_BR_ABSENCE = 0x00000004UL; //变更在线状态

        public const ulong IPMSG_BR_ISGETLIST = 0x00000010UL;
        public const ulong IPMSG_OKGETLIST = 0x00000011UL;
        public const ulong IPMSG_GETLIST = 0x00000012UL;
        public const ulong IPMSG_ANSLIST = 0x00000013UL;
        public const ulong IPMSG_FILE_MTIME = 0x00000014UL;
        public const ulong IPMSG_FILE_CREATETIME = 0x00000016UL;
        public const ulong IPMSG_BR_ISGETLIST2 = 0x00000018UL;

        public const ulong IPMSG_SENDMSG = 0x00000020UL;
        public const ulong IPMSG_RECVMSG = 0x00000021UL;
        public const ulong IPMSG_READMSG = 0x00000030UL;
        public const ulong IPMSG_DELMSG = 0x00000031UL;

        /* 命令选项 */
        public const ulong IPMSG_CMD_OPT_ABSENCE = 0x00000100UL;//非在线状态：外出，吃饭中...
        public const ulong IPMSG_SERVEROPT = 0x00000200UL;
        public const ulong IPMSG_DIALUPOPT = 0x00010000UL;
        public const ulong IPMSG_CMD_OPT_FILEATTACH = 0x00200000UL; //发送文件 ??
        public const ulong IPMSG_ENCRYPTOPT = 0x00400000UL; //加密发送 ??
        public const ulong IPMSG_CMD_OPT_UTF8 = 0x00800000UL;
        public const ulong IPMSG_CAPUTF8OPT = 0x01000000UL;//加密区进行UTF8编码 ??
        public const ulong IPMSG_ENCEXTMSGOPT = 0x04000000UL; //加密扩展消息 ??
        public const ulong IPMSG_CLIPBOARDOPT = 0x08000000UL; //剪贴板？？

        /* file types for fileattach command */
        public const ulong IPMSG_FILE_REGULAR = 0x00000001UL;
        public const ulong IPMSG_FILE_DIR = 0x00000002UL;
        public const ulong IPMSG_LISTGET_TIMER = 0x0104;
        public const ulong IPMSG_LISTGETRETRY_TIMER = 0x0105;

        //最大消息发送缓存
        public const int IPMSG_MAX_MESSAGE_BYTE_LEN = 32768;

        private string _sender;
        private string _senderHost;
        private string _message;
        private string _extend;

        public IPMsgUdpPacket()
        {
            this.Version = IPMSG_VERSION;
        }

        private string UTF82ASCII(string str)
        {
            byte[] buff = Encoding.UTF8.GetBytes(str);
            return Encoding.ASCII.GetString(buff);
        }
        public IPMsgUdpPacket(string senderName, string senderHostName, ulong command, string message)
        {
            this._sender = senderName;
            this._senderHost = senderHostName;
            this.Command = command;
            this._message = message;
        }

        public string Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                _sender = value;
            }
        }

        public string SenderHost
        {
            get
            {
                return _senderHost;
            }
            set
            {
                _senderHost = value;
            }
        }

        public ulong CommandMode
        {
            get
            {
                return (Command & IPMSG_CMD_MASK);
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        public string ExtendMessage
        {
            get
            {
                return _extend;
            }
            set
            {
                _extend = value;
            }
        }

        public override string ToString()
        {
            string str = string.Format("ver={0}, id={1}, sender={2}, senderHost={3}, command={4}, message={5}, extend={6}",
                    Version,
                    ID,
                    _sender,
                    _senderHost,
                    Command,
                    _message,
                    _extend);
            return str;
        }
    }
}
