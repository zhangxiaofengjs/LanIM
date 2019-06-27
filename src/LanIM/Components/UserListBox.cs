using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Components
{
    class UserListBox : ScrollableList
    {
        public LanUser SelectedUser
        {
            get
            {
                UserListItem item = this.SelectedItem as UserListItem;
                if(item == null)
                {
                    return null;
                }

                return item.User;
            }
        }

        public UserListBox()
        {
        }

        public void Add(LanUser user)
        {
            UserListItem item = new UserListItem();
            item.User = user;
            this.Items.Add(item);
        }

        private UserListItem this[string userId]
        {
            get
            {
                foreach (UserListItem item in this.Items)
                {
                    if (item.User.ID == userId)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public bool ContainsUser(LanUser user)
        {
            return this[user.ID] != null;
        }

        public void UpdateUser(LanUser user)
        {
            UserListItem item = this[user.ID];
            //TODO 更加精细的刷新
            //this.Invalidate(item);
            this.Invalidate();
        }

        public void AddUser(LanUser user)
        {
            UserListItem item = new UserListItem(user);
            this.Items.Add(item);
        }
    }
}
