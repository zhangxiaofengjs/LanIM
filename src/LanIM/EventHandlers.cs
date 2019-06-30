using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM
{
    public delegate void SendEventHandler(object sender, SendEventArgs args);
    public delegate void UserStateEventHandler(object sender, UserStateChangeEventArgs args);
    public delegate void TextMessageReceivedHandler(object sender, TextMessageReceivedEventArgs args);
    public delegate void ImageReceivedHandler(object sender, ImageReceivedEventArgs args);
    public delegate void FileTransportRequestedHandler(object sender, FileTransportRequestedEventArgs args);
}
