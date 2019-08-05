using Com.LanIM.Common.Network;
using Com.LanIM.Network;
using Com.LanIM.Network.Packets;
using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM
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

        /// <summary>
        /// 是否用户需要明示的包
        /// </summary>
        public bool IsUserPacket
        {
            get
            {
                switch (base.Packet.CMD)
                {
                    case UdpPacket.CMD_SEND_TEXT:
                    case UdpPacket.CMD_SEND_IMAGE:
                    case UdpPacket.CMD_SEND_FILE_REQUEST:
                        return true;
                    default: return false;
                }
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
        public UpdateState UpdateState { get; }
        public UserStateChangeEventArgs(LanUser user, UpdateState updateState)
            :base(user)
        {
            this.UpdateState = updateState;
        }
    }

    class TextMessageReceivedEventArgs : LanIMUserEventArgs
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

    class ImageReceivedEventArgs : LanIMUserEventArgs
    {
        private readonly Image _image;

        public long ID { get; }

        public Image Image
        {
            get { return _image; }
        }

        public ImageReceivedEventArgs(LanUser user, long id, Image image)
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

    public class ImageEventArgs
    {
        public Image Image { get; }

        public ImageEventArgs(Image image)
        {
            this.Image = image;
        }
    }

    public class NCIInfoEventArgs
    {
        public NCIInfo NCIInfo { get; }

        public NCIInfoEventArgs(NCIInfo n)
        {
            this.NCIInfo = n;
        }
    }

    public class SendMessageEventArgs
    {
        public Message Message { get; }

        public SendMessageEventArgs(Message n)
        {
            this.Message = n;
        }
    }
}
