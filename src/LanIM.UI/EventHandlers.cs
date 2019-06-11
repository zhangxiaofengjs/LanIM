using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    delegate void SendEventHandler(object sender, SendEventArgs args);
    delegate void UserStateEventHandler(object sender, UserStateChangeEventArgs args);
    delegate void TextMessageReceivedHandler(object sender, TextMessageReceivedEventArgs args);
    delegate void ImageReceivedHandler(object sender, ImageReceivedEventArgs args);
    delegate void FileTransportRequestedHandler(object sender, FileTransportRequestedEventArgs args);
}
