using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.LanIM.Network
{
    //用户状态
    [Flags]
	public enum UpdateState
	{
        Status = 0x001,
        Photo = 0x002,
        PublicKey = 0x004,
        NickName = 0x008,
        IP = 0x0010,
        Port = 0x0020,
        All = Status | Photo | PublicKey | NickName | IP | Port,
    }
}
