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

    public class UdpClientErrorEventArgs
    {
        public Errors Error { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public UdpClientErrorEventArgs(Errors error, string message, Exception e)
        {
            this.Error = error;
            this.Message = message;
            this.Exception = e;
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
        private Errors _error = Errors.None;

        public Errors Error { get => _error; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }
        public FileTransportErrorEventArgs(Errors error, string message, TransportFile file, Exception e)
            : base(file)
        {
            _error = error;
            Message = message;
            Exception = e;
        }
    }
}
