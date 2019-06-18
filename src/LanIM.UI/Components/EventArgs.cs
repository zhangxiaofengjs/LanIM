using Com.LanIM.Network;
using Com.LanIM.Network.Packets;
using LanIM.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI.Components
{
    class MeasureItemEventArgs
    {
        public Graphics Graphics { get; }
        public ScrollableListItem Item { get; }
        public int Index { get; }

        public MeasureItemEventArgs(int index, ScrollableListItem item, Graphics g)
        {
            this.Index = index;
            this.Item = item;
            this.Graphics = g; 
        }
    }
}
