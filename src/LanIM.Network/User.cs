using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Com.LanIM.Common;
using Com.LanIM.Common.Security;

namespace Com.LanIM.Network
{
    public class User
    {
        public virtual string ID { get { return MAC; } set { this.MAC = value; } }
        public virtual SecurityKeys SecurityKeys { get; set; } = new SecurityKeys();
        public virtual UserStatus Status { get; set; }
        public virtual string MAC { get; set; }
        public virtual IPAddress Address { get; set; }
        public virtual int Port { get; set; }
        public virtual string NickName { get; set; }
        public virtual Image ProfilePhoto { get; set; }

        public void Update(User u, UpdateState updateState)
        {
            //更新用户状态
            if ((updateState & UpdateState.PublicKey) != 0)
            {
                this.SecurityKeys.Public = u.SecurityKeys.Public;
            }
            if ((updateState & UpdateState.NickName) != 0)
            {
                this.NickName = u.NickName;
            }
            if ((updateState & UpdateState.Photo) != 0)
            {
                this.ProfilePhoto = u.ProfilePhoto;
            }
            if ((updateState & UpdateState.Status) != 0)
            {
                this.Status = u.Status;
            }
            if ((updateState & UpdateState.Port) != 0)
            {
                this.Port = u.Port;
            }
            if ((updateState & UpdateState.IP) != 0)
            {
                this.Address = u.Address;
            }
        }

        public override string ToString()
        {
            string str = string.Format("address={0}, port={1}, mac={2}, nickname={3}, status={4}, profilephoto={5}, secrkey={6}",
                this.Address, this.Port, this.MAC, this.NickName, this.Status, Convert.ToBase64String(this.SecurityKeys.Public));
            return str;
        }
    }
}
