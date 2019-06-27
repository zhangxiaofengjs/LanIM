using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    public class ScrollableListItem
    {
        private const int DEFAULT_ITEM_HEIGHT = 56;

        //项目的区域（不包含在父控件中的滑块偏移量）
        public Rectangle Bounds { get; set; }
        public int Height
        {
            get
            {
                return Bounds.Height;
            }
        }

        public ScrollableListItem()
        {
            this.Bounds = new Rectangle(0,0, 0, DEFAULT_ITEM_HEIGHT);
        }
    }
}
