using Com.LanIM.Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Security
{
    public class SecurityKeys
    {
        public byte[] Public { get; set; }
        public byte[] Private { get; set; }
    }
}
