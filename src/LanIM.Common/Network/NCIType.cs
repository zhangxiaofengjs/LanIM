using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Network
{
    [Flags]
    public enum NCIType
    {
        UNKNOW = 0,
        Physical = 0x001,
        Wireless = 0x002,
        Virtual = 0x004,
    }
}
