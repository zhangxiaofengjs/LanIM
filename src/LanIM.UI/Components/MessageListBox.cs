using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI.Components
{
    class MessageListBox : ScrollableList
    {
            public void Add(Message m)
            {
            MessageListItem item = new MessageListItem();
                item.Message = m;
                this.Items.Add(item);
            }
    }
}
