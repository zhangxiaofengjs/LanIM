using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packets;
using LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.PacketsResolver
{
    public class DefaultUdpPacketResolver : IPacketResolver
    {
        private byte[] _datagram;
        private byte[] _securityKey;

        public DefaultUdpPacketResolver(byte[] datagram, byte[] securityKey)
        {
            this._datagram = datagram;
            this._securityKey = securityKey;
        }

        public Packet Resolve()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(this._datagram))
                {
                    BinaryReader rdr = new BinaryReader(ms, Packet.ENCODING);

                    UdpPacket packet = new UdpPacket();
                    packet.Version = rdr.ReadInt16();
                    rdr.ReadInt16(); //skip packet.Type;
                    packet.ID = rdr.ReadInt64();
                    packet.Command = rdr.ReadUInt64();
                    packet.MAC = rdr.ReadString();

                    switch (packet.CMD)
                    {
                        case UdpPacket.CMD_ENTRY:
                            packet.Extend = ResolveEntryExtend(rdr);
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
                            packet.Extend = ResolveEntryExtend(rdr);
                            break;
                        default:
                            break;
                    }

                    return packet;
                }
            }
            catch (Exception e)
            {
                LoggerFactory.Instance().Error("[resolve error]", e);
            }
            return null;
        }

        private static UdpPacketEntryExtend ResolveEntryExtend(BinaryReader rdr)
        {
            UdpPacketEntryExtend extend = new UdpPacketEntryExtend();
            int len = rdr.ReadInt32();
            extend.PublicKey = rdr.ReadBytes(len);

            extend.NickName = rdr.ReadString();
            extend.HideState = rdr.ReadBoolean();
            return extend;
        }

        private static UdpPacketResponseExtend ResolveResponseExtend(BinaryReader rdr)
        {
            UdpPacketResponseExtend extend = new UdpPacketResponseExtend();
            extend.ID = rdr.ReadInt64();

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
