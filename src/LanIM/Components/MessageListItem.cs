using Com.LanIM.Store.Models;
using Com.LanIM.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.LanIM.UI;
using System.Drawing;

namespace Com.LanIM.Components
{
    class MessageListItem : ScrollableListItem
    {
        public Message Message { get; set; }
        public LanUser User { get; set; }

        //public List<Rectangle> DrawingObjects = new List<Rectangle>();

        public MessageListItem()
        {
            //追加描绘部件
            //this.DrawingObjects.Add(new Rectangle(MARGIN, MARGIN, ICON_WIDTH, ICON_WIDTH));
            //this.DrawingObjects.Add(new Rectangle(MARGIN, 0, ICON_WIDTH, ICON_WIDTH));
        }

        public override string ToString()
        {
            return "user=" + User.ToString() + " message=" + Message.ToString();
        }
    }
}
