using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.PacketEncoder
{
    public class EncodeResult
    {
        public List<byte[]> Fragments { get; set; } = new List<byte[]>();
        public int Length
        {
            get
            {
                int len = 0;
                foreach(byte[] buff in Fragments)
                {
                    len += buff.Length;
                }
                return len;
            }
        }
        public EncodeResult(byte[] buff)
        {
            AddFragment(buff);
        }

        public EncodeResult()
        {
        }

        internal void AddFragment(byte[] buff)
        {
            this.Fragments.Add(buff);
        }

        public override string ToString()
        {
            string str = string.Format("{{encode count:{0},", this.Fragments.Count);
            foreach (byte[] item in this.Fragments)
            {
                str += "," + item.Length + "bytes";
            }
            return str;
        }
    }
}
