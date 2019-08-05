using Com.LanIM.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.PacketResolver
{
    //兼容IPMsg 3.42的包格式的解析类
    public class IPMsgUdpPacketResolver : IPacketResolver
    {
        private byte[] _datagram;
        public IPMsgUdpPacketResolver(byte[] datagram)
        {
            this._datagram = datagram;
        }

        public Packet Resolve()
        {
            IPMsgUdpPacket packet = new IPMsgUdpPacket();
            //更改在线状态消息示例：1:1559263918:hyron-<06f73d2bf8f254bd>:DESKTOP-904FV8J:258212096:202?��?[�H����]\0202g\0\nUN:hyron-<06f73d2bf8f254bd>\nHN:DESKTOP-904FV8J\nNN:202张小锋[食事中]\nGN:202g\nVS:00010002:4:99:106"
            string str = Encoding.UTF8.GetString(_datagram);
            int index = str.IndexOf(":");
            if (index < 0)
            {
                return null;
            }
            string strTmp = str.Substring(0, index);
            short ver = 0;
            if (!short.TryParse(strTmp, out ver))
            {
                return null;
            }
            packet.Version = ver;

            int index2 = str.IndexOf(":", index + 1);
            if (index2 < 0)
            {
                return null;
            }
            strTmp = str.Substring(index + 1, index2 - index - 1);
            long id = 0;
            if (!long.TryParse(strTmp, out id))
            {
                return null;
            }
            packet.ID = id;

            index = str.IndexOf(":", index2 + 1);
            if (index < 0)
            {
                return null;
            }
            packet.Sender = str.Substring(index2 + 1, index - index2 - 1);

            index2 = str.IndexOf(":", index + 1);
            if (index2 < 0)
            {
                return null;
            }
            packet.SenderHost = str.Substring(index + 1, index2 - index - 1);

            index = str.IndexOf(":", index2 + 1);
            if (index < 0)
            {
                return null;
            }
            strTmp = str.Substring(index2 + 1, index - index2 - 1);
            ulong cmd = 0;
            if (!ulong.TryParse(strTmp, out cmd))
            {
                return null;
            }
            packet.Command = cmd;

            //查找消息扩展区
            index2 = str.IndexOf("\0", index + 1);
            if (index2 < 0)
            {
                //无扩展区
                packet.Message = str.Substring(index + 1);
            }
            else
            {
                packet.Message = str.Substring(index + 1, index2 - index - 1);
                packet.Extend = str.Substring(index2 + 1);
            }

            if ((packet.Command & IPMsgUdpPacket.IPMSG_CMD_OPT_UTF8) == 0)
            {
                //如果不是UTF8编码的话，前面已经用UTF8编码给转化了，所以要ASCII转换回来
                //TODO 此处稍微和IPMsg有差异，暂未调查清楚编码问题(除了上下线以外一般是UTF8编码的)
                packet.Sender = UTF82ASCII(packet.Sender);
                packet.SenderHost = UTF82ASCII(packet.SenderHost);
                packet.Message = UTF82ASCII(packet.Message);
            }

            //处理扩展区消息
            if (!string.IsNullOrEmpty(packet.ExtendMessage))
            {
                index = packet.ExtendMessage.IndexOf("\0");
                string strExt = null;
                string strExt2 = null;
                if (index < 0)
                {
                    strExt = packet.ExtendMessage;
                }
                else
                {
                    strExt = packet.ExtendMessage.Substring(0, index);
                    strExt2 = packet.ExtendMessage.Substring(index + 1);
                }

                if (strExt[0] != '\n')
                {
                    if ((packet.Command & IPMsgUdpPacket.IPMSG_CMD_OPT_UTF8) == 0)
                    {
                        strExt = UTF82ASCII(strExt);
                    }
                    packet.Extend = strExt;
                }
                else if (string.IsNullOrEmpty(strExt2))
                {
                    strExt2 = strExt;
                }

                if (!string.IsNullOrEmpty(strExt2) &&
                    strExt2[0] == '\n' &&
                    (packet.Command & IPMsgUdpPacket.IPMSG_CAPUTF8OPT) != 0)
                {
                    string[] fields = strExt2.Split('\n');
                    foreach (string field in fields)
                    {
                        if (field.StartsWith("UN"))
                        {
                            packet.Sender = field.Substring(3);
                        }
                        else if (field.StartsWith("HN"))
                        {
                            packet.SenderHost = field.Substring(3);
                        }
                        else if (field.StartsWith("NN"))
                        {
                            switch (packet.CommandMode)
                            {
                                case IPMsgUdpPacket.IPMSG_CMD_BR_ENTRY:
                                case IPMsgUdpPacket.IPMSG_CMD_BR_ABSENCE:
                                    packet.Message = field.Substring(3);
                                    break;
                            }
                        }
                        else if (field.StartsWith("GN"))
                        {
                            switch (packet.CommandMode)
                            {
                                case IPMsgUdpPacket.IPMSG_CMD_BR_ENTRY:
                                case IPMsgUdpPacket.IPMSG_CMD_BR_ABSENCE:
                                    packet.Extend = field.Substring(3);
                                    break;
                            }
                        }
                        else if (field.StartsWith("VS"))
                        {
                            //IPMsg 4.x的消息，暂时未对应
                        }

                    }
                }
            }

            return packet;
        }

        private static string UTF82ASCII(string str)
        {
            byte[] buff = Encoding.UTF8.GetBytes(str);
            return Encoding.ASCII.GetString(buff);
        }
    }
}
