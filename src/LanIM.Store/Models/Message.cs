using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store.Models
{
    public class Message
    {
        public long ID { get; set; }
        public MessageType Type { get; private set; }
        public DateTime Time { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Content { get; set; }
        public bool Flag { get; set; }

        public Message(MessageType type)
        {
            this.Type = type;
            this.Time = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("ID={0}, Type={1}, Time={2:yyyy-MM-dd HH:mm:ss}, FromUserId={3}, ToUserId={4}, Content={5}, Flag={6}",
                ID,
                Type,
                Time,
                FromUserId,
                ToUserId,
                Content,
                Flag);
        }
    }
}
