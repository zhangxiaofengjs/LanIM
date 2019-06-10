using Com.LanIM.Network.Packet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    class LanIMEventArgs
    {
        private UdpPacket _packet;
        public UdpPacket Packet
        {
            get { return _packet; }
        }

        public LanIMEventArgs(UdpPacket packet)
        {
            _packet = packet;
        }
    }

    class SendEventArgs  : LanIMEventArgs
    {
        private bool _success = false;
        public bool Success
        {
            get
            {
                return this._success;
            }
        }

        public SendEventArgs(UdpPacket packet, bool success)
       : base(packet)
        {
            this._success = success;
        }
    }

    class ReceiveEventArgs: LanIMEventArgs
    {
        public ReceiveEventArgs(UdpPacket packet)
        : base (packet)
        {
        }
    }

    class UserStateChangeEventArgs
    {
        private LanUser _user;
        public LanUser User
        {
            get { return _user; }
        }

        public UserStateChangeEventArgs(LanUser user)
        {
            _user = user;
        }
    }

    class TextMessageReceivedEventArgs
    {
        private LanUser _user;
        private String _meassage;
        public LanUser User
        {
            get { return _user; }
        }

        public string Message
        {
            get { return _meassage; }
        }

        public TextMessageReceivedEventArgs(LanUser user, string msg)
        {
            _user = user;
            _meassage = msg;
            
        }
    }

    class ImageReceivedEventArgs
    {
        private LanUser _user;
        private Image _image;
        public LanUser User
        {
            get { return _user; }
        }

        public Image Image
        {
            get { return _image; }
        }

        public ImageReceivedEventArgs(LanUser user, Image image)
        {
            _user = user;
            _image = image;
        }
    }
}
