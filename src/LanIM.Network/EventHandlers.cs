using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network
{
    public delegate void UdpClientSendEventHandler(object sender, UdpClientSendEventArgs args);
    public delegate void UdpClientReceiveEventHandler(object sender, UdpClientReceiveEventArgs args);
    public delegate void UdpClientErrorEventHandler(object sender, UdpClientErrorEventArgs args);
    
    public delegate void FileTransportEventHandler(object sender, FileTransportEventArgs args);
    public delegate void FileTransportErrorEventHandler(object sender, FileTransportErrorEventArgs args);
}
