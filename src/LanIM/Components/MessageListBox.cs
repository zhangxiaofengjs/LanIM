using Com.LanIM.Common;
using Com.LanIM.Store.Models;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static readonly int ICON_WIDTH = 36;
        public static readonly int PICTURE_THUMBNAIL_HEIGHT = 64;
        public static readonly int FILE_SEND_WIDTH = 215;
        public static readonly int FILE_SEND_HEIGHT = 100;
        private static readonly int MARGIN = 5;
        private static readonly int PROGRESS_PIE_D = 80;
        private static readonly Color SUCCESS_TITLE_COLOR = Color.FromArgb(150, 150, 150);
        private static readonly Color ERROR_TITLE_COLOR = Color.FromArgb(163, 21, 21);
        private static readonly Color TEXT_SELECTION_BACK_COLOR = Color.FromArgb(255, 201, 14);//Color.FromArgb(48, 148, 254);
        private static readonly Color TEXT_SELECTION_FORE_COLOR = Color.White;
        private static readonly Pen IMAGE_BORDER_PEN = new Pen(Color.White, 2.0f);
        private static readonly Pen IMAGE_BORDER_PEN_FOCUS = new Pen(Color.Gray, 1.0f);
        private static readonly Pen PROGRESS_PEN = new Pen(Color.FromArgb(150, 255, 255, 255), 2.0f);
        private static readonly Brush MESSAGELIST_PROGRESS_BACKGROUND_BRUSH = new SolidBrush(Color.FromArgb(200, 150, 150, 150));
        private static readonly Brush MESSAGELIST_PROGRESS_FILLED_BRUSH = new SolidBrush(Color.FromArgb(150, 255, 255, 255));

        //正在鼠标选择文本的消息
        private MessageListBoxSelectionInfo _selectionInfo;

        public MessageListBox()
        {
            _selectionInfo = new MessageListBoxSelectionInfo(this);
        }

        protected override void OnMeasureItem(UI.MeasureItemEventArgs args)
        {
            base.OnMeasureItem(args);

            MessageListItem item = args.Item as MessageListItem;
            Message m = item.Message;

            Graphics g = args.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            int x = MARGIN;
            int y = MARGIN;

            item.DrawingObjects.Clear();

            //头像
            DrawingObject dobj = new DrawingObject(DrawingObjectType.ProfilePhoto, new Rectangle(x, y, ICON_WIDTH, ICON_WIDTH));
            item.DrawingObjects.Add(dobj);

            //时间, 昵称，状态
            x += ICON_WIDTH + MARGIN;
            dobj = new DrawingObject(DrawingObjectType.Title, new Rectangle(x, y, this.ClientSize.Width - x, args.Font.Height));
            item.DrawingObjects.Add(dobj);

            y = MARGIN + args.Font.Height + MARGIN;
            int height = y;
            int width = 0;
            if (m.Type == MessageType.Text)
            {
                width = item.Width - ICON_WIDTH - 3 * MARGIN;
                //height = (int)g.MeasureString(m.Content, args.Font, width).Height;

                List<StringPart> spList = StringMeasurer.Measure(g, args.Font, width, m.Content);
                foreach (StringPart sp in spList)
                {
                    Rectangle textRect = sp.Bounds;
                    textRect.X += x;
                    textRect.Y += y;
                    dobj = new DrawingObject(DrawingObjectType.TextBlock, textRect);
                    dobj.Tag = new TextBlockObj(sp);
                    item.DrawingObjects.Add(dobj);

                    height += sp.Bounds.Height;
                }
            }
            else if (m.Type == MessageType.Image)
            {
                Image image = (m as ImageMessage).Image;
                //对于一些过小的图片最小32，此时图片会画歪掉，算了就这样
                width = Math.Max(32, (int)(1.0 * image.Width * PICTURE_THUMBNAIL_HEIGHT / image.Height));
                width = Math.Min(width, this.ClientSize.Width - 64);//当然也不要超过当前宽度

                dobj = new DrawingObject(DrawingObjectType.Image, new Rectangle(x, y, width, PICTURE_THUMBNAIL_HEIGHT));
                item.DrawingObjects.Add(dobj);

                height += PICTURE_THUMBNAIL_HEIGHT;
            }
            else if (m.Type == MessageType.File)
            {
                width = FILE_SEND_WIDTH;

                dobj = new DrawingObject(DrawingObjectType.File, new Rectangle(x, y, width, FILE_SEND_HEIGHT));
                item.DrawingObjects.Add(dobj);

                height += FILE_SEND_HEIGHT;
            }

            item.Height = Math.Max(ICON_WIDTH + MARGIN, height) + MARGIN;
        }

        protected override void OnDrawItem(UI.DrawItemEventArgs args)
        {
            MessageListItem item = args.Item as MessageListItem;
            Message m = item.Message;

            args.DrawBackground();

            Graphics g = args.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            Rectangle rect = item.Bounds;

            Rectangle drawingObjRect;
            Point pos = PointToClient(MousePosition);

            foreach (DrawingObject dobj in item.DrawingObjects)
            {
                drawingObjRect = dobj.Offset(rect.X, rect.Y);

                switch (dobj.Type)
                {
                    //头像
                    case DrawingObjectType.ProfilePhoto:
                        {
                            g.DrawImage(ProfilePhotoPool.GetPhoto(item.User.ID), drawingObjRect);

                            if (drawingObjRect.Contains(pos))
                            {
                                g.DrawRectangle(IMAGE_BORDER_PEN_FOCUS, drawingObjRect.X, drawingObjRect.Y, drawingObjRect.Width, drawingObjRect.Height);
                            }
                            break;
                        }
                    //时间, 昵称，状态
                    case DrawingObjectType.Title:
                        {
                            string text = item.User.NickName + "  " + m.Time.ToString("yyyy/MM/dd HH:mm:ss");
                            if (!m.Flag)
                            {
                                text += "  传送失败";
                            }
                            TextRenderer.DrawText(g, text,
                                args.Font, new Point((int)drawingObjRect.X, (int)drawingObjRect.Y)
                                , m.Flag ? SUCCESS_TITLE_COLOR : ERROR_TITLE_COLOR, TextFormatFlags.Left);
                            break;
                        }
                    //文本消息
                    case DrawingObjectType.TextBlock:
                        {
                            TextBlockObj tb = dobj.Tag as TextBlockObj;
                            StringPart sp = tb.StringPart;
                            int selStart = Math.Min(tb.SelectionStart, tb.SelectionEnd);
                            int selEnd = Math.Max(tb.SelectionStart, tb.SelectionEnd);

                            string str1 = selStart == -1 ? sp.String : sp.String.Substring(0, selStart);
                            string str2 = selStart == -1 ? "" : sp.String.Substring(selStart, tb.SelectionLength);
                            string str3 = selEnd == -1 ? "" : sp.String.Substring(selEnd + 1);

                            //选择前的文本
                            if (!string.IsNullOrEmpty(str1))
                            {
                                TextRenderer.DrawText(g, str1, sp.Font, new Point((int)drawingObjRect.X, (int)drawingObjRect.Y), args.ForeColor,
                                    StringMeasurer.TextFormatFlags);
                            }

                            //选择中文本
                            if (!string.IsNullOrEmpty(str2))
                            {
                                int w1 = StringMeasurer.Width(g, sp.Font, str1, sp.TextFormatFlags);
                                int w2 = StringMeasurer.Width(g, sp.Font, str2, sp.TextFormatFlags);
                                drawingObjRect.X += w1;
                                drawingObjRect.Width = w2;

                                g.FillRectangle(BrushPool.GetBrush(TEXT_SELECTION_BACK_COLOR), drawingObjRect);
                                TextRenderer.DrawText(g, str2, sp.Font, new Point((int)drawingObjRect.X, (int)drawingObjRect.Y),
                                        TEXT_SELECTION_FORE_COLOR, StringMeasurer.TextFormatFlags);

                                //选择后的文本
                                if (!string.IsNullOrEmpty(str3))
                                {
                                    drawingObjRect.X += w2;
                                    drawingObjRect.Width = dobj.Width - w1 - w2;
                                    TextRenderer.DrawText(g, str3, sp.Font, new Point((int)drawingObjRect.X, (int)drawingObjRect.Y), args.ForeColor,
                                        StringMeasurer.TextFormatFlags);
                                }
                            }
                            break;
                        }
                    case DrawingObjectType.Image:
                        {
                            Image image = (m as ImageMessage).Image;
                            g.DrawImage(image, drawingObjRect);

                            if (drawingObjRect.Contains(pos))
                            {
                                g.DrawRectangle(IMAGE_BORDER_PEN_FOCUS, drawingObjRect.X, drawingObjRect.Y, drawingObjRect.Width, drawingObjRect.Height);
                            }
                            else
                            {
                                g.DrawRectangle(IMAGE_BORDER_PEN, drawingObjRect.X, drawingObjRect.Y, drawingObjRect.Width, drawingObjRect.Height);
                            }
                            break;
                        }
                    case DrawingObjectType.File:
                        {
                            FileMessage fm = m as FileMessage;

                            //底背景
                            g.FillRectangle(Brushes.White, drawingObjRect);
                            if (drawingObjRect.Contains(pos))
                            {
                                g.DrawRectangle(IMAGE_BORDER_PEN_FOCUS, drawingObjRect.X, drawingObjRect.Y, drawingObjRect.Width, drawingObjRect.Height);
                            }
                            else
                            {
                                g.DrawRectangle(IMAGE_BORDER_PEN, drawingObjRect.X, drawingObjRect.Y, drawingObjRect.Width, drawingObjRect.Height);
                            }

                            //文件名，文件大小，ICON
                            Image image = FileIconPool.GetIcon(fm.OriginFilePath);
                            int imageWidth = 64;
                            int imageHeight = 64 * image.Height / image.Width;

                            Rectangle imageRect = new Rectangle(drawingObjRect.X + FILE_SEND_WIDTH - imageWidth - MARGIN, drawingObjRect.Y + MARGIN, imageWidth, FILE_SEND_HEIGHT - Font.Height - MARGIN * 3);
                            Rectangle fileNameRect = new Rectangle(drawingObjRect.X + MARGIN, drawingObjRect.Y + MARGIN, FILE_SEND_WIDTH - imageWidth - MARGIN * 3, FILE_SEND_HEIGHT - Font.Height - MARGIN * 3);
                            Rectangle fontRect = new Rectangle(drawingObjRect.X + MARGIN, drawingObjRect.Y + FILE_SEND_HEIGHT - Font.Height - MARGIN, FILE_SEND_WIDTH - MARGIN * 2, Font.Height);
                            Rectangle pieRect = new Rectangle(drawingObjRect.X + (FILE_SEND_WIDTH - PROGRESS_PIE_D) / 2, drawingObjRect.Y + (FILE_SEND_HEIGHT - PROGRESS_PIE_D) / 2, PROGRESS_PIE_D, PROGRESS_PIE_D);
                            Rectangle processRect = new Rectangle(drawingObjRect.X, drawingObjRect.Y + (FILE_SEND_HEIGHT - Font.Height * 2) / 2, FILE_SEND_WIDTH, Font.Height * 2);

                            //当文件名超过显示部分时，中间用省略号表示
                            string str = fm.FileName;
                            SizeF s = g.MeasureString(fm.FileName, Font, new System.Drawing.SizeF((float)fileNameRect.Width, (float)fileNameRect.Height), StringFormat.GenericDefault, out int count, out int lines);
                            if (s.Height >= fileNameRect.Height & str.Length >= count)
                            {
                                //由于会出现半行的情况，直接写死了所取的字符串数目
                                str = str.Substring(0, 36) + "・・・" + str.Substring(str.Length - 4);
                            }
                            g.DrawImage(image, imageRect.X, imageRect.Y, imageWidth, imageHeight);
                            g.DrawString(str, args.Font, Brushes.Black, fileNameRect);
                            TextRenderer.DrawText(g, LanFile.HumanReadbleLen(fm.FileLength), args.Font, fontRect, Color.FromArgb(0, 0, 0), TextFormatFlags.Left);

                            if (item.State == MessageState.Receiving ||
                                item.State == MessageState.Sending)
                            {
                                //接受进度
                                g.FillRectangle(MESSAGELIST_PROGRESS_BACKGROUND_BRUSH, drawingObjRect);

                                g.DrawPie(PROGRESS_PEN, pieRect, 0, 360);
                                g.FillPie(MESSAGELIST_PROGRESS_FILLED_BRUSH, pieRect, 0, 360 * item.Progress / 100);

                                //TODO 显示剩余时间，传输速度
                                TextRenderer.DrawText(g, item.Progress + "%" + "\r\n" + LanFile.HumanReadbleLen(item.FileTransportedLength) + "/" + LanFile.HumanReadbleLen(fm.FileLength),
                                    args.Font, processRect, Color.FromArgb(0, 0, 0), TextFormatFlags.HorizontalCenter);
                            }
                            break;
                        }
                }
            }
        }

        internal void Invalidate(MessageListItem item)
        {
            if (item == null)
            {
                this.Invalidate();
            }
            else
            {
                this.Invalidate(item.Bounds);
            }
        }

        protected override void OnItemClicked(ItemClickedEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                MessageListItem item = args.Item as MessageListItem;
                Message m = item.Message;
                Rectangle rect = item.Bounds;

                foreach (DrawingObject dobj in item.DrawingObjects)
                {
                    RectangleF drawingObjRect = dobj.Offset(rect.X, rect.Y);

                    switch (dobj.Type)
                    {
                        //头像
                        case DrawingObjectType.ProfilePhoto:
                            if (drawingObjRect.Contains(args.Location))
                            {
                                UserProfileControl control = new UserProfileControl();
                                control.User = item.User;
                                control.Show(this.PointToScreen(args.Location));
                                return;
                            }
                            break;
                        case DrawingObjectType.Image:
                        case DrawingObjectType.File:
                            {
                                if (drawingObjRect.Contains(args.Location))
                                {
                                    //点击了图像，显示大图, 或者打开文件
                                    string path;
                                    if (m.Type == MessageType.Image)
                                    {
                                        path = (m as ImageMessage).OriginPath;
                                    }
                                    else
                                    {
                                        path = (m as FileMessage).OriginFilePath;
                                    }
                                    if (LanFile.Exists(path))
                                    {
                                        Process.Start(path);
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            else if (args.Button == MouseButtons.Right)
            {
                //交给显示右键菜单
            }
        }

        protected override void OnItemHover(ItemHoverEventArgs args)
        {
            MessageListItem item = args.Item as MessageListItem;
            Rectangle rect = item.Bounds;

            foreach (DrawingObject dobj in item.DrawingObjects)
            {
                RectangleF drawingObjRect = dobj.Offset(rect.X, rect.Y);
                switch (dobj.Type)
                {
                    case DrawingObjectType.ProfilePhoto:
                    case DrawingObjectType.Image:
                    case DrawingObjectType.File:
                        {
                            //头像 鼠标变化
                            if (drawingObjRect.Contains(args.Location))
                            {
                                this.Cursor = Cursors.Hand;
                                return;
                            }
                            break;
                        }
                    case DrawingObjectType.TextBlock:
                        {
                            if (drawingObjRect.Contains(args.Location))
                            {
                                this.Cursor = Cursors.IBeam;
                                return;
                            }
                            break;
                        }
                }
            }

            this.Cursor = Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                _selectionInfo.SetSelectionStartItem(e.Location);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left)
            {
                _selectionInfo.SetSelectionEndItem(e.Location);
            }
        }
    }
}
