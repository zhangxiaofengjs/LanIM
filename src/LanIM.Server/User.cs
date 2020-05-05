using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Com.LanIM.Common;
using Com.LanIM.Common.Security;

namespace Com.LanIM.Server
{
    class User
    {
        public string ID { get { return MAC; } set { this.MAC = value; } }
        public SecurityKeys SecurityKeys { get; set; }
        public UserStatus Status { get; internal set; }
        public string MAC { get; set; }
        public int Port { get; internal set; }
        public IPAddress IP { get; internal set; }
        public string NickName { get; internal set; }
    }
}
