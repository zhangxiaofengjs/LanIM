using Com.LanIM.Store.Models;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Com.LanIM.Components.MessageListItem;
using Message = Com.LanIM.Store.Models.Message;

namespace Com.LanIM.Components
{
    class MessageListBox : ScrollableList
    {
        //假设绘制的最大高度
        private static int MAX_HEIGHT = Screen.PrimaryScreen.WorkingArea.Height;
        private static int ICON_WIDTH = 36;
        public static int PICTURE_THUMBNAIL_HEIGHT = 64;
        public static int FILE_SEND_WIDTH = 215;
        public static int FILE_SEND_HEIGHT = 100;
        private static int MARGIN = 5;
        private static Color SUCCESS_TITLE_COLOR = Color.FromArgb(150, 150, 150);
        private static Color ERROR_TITLE_COLOR = Color.FromArgb(163, 21, 21);
        private static Pen IMAGE_BORDER_PEN = new Pen(Color.White, 2.0f);
        private static Pen IMAGE_BORDER_PEN_FOCUS = new Pen(Color.Gray, 2.0f);
        
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
            int x = MARGIN;
            int y = MARGIN;

            //头像
            item.DrawingObjects[0].Bounds = new Rectangle(x, y, ICON_WIDTH, ICON_WIDTH);

            //时间, 昵称，状态
            x += ICON_WIDTH + MARGIN;
            item.DrawingObjects[1].Bounds = new Rectangle(x, y, this.ClientSize.Width - x, args.Font.Height);

            y = MARGIN + args.Font.Height + MARGIN;
            int height = 0;
            int width = 0;
            if (m.Type == MessageType.Text)
            {
                width = rect.Width - ICON_WIDTH - 3 * MARGIN;
                height = (int)g.MeasureString(m.Content, args.Font, width).Height;
            }
            else if (m.Type == MessageType.Image)
            {
                Image image = (m as ImageMessage).Image;
                width = (int)(1.0 * image.Width * PICTURE_THUMBNAIL_HEIGHT / image.Height);
                height = PICTURE_THUMBNAIL_HEIGHT;
            }
            else if (m.Type == MessageType.File)
            {
                width = FILE_SEND_WIDTH;
                height = FILE_SEND_HEIGHT;
            }

            //消息内容
            item.DrawingObjects[2].Bounds = new Rectangle(x, y, width, height);

            rect.Height = Math.Max(ICON_WIDTH, item.DrawingObjects[2].Bounds.Bottom + MARGIN);

            item.Bounds = rect;
        }

        protected override void OnDrawItem(UI.DrawItemEventArgs args)
        {
            System.Diagnostics.Trace.WriteLine(2222);
            MessageListItem item = args.Item as MessageListItem;
            Message m = item.Message;

            args.DrawBackground();

            Graphics g = args.Graphics;

            Rectangle rect = args.Bounds;
            Rectangle drawingObjRect;

            //头像
            drawingObjRect = item.DrawingObjects[0].Bounds;
            drawingObjRect.X += rect.X;
            drawingObjRect.Y += rect.Y;
            g.DrawImage(ProfilePhotoPool.GetPhoto(item.User.ID), drawingObjRect);

            //时间, 昵称，状态
            drawingObjRect = item.DrawingObjects[1].Bounds;
            drawingObjRect.X += rect.X;
            drawingObjRect.Y += rect.Y;

            string text = item.User.NickName + "  " + m.Time.ToString("yyyy/MM/dd HH:mm:ss");
            if (!m.Flag)
            {
                text += "  传送失败";
            }

            TextRenderer.DrawText(g, text, 
                args.Font, drawingObjRect, m.Flag ? SUCCESS_TITLE_COLOR : ERROR_TITLE_COLOR, TextFormatFlags.Left);

            //消息
            drawingObjRect = item.DrawingObjects[2].Bounds;
            drawingObjRect.X += rect.X;
            drawingObjRect.Y += rect.Y;

            if (m.Type == MessageType.Text)
            {
                using (Brush brush = new SolidBrush(args.ForeColor))
                {
                    g.DrawString(m.Content, args.Font, brush, drawingObjRect, StringFormat.GenericDefault);
                }
            }
            else if (m.Type == MessageType.Image)
            {
                Image image = (m as ImageMessage).Image;
                g.DrawImage(image, drawingObjRect);

                if (drawingObjRect.Contains(PointToClient(MousePosition)))
                {
                    g.DrawRectangle(IMAGE_BORDER_PEN_FOCUS, drawingObjRect);
                }
                else
                {
                    g.DrawRectangle(IMAGE_BORDER_PEN, drawingObjRect);
                }
            }
            else if (m.Type == MessageType.File)
            {
                g.FillRectangle(Brushes.White, drawingObjRect);
                g.DrawImage(Properties.Resources.file, drawingObjRect);
                g.DrawString("一个文件", args.Font, Brushes.Black, drawingObjRect);
            }
            //画的时候更新一下，以便点击的时候用
            item.DrawingObjects[2].Offset = rect.Y;
        }

        protected override void OnItemClicked(ItemClickedEventArgs args)
        {
            base.OnItemClicked(args);

            MessageListItem item = args.Item as MessageListItem;
            Message m = item.Message;

            if (m.Type == MessageType.Image)
            {
                Rectangle drawingObjRect = item.DrawingObjects[2].Bounds;
                drawingObjRect.Y += item.DrawingObjects[2].Offset;
                if (drawingObjRect.Contains(args.Location))
                {
                    //点击了图像，显示大图
                    MessageBox.Show("点击了图片");
                }
            }
        }

        protected override void OnItemHover(ItemHoverEventArgs args)
        {
            MessageListItem item = args.Item as MessageListItem;

            if (item.Message.Type == MessageType.Image)
            {
                Rectangle drawingObjRect = item.DrawingObjects[2].Bounds;
                drawingObjRect.Y += item.DrawingObjects[2].Offset;
                if (drawingObjRect.Contains(args.Location))
                {
                    if (this.Cursor != Cursors.Hand)
                    {
                        this.Cursor = Cursors.Hand;
                    }
                }
                else
                {
                    if (this.Cursor != Cursors.Default)
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }
    }
}
