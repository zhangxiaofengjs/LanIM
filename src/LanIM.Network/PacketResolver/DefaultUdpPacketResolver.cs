using Com.LanIM.Common;
using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.PacketResolver
{
    public class DefaultUdpPacketResolver : IPacketResolver
    {
        private readonly byte[] _datagram;
        private readonly byte[] _securityKey;

        public DefaultUdpPacketResolver(byte[] datagram, byte[] securityKey)
        {
            this._datagram = datagram;
            this._securityKey = securityKey;
        }

        public Packet Resolve()
        {
            using (MemoryStream ms = new MemoryStream(this._datagram))
            {
                BinaryReader rdr = new BinaryReader(ms, Packet.ENCODING);

                short version = rdr.ReadInt16();
                byte type = rdr.ReadByte(); //skip packet.Type;
                long id = rdr.ReadInt64();

                UdpPacket packet = null;
                if (type == Packet.PACKTE_TYPE_MULTI_UDP)
                {
                    //分别看下是否复合UDP分包
                    MultiUdpPacket packetm = new MultiUdpPacket();
                    packetm.Version = version;
                    packetm.ID = id;
                    packetm.ParentID = rdr.ReadInt64();
                    packetm.TotalLength = rdr.ReadInt32();
                    packetm.Position = rdr.ReadInt32();
                    packetm.Length = rdr.ReadInt32();
                    packetm.FragmentBuff  = rdr.ReadBytes(packetm.Length);

                    packet = packetm;
                }
                else
                {
                    packet = new UdpPacket();
                    packet.Version = version;
                    packet.ID = id;
                    packet.Command = rdr.ReadUInt64();
                    packet.FromMAC = rdr.ReadString();
                    packet.ToMAC = rdr.ReadString();

                    switch (packet.CMD)
                    {
                        case UdpPacket.CMD_ENTRY:
                            packet.Extend = ResolveEntryExtend(rdr, packet.Command);
                            break;
                        case UdpPacket.CMD_SEND_TEXT:
                            packet.Extend = ResolveTextExtend(rdr, this._securityKey);
                            break;
                        case UdpPacket.CMD_SEND_IMAGE:
                            packet.Extend = ResolveImageExtend(rdr, this._securityKey);
                            break;
                        case UdpPacket.CMD_SEND_FILE_REQUEST:
                            packet.Extend = ResolveSendFileRequestExtend(rdr, this._securityKey);
                            break;
                        case UdpPacket.CMD_RESPONSE:
                            packet.Extend = ResolveResponseExtend(rdr);
                            break;
                        case UdpPacket.CMD_STATE:
                            packet.Extend = ResolveEntryExtend(rdr, packet.Command);
                            break;
                        case UdpPacket.CMD_RETRANSMIT:
                            packet.Extend = ResolveRetransmitExtend(rdr);
                            break;
                        case UdpPacket.CMD_USER_LIST:
                            packet.Extend = ResolveUserListExtend(rdr);
                            break;
                        default:
                            break;
                    }
                }

                return packet;
            }
        }

        private object ResolveUserListExtend(BinaryReader rdr)
        {
            UdpPacketUserListExtend extend = new UdpPacketUserListExtend();

            int count = rdr.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                User u = new User();
                u.Address = IPAddress.Parse(rdr.ReadString());
                u.Port = rdr.ReadInt32();
                u.MAC = rdr.ReadString();
                u.Status = (UserStatus)rdr.ReadInt32();

                int bc = rdr.ReadInt32();
                u.SecurityKeys.Public = rdr.ReadBytes(bc);

                extend.AddUser(u);
            }

            return extend;
        }

        private static UdpPacketRetransExtend ResolveRetransmitExtend(BinaryReader rdr)
        {
            long pktId = rdr.ReadInt64();

            string strIp = rdr.ReadString();
            int port = rdr.ReadInt32();
            int len = rdr.ReadInt32();
            byte[] buf = rdr.ReadBytes(len);

            UdpPacketRetransExtend extend = new UdpPacketRetransExtend(buf);
            extend.PacketID = pktId;
            extend.Address = IPAddress.Parse(strIp);
            extend.Port = port;
            return extend;
        }

        private static UdpPacketUserStateExtend ResolveEntryExtend(BinaryReader rdr, ulong command)
        {
            UdpPacketUserStateExtend extend = new UdpPacketUserStateExtend();
            extend.UpdateState = (UpdateState)rdr.ReadInt32();

            User user = new User();
            extend.User = user;

            if ((extend.UpdateState & UpdateState.PublicKey) != 0)
            {
                int len = rdr.ReadInt32();
                user.SecurityKeys.Public = rdr.ReadBytes(len);
            }
            if ((extend.UpdateState & UpdateState.NickName) != 0)
            {
                user.NickName = rdr.ReadString();
            }
            if ((extend.UpdateState & UpdateState.Status) != 0)
            {
                user.Status = (UserStatus)rdr.ReadInt32();
            }
            if ((extend.UpdateState & UpdateState.Photo) != 0)
            {
                int len = rdr.ReadInt32();
                if (len != 0)
                {
                    byte[] buf = rdr.ReadBytes(len);
                    using (MemoryStream ms = new MemoryStream(buf))
                    {
                        user.ProfilePhoto = Image.FromStream(ms);
                    }
                }
            }
            if ((extend.UpdateState & UpdateState.IP) != 0)
            {
                user.Address = IPAddress.Parse(rdr.ReadString());
            }
            if ((extend.UpdateState & UpdateState.Port) != 0)
            {
                user.Port = rdr.ReadInt32();
            }
            return extend;
        }

        private static UdpPacketResponseExtend ResolveResponseExtend(BinaryReader rdr)
        {
            UdpPacketResponseExtend extend = new UdpPacketResponseExtend
            {
                ID = rdr.ReadInt64()
            };

            return extend;
        }

        private static UdpPacketTextExtend ResolveTextExtend(BinaryReader rdr, byte[] priKey)
        {
            UdpPacketTextExtend extend = new UdpPacketTextExtend();
            int len = rdr.ReadInt32();
            byte[] buf = rdr.ReadBytes(len);

            byte[] deBuf = SecurityFactory.Decrypt(buf, priKey);
            extend.Text = Packet.ENCODING.GetString(deBuf);

            return extend;
        }

        private static UdpPacketImageExtend ResolveImageExtend(BinaryReader rdr, byte[] priKey)
        {
            UdpPacketImageExtend extend = new UdpPacketImageExtend();
            string fileName = rdr.ReadString();
            int len = rdr.ReadInt32();
            byte[] buf = rdr.ReadBytes(len);

            byte[] deBuf = SecurityFactory.Decrypt(buf, priKey);

            using (MemoryStream ms = new MemoryStream(deBuf))
            {
                Image image = Image.FromStream(ms);
                extend.Image = image;
                extend.FileName = fileName;
            }
            return extend;
        }

        private static UdpPacketSendFileRequestExtend ResolveSendFileRequestExtend(BinaryReader rdr, byte[] priKey)
        {
            int len = rdr.ReadInt32();
            byte[] buf = rdr.ReadBytes(len);

            byte[] deBuf = SecurityFactory.Decrypt(buf, priKey);

            using (MemoryStream ms = new MemoryStream(deBuf))
            {
                using (BinaryReader bw = new BinaryReader(ms))
                {
                    LanFile file = new LanFile();
                    file.Name = bw.ReadString();
                    file.Length = bw.ReadInt32();
                    file.IsFolder = bw.ReadBoolean();

                    bw.Close();
                    ms.Close();

                    UdpPacketSendFileRequestExtend extend = new UdpPacketSendFileRequestExtend();
                    extend.File = file;

                    return extend;
                }
            }
        }
    }
}
