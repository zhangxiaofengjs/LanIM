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
        internal enum DrawingObjectType
        {
            ProfilePhoto,
            Title,
            MessageArea,
        }

        internal class DrawingObject
        {
            public DrawingObjectType Type { get; }
            public Rectangle Bounds { get; set; }
            public int Offset { get; set; }
            public DrawingObject(DrawingObjectType type)
            {
                this.Type = type;
            }
        }

        public enum MessageState
        {
            Sending,
            SendSuccess,
            SendError,
            Received,
        }

        public long ID { get; set; }
        public Message Message { get; set; }
        public LanUser User { get; set; }
        public MessageState State { get; set; }

        internal List<DrawingObject> DrawingObjects = new List<DrawingObject>();

        public MessageListItem()
        {
            //追加描绘部件
            //头像
            this.DrawingObjects.Add(new DrawingObject(DrawingObjectType.ProfilePhoto));
            //标题
            this.DrawingObjects.Add(new DrawingObject(DrawingObjectType.Title));
            //消息区
            this.DrawingObjects.Add(new DrawingObject(DrawingObjectType.MessageArea));
        }

        public override string ToString()
        {
            return "user=" + User.ToString() + " message=" + Message.ToString();
        }
    }
}
