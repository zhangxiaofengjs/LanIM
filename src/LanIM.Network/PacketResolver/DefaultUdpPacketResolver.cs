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
                        default:
                            break;
                    }
                }

                return packet;
            }
        }

        private static UdpPacketStateExtend ResolveEntryExtend(BinaryReader rdr, ulong command)
        {
            UdpPacketStateExtend extend = new UdpPacketStateExtend();

            if ((command & UdpPacket.CMD_OPTION_STATE_PUBKEY) != 0)
            {
                int len = rdr.ReadInt32();
                extend.PublicKey = rdr.ReadBytes(len);
            }
            if ((command & UdpPacket.CMD_OPTION_STATE_NICKNAME) != 0)
            {
                extend.NickName = rdr.ReadString();
            }
            if ((command & UdpPacket.CMD_OPTION_STATE_STATUS) != 0)
            {
                extend.Status = (UserStatus)rdr.ReadInt32();
            }
            if ((command & UdpPacket.CMD_OPTION_STATE_PROFILE_PHOTO) != 0)
            {
                int len = rdr.ReadInt32();
                if (len != 0)
                {
                    byte[] buf = rdr.ReadBytes(len);
                    using (MemoryStream ms = new MemoryStream(buf))
                    {
                        extend.ProfilePhoto = Image.FromStream(ms);
                    }
                }
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
            int len = rdr.ReadInt32();
            byte[] buf = rdr.ReadBytes(len);

            byte[] deBuf = SecurityFactory.Decrypt(buf, priKey);

            using (MemoryStream ms = new MemoryStream(deBuf))
            {
                Image image = Image.FromStream(ms);
                extend.Image = image;
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
