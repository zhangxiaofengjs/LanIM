using Com.LanIM.Network;
using Com.LanIM.Network.Packets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    class LanIMUserEventArgs
    {
        private readonly LanUser _user;
        public LanUser User
        {
            get { return _user; }
        }

        public LanIMUserEventArgs(LanUser user)
        {
            _user = user;
        }
    }

    class LanIMPacketEventArgs
    {
        private readonly UdpPacket _packet;
        public UdpPacket Packet
        {
            get { return _packet; }
        }

        public LanIMPacketEventArgs(UdpPacket packet)
        {
            _packet = packet;
        }
    }

    class SendEventArgs  : LanIMPacketEventArgs
    {
        private readonly bool _success = false;
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

    class UserStateChangeEventArgs : LanIMUserEventArgs
    {
        public UserStateChangeEventArgs(LanUser user)
            :base(user)
        {
        }
    }

    class TextMessageReceivedEventArgs : LanIMUserEventArgs
    {
        private readonly String _meassage;

        public string Message
        {
            get { return _meassage; }
        }

        public TextMessageReceivedEventArgs(LanUser user, string msg)
            :base(user)
        {
            _meassage = msg;
            
        }
    }

    class ImageReceivedEventArgs : LanIMUserEventArgs
    {
        private readonly Image _image;

        public Image Image
        {
            get { return _image; }
        }

        public ImageReceivedEventArgs(LanUser user, Image image)
            : base(user)
        {
            _image = image;
        }
    }

    class FileTransportRequestedEventArgs : LanIMUserEventArgs
    {
        private readonly TransportFile _file;

        public TransportFile File
        {
            get { return _file; }
        }

        public FileTransportRequestedEventArgs(LanUser user, TransportFile file)
            : base(user)
        {
            this._file = file;
        }

        public override string ToString()
        {
            return string.Format("{{user={0}, file={1}}}",
                this.User, this.File);
        }
    }
}
