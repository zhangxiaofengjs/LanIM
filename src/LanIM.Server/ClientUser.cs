using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Com.LanIM.Common;
using Com.LanIM.Common.Security;
using Com.LanIM.Network;
using Com.LanIM.UI;

namespace Com.LanIM.Server
{
    public class ClientUser : User
    {
        public DateTime LastHeartBeat { get; set; }
    }
}
