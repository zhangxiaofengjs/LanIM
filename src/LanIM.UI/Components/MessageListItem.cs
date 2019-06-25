using Com.LanIM.Store.Models;
using Com.LanIM.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI.Components
{
    class MessageListItem : ScrollableListItem
    {
        public Message Message { get; set; }

        public override string ToString()
        {
            return Message.ToString();
        }
    }
}
