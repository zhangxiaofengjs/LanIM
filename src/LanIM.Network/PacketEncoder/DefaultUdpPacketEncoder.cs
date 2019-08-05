using Com.LanIM.Common;
using Com.LanIM.Common.Logger;
using Com.LanIM.Common.Security;
using Com.LanIM.Network.Packets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.PacketEncoder
{
    public class DefaulUdpPacketEncoder : IPacketEncoder
    {
        private UdpPacket _packet;

        public DefaulUdpPacketEncoder(UdpPacket packet)
        {
            this._packet = packet;
        }

        public EncodeResult Encode(object args)
        {
            using (MemoryStream ms = new MemoryStream(8042))
            {
                BinaryWriter wtr = new BinaryWriter(ms, UdpPacket.ENCODING);

                EncodeResult result = new EncodeResult();

                if (_packet is MultiUdpPacket)
                {
                    MultiUdpPacket p = _packet as MultiUdpPacket;
                    long parentId = p.ID;

                    int pos = 0;
                    int len = (int)args;
                    
                    while (pos < p.FragmentBuff.Length)
                    {
                        int restLen = p.FragmentBuff.Length - pos;
                        if (restLen < len)
                        {
                            len = restLen;
                        }

                        //包头
                        p.GenerateID();

                        wtr.Write(p.Version);
                        wtr.Write(p.Type);
                        wtr.Write(p.ID);
                        wtr.Write(parentId);
                        wtr.Write(p.TotalLength);

                        wtr.Write(pos);
                        wtr.Write(len);
                        wtr.Write(p.FragmentBuff, pos, len);

                        byte[] buff = ms.ToArray();
                        result.AddFragment(buff);

                        wtr.Seek(0, SeekOrigin.Begin);
                        ms.Position = 0;
                        ms.SetLength(0);

                        pos += len;
                    }
                }
                else
                {
                    //包头
                    wtr.Write(_packet.Version);
                    wtr.Write(_packet.Type);
                    wtr.Write(_packet.ID);
                    wtr.Write(_packet.Command);
                    wtr.Write(_packet.FromMAC);
                    wtr.Write(_packet.ToMAC);

                    switch (_packet.CMD)
                    {
                        case UdpPacket.CMD_ENTRY:
                            EncodeEntryExtend(wtr, _packet);
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
                            EncodeEntryExtend(wtr, _packet);
                            break;
                        default:
                            break;
                    }

                    byte[] buff = ms.ToArray();
                    result.AddFragment(buff);
                }

                wtr.Close();
                ms.Close();

                return result;
            }
        }

        private static void EncodeEntryExtend(BinaryWriter wtr, UdpPacket packet)
        {
            UdpPacketStateExtend extend = packet.Extend as UdpPacketStateExtend;
            if (extend == null)
            {
                throw new Exception("[EncodeEntryExtend]未想定附加包");
            }

            if ((packet.Command & UdpPacket.CMD_OPTION_STATE_PUBKEY) != 0)
            {
                byte[] keyBuf = extend.PublicKey;
                wtr.Write(keyBuf.Length);
                wtr.Write(keyBuf);
            }
            if ((packet.Command & UdpPacket.CMD_OPTION_STATE_NICKNAME) != 0)
            {
                wtr.Write(extend.NickName);
            }
            if ((packet.Command & UdpPacket.CMD_OPTION_STATE_STATUS) != 0)
            {
                wtr.Write((int)extend.Status);
            }
            if ((packet.Command & UdpPacket.CMD_OPTION_STATE_PROFILE_PHOTO) != 0)
            {
                if (extend.ProfilePhoto == null)
                {
                    wtr.Write(0);
                }
                else
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        extend.ProfilePhoto.Save(ms, ImageFormat.Png);
                        wtr.Write((int)ms.Length);
                        byte[] buf = ms.ToArray();
                        wtr.Write(buf);
                    }
                }
            }
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
                Image image = extend.Image;
                image.Save(ms, image.RawFormat);

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
