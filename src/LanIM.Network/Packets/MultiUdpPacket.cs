using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packets
{
    class MultiUdpPacket : UdpPacket
    {
        //包格式：版本（2字节)，包类型(1字节），包编号(8字节), 父包编号(8byte), 总长(4字节），开始位置（4byte），长度（4byte), 缓冲区
        public const int HEAD_SIZE = 31;

        public override byte Type
        {
            get
            {
                return Packet.PACKTE_TYPE_MULTI_UDP;
            }
        }

        public byte[] FragmentBuff { get; set; }
        public long ParentID { get; set; }
        public int TotalLength { get; set; }
        public int Position { get; set; }
        public int Length { get; set; }
        public int MaxFragmentLength { get; set; }

        public MultiUdpPacket(int totalLen)
        {
            this.FragmentBuff = new byte[totalLen];
            this.TotalLength = totalLen;
        }

        public MultiUdpPacket()
        {
        }

        public MultiUdpPacket(byte[] buff)
        {
            this.FragmentBuff = buff;
            this.TotalLength = buff.Length;
            this.Length = buff.Length;
        }

        internal void CopyFragmentBuff(MultiUdpPacket mp)
        {
            Array.Copy(mp.FragmentBuff, 0, FragmentBuff, mp.Position, mp.Length);
            this.Length += mp.Length;
        }
    }
}
