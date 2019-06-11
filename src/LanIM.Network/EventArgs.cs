using Com.LanIM.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network
{
    public class UdpClientEventArgs
    {
        private UdpPacket _packet;
        public UdpPacket Packet
        {
            get { return _packet; }
        }

        public UdpClientEventArgs(UdpPacket packet)
        {
            _packet = packet;
        }
    }

    public class UdpClientSendEventArgs  : UdpClientEventArgs
    {
        private bool _success = false;
        public bool Success
        {
            get
            {
                return this._success;
            }
        }

        public UdpClientSendEventArgs(UdpPacket packet, bool success)
       : base(packet)
        {
            this._success = success;
        }
    }

    public class UdpClientReceiveEventArgs: UdpClientEventArgs
    {
        public UdpClientReceiveEventArgs(UdpPacket packet)
        : base (packet)
        {
        }
    }

    public class FileTransportEventArgs
    {
        private TransportFile _file = null;
        public TransportFile File
        {
            get { return _file; }
        }

        public FileTransportEventArgs(TransportFile file)
        {
            _file = file;
        }
    }

    public class FileTransportErrorEventArgs: FileTransportEventArgs
    {
        public enum ErrorReason
        {
            None,
            FileOpenError,
            FileWriteError,
            NetworkError,
            NotExistTransportId,
        }
        private ErrorReason _reason = ErrorReason.None;

        public ErrorReason Reason { get => _reason; }

        public FileTransportErrorEventArgs(TransportFile file, ErrorReason reason)
            :base(file)
        {
            _reason = reason;
        }
    }
}
