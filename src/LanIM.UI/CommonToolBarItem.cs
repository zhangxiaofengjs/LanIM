using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI
{
    public class CommonToolBarItem
    {
        public Rectangle Bounds { get; internal set; }
        public Image Image { get; set; }
        public Image ImageFocus { get; set; }
        public string Name { get; set; }

        public event EventHandler Click;

        public CommonToolBarItem()
        {
        }

        internal void RaiseClickEvent()
        {
            this.Click?.Invoke(this, new EventArgs());
        }
    }
}
