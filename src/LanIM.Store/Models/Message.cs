using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store.Models
{
    public abstract class Message
    {
        public long ID { get; set; }
        public DateTime Time { get; set; }
        public string UserId { get; set; }

        public Message()
        {
        }
    }
}
