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
    public class UdpPacketUserStateExtend
    {
        public User User { get; set; }
        public UpdateState UpdateState { get; set; }

        //public byte[] PublicKey { get; set; }
        //public string NickName { get; set; }
        //public Image ProfilePhoto { get; set; }
        //public UserStatus Status { get; set; }

        public UdpPacketUserStateExtend()
        {
        }

        public override string ToString()
        {
            string str = string.Format("{{updatestate={4}, public key={0}, nickname={1}, status={2}, photo={3}}}",
                    User.SecurityKeys.Public == null? "" : Convert.ToBase64String(User.SecurityKeys.Public),
                    User.NickName,
                    User.Status,
                    User.ProfilePhoto == null ? "null" : User.ProfilePhoto.ToString(),
                    UpdateState);
            return str;
        }
    }
}
