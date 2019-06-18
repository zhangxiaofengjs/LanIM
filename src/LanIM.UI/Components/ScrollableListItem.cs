using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI.Components
{
    class ScrollableListItem
    {
        private const int DEFAULT_ITEM_HEIGHT = 56;

        public int Height { get; set; }

        public ScrollableListItem()
        {
            this.Height = DEFAULT_ITEM_HEIGHT;
        }
    }
}
