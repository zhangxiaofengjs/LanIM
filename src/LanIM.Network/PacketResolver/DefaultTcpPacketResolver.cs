﻿using Com.LanIM.Common.Logger;
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
    public class DefaultTcpPacketResolver : IPacketResolver
    {
        private byte[] _datagram;
        private byte[] _securityKey;

        public DefaultTcpPacketResolver(byte[] datagram, byte[] securityKey)
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

                    TcpPacket packet = new TcpPacket();
                    packet.Version = rdr.ReadInt16();
                    rdr.ReadInt16();//skip packet.Type
                    packet.Command = rdr.ReadUInt64();

                    switch (packet.CMD)
                    {
                        case TcpPacket.CMD_REQUEST_FILE_TRANSPORT:
                            packet.Extend = ResolveRequestFileTransportExtend(rdr, _securityKey);
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

        private static TcpPacketRequestFileTransportExtend ResolveRequestFileTransportExtend(BinaryReader rdr, byte[] priKey)
        {
            TcpPacketRequestFileTransportExtend extend = new TcpPacketRequestFileTransportExtend();
            int len = rdr.ReadInt32();
            byte[] buf = rdr.ReadBytes(len);

            byte[] deBuf = SecurityFactory.Decrypt(buf, priKey);
            extend.FileID = BitConverter.ToInt64(deBuf, 0);

            return extend;
        }
    }
}