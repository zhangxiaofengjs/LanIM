using Com.LanIM.Store.Models;
using Com.LanIM.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.LanIM.UI;
using System.Drawing;
using Com.LanIM.Store;

namespace Com.LanIM.Components
{
    class MessageListItem : ScrollableListItem
    {
        internal enum DrawingObjectType
        {
            ProfilePhoto,
            Title,
            Image,
            File,
            TextBlock,
        }

        internal class TextBlockObj
        {
            public StringPart StringPart{ get; }
            public string Text { get { return StringPart.String; } }
            public int Length { get { return Text.Length; } }
            public int SelectionStart { get; set; }
            public int SelectionEnd { get; set; }
            public int SelectionLength
            {
                get
                {
                    if (SelectionStart == -1 || SelectionEnd == -1) return 0;
                    return Math.Abs(SelectionEnd - SelectionStart) + 1;
                }
            }
            public string SelectedText
            {
                get
                {
                    if(SelectionStart == -1 || SelectionStart == -1)
                    {
                        return string.Empty;
                    }
                    int startIndex = Math.Min(SelectionStart, SelectionEnd);
                    return Text.Substring(startIndex, SelectionLength);
                }
            }
            
            public TextBlockObj(StringPart sp)
            {
                this.StringPart = sp;
                ClearSelection();
            }
            internal void ClearSelection()
            {
                SelectionStart = -1;
                SelectionEnd = -1;
            }

            internal void SelectAll()
            {
                SelectionToStart();
                SelectionToEnd();
            }

            internal void SelectionToEnd()
            {
                SelectionEnd = StringPart.String.Length - 1;
            }

            internal void SelectionToStart()
            {
                SelectionStart = 0;
            }
        }
        internal class DrawingObject
        {
            public DrawingObjectType Type { get; }
            public int X { get { return Bounds.X; } }
            public int Width { get { return Bounds.Width; } }
            public Rectangle Bounds { get; set; }
            public Object Tag { get; set; }
            public DrawingObject(DrawingObjectType type, Rectangle bounds)
            {
                this.Type = type;
                this.Bounds = bounds;
            }

            internal Rectangle Offset(int x, int y)
            {
                return new Rectangle(Bounds.X + x, Bounds.Y + y, Bounds.Width, Bounds.Height);
            }
        }

        public enum MessageState
        {
            Sending,
            SendSuccess,
            SendError,
            Receiving,
            Received,
            ReceiveError,
        }

        public long ID { get; set; }
        public Message Message { get; set; }
        public LanUser User { get; set; }
        public MessageState State { get; set; }
        internal List<DrawingObject> DrawingObjects = new List<DrawingObject>();
        private MessageHistoryMapper _messageHistoryMapper = new MessageHistoryMapper();

        //以下文件传输用
        public long FileTransportedLength { get; internal set; }
        public long FileTransportedSpeed { get; internal set; }
        public int Progress { get; internal set; }
        public string SelectedText
        {
            get
            {
                string str = "";
                bool prevWrap = false;
                foreach (DrawingObject dobj in DrawingObjects)
                {
                    if(dobj.Type == DrawingObjectType.TextBlock)
                    {
                        TextBlockObj tb = dobj.Tag as TextBlockObj;
                        if (tb.SelectionLength != 0)
                        {
                            if (!prevWrap && !string.IsNullOrEmpty(str))
                            {
                                str += "\r\n";
                            }
                            str += tb.SelectedText;
                            prevWrap = tb.StringPart.Wrap;
                        }
                    }
                }
                return str;
            }
        }

        public MessageListItem()
        {
        }

        public void Save()
        {
            _messageHistoryMapper.Add(this.Message);
        }

        public void Update()
        {
            _messageHistoryMapper.UpdateState(this.Message);
        }
        
        public override string ToString()
        {
            return "user=" + User.ToString() + " message=" + Message.ToString();
        }
    }
}
