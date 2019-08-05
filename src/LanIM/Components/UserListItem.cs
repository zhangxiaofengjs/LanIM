using Com.LanIM.Components;
using Com.LanIM.Store;
using Com.LanIM.Store.Models;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Components
{
    internal class UserListItem : ScrollableListItem
    {
        public LanUser User { get; set; }
        public UserChatControl ChatControl { get; set; }
        public string UserID
        {
            get
            {
                return User.ID;
            }
        }

        /// <summary>
        /// 未读消息数
        /// </summary>
        public int UnreadMessageCount { get; set; }

        public List<MessageListItem> WaitDisplayMessages { get; set; } = new List<MessageListItem>();

        public UserListItem()
        {
        }

        public UserListItem(LanUser user)
        {
            this.User = user;
        }

        public override string ToString()
        {
            return User.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj is LanUser)
            {
                return (obj as LanUser).ID == this.User.ID;
            }
            return base.Equals(obj);
        }

        internal MessageListItem GetWaitDisplayMessageItem(long id)
        {
            //因为后后加的，倒过来循环速度更快应该
            for (int i = WaitDisplayMessages.Count - 1; i > -1; i--)
            {
                MessageListItem item = WaitDisplayMessages[i];

                if (item.ID == id)
                {
                    return item;
                }
            }

            return null;
        }

        internal void Save()
        {
            ContacterMapper contacterMapper = new ContacterMapper();
            Contacter c = new Contacter();
            c.MAC = this.User.ID;
            c.IP = this.User.IP == null ? "0.0.0.0" : this.User.IP.ToString();
            c.NickName = this.User.NickName;

            contacterMapper.Add(c);
        }

        internal void Update()
        {
            ContacterMapper contacterMapper = new ContacterMapper();
            contacterMapper.Update(this.User.Contacter);
        }
    }
}
