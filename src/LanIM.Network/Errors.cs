using Com.LanIM.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Network
{
    public enum Errors
    {
        None,
        Unknow,
        NetworkError,
        NotExistTransportId,
        OutofSizePacket,
        ResolveError,
        EncodeError,
        FileOpenError,
        FileWriteError,
    }
}
