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
        public UdpPacket Packet { get; } = null;

        public UdpClientEventArgs(UdpPacket packet)
        {
            Packet = packet;
        }
    }

    public class UdpClientSendEventArgs  : UdpClientEventArgs
    {
        public bool Success { get; } = false;

        public UdpClientSendEventArgs(UdpPacket packet, bool success)
       : base(packet)
        {
            this.Success = success;
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
        public TransportFile File { get; } = null;

        public FileTransportEventArgs(TransportFile file)
        {
            File = file;
        }
    }

    public class FileTransportErrorEventArgs: FileTransportEventArgs
    {
        public Errors Error { get; } = Errors.None;
        public string Message { get; private set; }
        public Exception Exception { get; private set; }
        public FileTransportErrorEventArgs(Errors error, string message, TransportFile file, Exception e)
            : base(file)
        {
            Error = error;
            Message = message;
            Exception = e;
        }
    }
}
