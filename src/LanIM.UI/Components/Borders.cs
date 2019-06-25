using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI.Components
{
    [Flags]
    enum Borders
    {
        None = 0x000,
        Left = 0x0001,
        Top = 0x0010,
        Right = 0x0100,
        Bottom = 0x1000
    }
}
