using Com.LanIM.Network;
using Com.LanIM.Network.Packets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM
{
    public class LanIMUserEventArgs
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

    public class LanIMPacketEventArgs
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

    public class SendEventArgs  : LanIMPacketEventArgs
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

    public class UserStateChangeEventArgs : LanIMUserEventArgs
    {
        public UserStateChangeEventArgs(LanUser user)
            :base(user)
        {
        }
    }

    public class TextMessageReceivedEventArgs : LanIMUserEventArgs
    {
        public long ID { get; }

        public string Message { get; }

        public TextMessageReceivedEventArgs(LanUser user, long id, string msg)
            :base(user)
        {
            ID = id;
            Message = msg;
        }
    }

    public class ImageReceivedEventArgs : LanIMUserEventArgs
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

    public class FileTransportRequestedEventArgs : LanIMUserEventArgs
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
