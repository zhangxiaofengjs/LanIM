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
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Size Size
        {
            get
            {
                return new Size(this.Width, this.Height);
            }
        }
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(new Point(X, Y), Size);
            }
        }
        
        internal bool Selected { get; set; }
        public ScrollableListItem()
        {
            this.Height = DEFAULT_ITEM_HEIGHT;
            this.Selected = false;
        }
    }
}
