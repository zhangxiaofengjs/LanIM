using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network
{
    public delegate void UdpClientSendEventHandler(object sender, UdpClientSendEventArgs args);
    public delegate void UdpClientReceiveEventHandler(object sender, UdpClientReceiveEventArgs args);
}
