using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packets;
using LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.PacketsEncoder
{
    public class DefaulUdpPacketEncoder : IPacketEncoder
    {
        private UdpPacket _packet;

        public DefaulUdpPacketEncoder(UdpPacket packet)
        {
            this._packet = packet;
        }

        public byte[] Encode()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(8042))
                {
                    BinaryWriter wtr = new BinaryWriter(ms, UdpPacket.ENCODING);

                    //包头
                    wtr.Write(_packet.Version);
                    wtr.Write(_packet.Type);
                    wtr.Write(_packet.ID);
                    wtr.Write(_packet.Command);
                    wtr.Write(_packet.MAC);

                    switch (_packet.CMD)
                    {
                        case UdpPacket.CMD_ENTRY:
                            EncodeEntryExtend(wtr, _packet.Extend);
                            break;
                        case UdpPacket.CMD_SEND_TEXT:
                            EncodeTextExtend(wtr, _packet.Extend);
                            break;
                        case UdpPacket.CMD_SEND_IMAGE:
                            EncodeImageExtend(wtr, _packet.Extend);
                            break;
                        case UdpPacket.CMD_SEND_FILE_REQUEST:
                            EncodeSendFileRequestExtend(wtr, _packet.Extend);
                            break;
                        case UdpPacket.CMD_RESPONSE:
                            EncodeResponseExtend(wtr, _packet.Extend);
                            break;
                        case UdpPacket.CMD_STATE:
                            EncodeEntryExtend(wtr, _packet.Extend);
                            break;
                        default:
                            break;
                    }

                    return ms.ToArray();
                }
            }
            catch (Exception e)
            {
                LoggerFactory.Instance().Error("[encoder error]", e);
            }
            return null;
        }

        private static void EncodeEntryExtend(BinaryWriter wtr, object extendObj)
        {
            UdpPacketEntryExtend extend = extendObj as UdpPacketEntryExtend;
            if (extend == null)
            {
                throw new Exception("[EncodeEntryExtend]未想定附加包");
            }
            byte[] keyBuf = extend.PublicKey;
            wtr.Write(keyBuf.Length);
            wtr.Write(keyBuf);

            wtr.Write(extend.NickName);
            wtr.Write(extend.HideState);
        }

        private static void EncodeResponseExtend(BinaryWriter wtr, object extendObj)
        {
            UdpPacketResponseExtend extend = extendObj as UdpPacketResponseExtend;
            if (extend == null)
            {
                throw new Exception("[EncodeResponseExtend]未想定附加包");
            }
            wtr.Write(extend.ID);
        }

        private static void EncodeTextExtend(BinaryWriter wtr, object extendObj)
        {
            UdpPacketTextExtend extend = extendObj as UdpPacketTextExtend;
            if (extend == null)
            {
                throw new Exception("[EncodeTextExtend]未想定附加包");
            }

            byte[] buf = UdpPacket.ENCODING.GetBytes(extend.Text);
            byte[] enBuf = SecurityFactory.Encrypt(buf, extend.EncryptKey);
            wtr.Write(enBuf.Length);
            wtr.Write(enBuf);
        }

        private static void EncodeImageExtend(BinaryWriter wtr, object extendObj)
        {
            UdpPacketImageExtend extend = extendObj as UdpPacketImageExtend;
            if (extend == null)
            {
                throw new Exception("[EncodeImageExtend]未想定附加包");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                extend.Image.Save(ms, ImageFormat.Png);
                byte[] buf = ms.ToArray();
                byte[] enBuf = SecurityFactory.Encrypt(buf, extend.EncryptKey);

                wtr.Write(enBuf.Length);
                wtr.Write(enBuf);
            }
        }

        private static void EncodeSendFileRequestExtend(BinaryWriter wtr, object extendObj)
        {
            UdpPacketSendFileRequestExtend extend = extendObj as UdpPacketSendFileRequestExtend;
            if (extend == null)
            {
                throw new Exception("[EncodeImageExtend]未想定附加包");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    LanFile file = extend.File;

                    bw.Write(file.Name);
                    bw.Write(file.Length);
                    bw.Write(file.IsFolder);

                    bw.Close();
                    ms.Close();

                    byte[] buf = ms.ToArray();
                    byte[] enBuf = SecurityFactory.Encrypt(buf, extend.EncryptKey);

                    wtr.Write(enBuf.Length);
                    wtr.Write(enBuf);
                }
            }
        }
    }
}
