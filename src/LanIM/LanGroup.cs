using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM
{
    class LanGroup : LanUser
    {
        //群组成员
        public List<LanUser> Members = new List<LanUser>();
    }
}
