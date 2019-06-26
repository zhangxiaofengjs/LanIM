using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = Com.LanIM.Store.Models.Message;

namespace Com.LanIM.UI.Components
{
    class MessageListBox : ScrollableList
    {
       public MessageListBox()
        {
        }

        protected override void OnMeasureItem(MeasureItemEventArgs args)
        {
            MessageListItem item = args.Item as MessageListItem;
            Message m = item.Message;

            Graphics g = args.Graphics;

            item.Height = TextRenderer.MeasureText(g, m.Content, args.Font).Height;
        }


        protected override void OnDrawItem(DrawItemEventArgs args)
        {
            MessageListItem item = args.Item as MessageListItem;
            Message m = item.Message;

            args.DrawBackground();

            Graphics g = args.Graphics;
            using (Brush brush = new SolidBrush(args.ForeColor))
            {
                g.DrawString(m.Content, args.Font, brush, args.Bounds.Location);
            }
        }
    }
}
