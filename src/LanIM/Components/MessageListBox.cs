using Com.LanIM.Store.Models;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = Com.LanIM.Store.Models.Message;

namespace Com.LanIM.Components
{
    class MessageListBox : ScrollableList
    {
        //假设绘制的最大高度
        private static int MAX_HEIGHT = Screen.PrimaryScreen.WorkingArea.Height;
        private static int ICON_WIDTH = 36;
        public static int PICTURE_THUMBNAIL_WIDTH = 64;
        private static int MARGIN = 5;

        public MessageListBox()
        {
        }

        protected override void OnMeasureItem(UI.MeasureItemEventArgs args)
        {
            base.OnMeasureItem(args);

            MessageListItem item = args.Item as MessageListItem;
            Message m = item.Message;

            Graphics g = args.Graphics;

            Rectangle rect = item.Bounds;

            int height = 0;
            if (m.Type == MessageType.Text)
            {
                height = (int)g.MeasureString(m.Content, args.Font, new Size(rect.Width - ICON_WIDTH - 3 * MARGIN, MAX_HEIGHT), StringFormat.GenericDefault).Height;
            }
            else if (m.Type == MessageType.Image)
            {
                height = PICTURE_THUMBNAIL_WIDTH;
            }

            //3个间距+ 时间行
            height += args.Font.Height + MARGIN * 3;

            rect.Height = Math.Max(ICON_WIDTH, height);

            item.Bounds = rect;
        }


        protected override void OnDrawItem(UI.DrawItemEventArgs args)
        {
            MessageListItem item = args.Item as MessageListItem;
            Message m = item.Message;

            args.DrawBackground();

            Graphics g = args.Graphics;

            Rectangle rect = args.Bounds;

            //头像
            int x = rect.X + MARGIN;
            int y = rect.Y + MARGIN;
            g.DrawImage(ProfilePhotoPool.GetPhoto(item.User.ID), x, y, ICON_WIDTH, ICON_WIDTH);

            //时间
            x += ICON_WIDTH + MARGIN;
            TextRenderer.DrawText(g, item.User.NickName + "  " + m.Time.ToString("yyyy/MM/dd HH:mm:ss"), 
                args.Font, new Point(x, y), Color.FromArgb(150,150,150));

            //消息
            y += args.Font.Height + MARGIN;
            rect.X = x;
            rect.Y = y;
            if (m.Type == MessageType.Text)
            {
                using (Brush brush = new SolidBrush(args.ForeColor))
                {
                    g.DrawString(m.Content, args.Font, brush, rect, StringFormat.GenericDefault);
                }
            }
            else if (m.Type == MessageType.Image)
            {
                rect.Width = PICTURE_THUMBNAIL_WIDTH;
                rect.Height = PICTURE_THUMBNAIL_WIDTH;
                g.DrawImage((m as ImageMessage).Image, rect);

                g.DrawRectangle(Pens.White, rect);
            }
        }
    }
}
