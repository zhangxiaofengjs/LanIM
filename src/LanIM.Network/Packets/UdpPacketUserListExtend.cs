using Com.LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network.Packets
{
    //上线附加信息包
    public class UdpPacketUserListExtend
    {
        public List<User> Users { get; set; } = new List<User>();

        public UdpPacketUserListExtend()
        {

        }

        public void AddUser(User u)
        {
            Users.Add(u);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(User u in Users)
            {
                sb.AppendLine(u.ToString());
            }
            string str = string.Format("{{users=\n{0}}}",sb.ToString());
            return str;
        }
    }
}
