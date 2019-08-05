using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packets;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.PacketEncoder
{
    public class DefaultTcpPacketEncoder : IPacketEncoder
    {
        private TcpPacket _packet;

        public DefaultTcpPacketEncoder(TcpPacket packet)
        {
            this._packet = packet;
        }

        public EncodeResult Encode(object args)
        {
            using (MemoryStream ms = new MemoryStream(8042))
            {
                BinaryWriter wtr = new BinaryWriter(ms, Packet.ENCODING);

                //包头
                wtr.Write(_packet.Version);
                wtr.Write(_packet.Type);
                wtr.Write(_packet.Command);

                if (_packet.Extend is TcpPacketRequestFileTransportExtend)
                {
                    Encode(wtr, _packet.Extend as TcpPacketRequestFileTransportExtend);
                }

                return new EncodeResult(ms.ToArray());
            }
        }

        private static void Encode(BinaryWriter wtr, TcpPacketRequestFileTransportExtend extend)
        {
            byte[] buf = BitConverter.GetBytes(extend.FileID);
            byte[] enBuf = SecurityFactory.Encrypt(buf, extend.EncryptKey);

            wtr.Write(enBuf.Length);
            wtr.Write(enBuf);
        }
    }
}
