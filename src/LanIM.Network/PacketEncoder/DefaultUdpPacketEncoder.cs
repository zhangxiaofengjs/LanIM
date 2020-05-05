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

        public EncodeResult Encode()
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
                    int len = p.MaxFragmentLength;
                    
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
                        case UdpPacket.CMD_RETRANSMIT:
                            EncodeRetransmitExtend(wtr, _packet.Extend);
                            break;
                        case UdpPacket.CMD_USER_LIST:
                            EncodeUserListExtend(wtr, _packet.Extend);
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

        private void EncodeUserListExtend(BinaryWriter wtr, object extendObj)
        {
            UdpPacketUserListExtend extend = extendObj as UdpPacketUserListExtend;
            wtr.Write(extend.Users.Count);

            foreach (User u in extend.Users)
            {
                wtr.Write(u.Address.ToString());
                wtr.Write(u.Port);
                wtr.Write(u.MAC);
                wtr.Write((int)u.Status);
                wtr.Write(u.SecurityKeys.Public.Length);
                wtr.Write(u.SecurityKeys.Public);
            }
        }

        private static void EncodeRetransmitExtend(BinaryWriter wtr, object extendObj)
        {
            UdpPacketRetransExtend extend = extendObj as UdpPacketRetransExtend;
            wtr.Write(extend.PacketID);

            wtr.Write(extend.Address.ToString());
            wtr.Write(extend.Port);

            wtr.Write(extend.Length);
            wtr.Write(extend.PacketBuf);
        }

        private static void EncodeEntryExtend(BinaryWriter wtr, UdpPacket packet)
        {
            UdpPacketUserStateExtend extend = packet.Extend as UdpPacketUserStateExtend;
            if (extend == null)
            {
                throw new Exception("[EncodeEntryExtend]未想定附加包");
            }
            User user = extend.User;

            wtr.Write((int)extend.UpdateState);

            if ((extend.UpdateState & UpdateState.PublicKey) != 0)
            {
                byte[] keyBuf = user.SecurityKeys.Public;
                wtr.Write(keyBuf.Length);
                wtr.Write(keyBuf);
            }
            if ((extend.UpdateState & UpdateState.NickName) != 0)
            {
                wtr.Write(user.NickName);
            }
            if ((extend.UpdateState & UpdateState.Status) != 0)
            {
                wtr.Write((int)user.Status);
            }
            if ((extend.UpdateState & UpdateState.Photo) != 0)
            {
                if (user.ProfilePhoto == null)
                {
                    wtr.Write(0);
                }
                else
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        user.ProfilePhoto.Save(ms, ImageFormat.Png);
                        wtr.Write((int)ms.Length);
                        byte[] buf = ms.ToArray();
                        wtr.Write(buf);
                    }
                }
            }
            if ((extend.UpdateState & UpdateState.IP) != 0)
            {
                wtr.Write(user.Address.ToString());
            }
            if ((extend.UpdateState & UpdateState.Port) != 0)
            {
                wtr.Write(user.Port);
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

                wtr.Write(extend.FileName);
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
